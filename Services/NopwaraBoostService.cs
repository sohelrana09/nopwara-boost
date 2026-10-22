using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Widgets.NopwaraBoost.Models;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Orders;

namespace Nop.Plugin.Widgets.NopwaraBoost.Services;

/// <summary>
/// Prepares the data of the conversion widgets
/// </summary>
public class NopwaraBoostService
{
    #region Fields

    private readonly ICurrencyService _currencyService;
    private readonly IDiscountService _discountService;
    private readonly IOrderTotalCalculationService _orderTotalCalculationService;
    private readonly IPriceFormatter _priceFormatter;
    private readonly IProductService _productService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IStoreContext _storeContext;
    private readonly IWorkContext _workContext;
    private readonly NopwaraBoostSettings _settings;
    private readonly ShippingSettings _shippingSettings;

    #endregion

    #region Ctor

    public NopwaraBoostService(ICurrencyService currencyService,
        IDiscountService discountService,
        IOrderTotalCalculationService orderTotalCalculationService,
        IPriceFormatter priceFormatter,
        IProductService productService,
        IShoppingCartService shoppingCartService,
        IStoreContext storeContext,
        IWorkContext workContext,
        NopwaraBoostSettings settings,
        ShippingSettings shippingSettings)
    {
        _currencyService = currencyService;
        _discountService = discountService;
        _orderTotalCalculationService = orderTotalCalculationService;
        _priceFormatter = priceFormatter;
        _productService = productService;
        _shoppingCartService = shoppingCartService;
        _storeContext = storeContext;
        _workContext = workContext;
        _settings = settings;
        _shippingSettings = shippingSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepares the free-shipping progress of the current cart; null when "free shipping over X" doesn't apply
    /// </summary>
    public async Task<FreeShippingModel> PrepareFreeShippingAsync(bool compact)
    {
        if (!_settings.FreeShippingBarEnabled || !_shippingSettings.FreeShippingOverXEnabled || _shippingSettings.FreeShippingOverXValue <= decimal.Zero)
            return null;

        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
        if (!cart.Any() || !await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
            return null;

        //same subtotal the core free-shipping check uses, in the primary store currency
        var (_, _, _, subTotal, _) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart, _shippingSettings.FreeShippingOverXIncludingTax);
        var threshold = _shippingSettings.FreeShippingOverXValue;

        //core decides: it also gives free shipping to customer roles with free shipping and to carts that only hold free-shipping items
        var reached = await _orderTotalCalculationService.IsFreeShippingAsync(cart, subTotal);

        //core needs a subtotal strictly over the threshold: at exactly the threshold one more cent is still needed
        var remaining = Math.Max(threshold - subTotal, 0.01m);
        var remainingInWorkingCurrency = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(remaining, await _workContext.GetWorkingCurrencyAsync());

        return new FreeShippingModel
        {
            Reached = reached,
            Remaining = await _priceFormatter.FormatPriceAsync(remainingInWorkingCurrency),
            Percent = reached ? 100 : (int)Math.Clamp(Math.Floor(subTotal / threshold * 100), 0, 99),
            Compact = compact
        };
    }

    /// <summary>
    /// Gets the stock quantity to show in the low-stock badge; null when the badge doesn't apply
    /// </summary>
    public async Task<int?> GetLowStockQuantityAsync(int productId)
    {
        if (!_settings.LowStockEnabled || _settings.LowStockThreshold <= 0)
            return null;

        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null || product.ManageInventoryMethod != ManageInventoryMethod.ManageStock)
            return null;

        var quantity = await _productService.GetTotalStockQuantityAsync(product);

        return quantity > 0 && quantity <= _settings.LowStockThreshold ? quantity : null;
    }

    /// <summary>
    /// Gets the end of the sale on the product: the earliest end date among the running discounts assigned to it
    /// (no coupon code) that ends within the configured number of days; null when there is none
    /// </summary>
    public async Task<DateTime?> GetSaleEndUtcAsync(int productId)
    {
        if (!_settings.CountdownEnabled)
            return null;

        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null)
            return null;

        var now = DateTime.UtcNow;
        var limit = now.AddDays(Math.Max(1, _settings.CountdownMaxDays));

        var candidates = (await _discountService.GetAppliedDiscountsAsync(product))
            .Where(d => !d.RequiresCouponCode && d.IsActive)
            .Where(d => !d.StartDateUtc.HasValue || d.StartDateUtc <= now)
            .Where(d => d.EndDateUtc.HasValue && d.EndDateUtc > now && d.EndDateUtc <= limit);

        //only discounts this customer actually gets (requirements such as customer roles, usage limitations)
        var customer = await _workContext.GetCurrentCustomerAsync();
        DateTime? end = null;
        foreach (var discount in candidates)
        {
            if ((await _discountService.ValidateDiscountAsync(discount, customer, Array.Empty<string>())).IsValid && (end == null || discount.EndDateUtc < end))
                end = discount.EndDateUtc;
        }

        return end;
    }

    #endregion
}
