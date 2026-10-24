using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.NopwaraBoost.Models;
using Nop.Plugin.Widgets.NopwaraBoost.Services;
using Nop.Services.Localization;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Widgets.NopwaraBoost.Components;

/// <summary>
/// Renders the conversion widgets; which one depends on the widget zone
/// </summary>
public class NopwaraBoostViewComponent : NopViewComponent
{
    #region Fields

    private readonly INopFileProvider _fileProvider;
    private readonly ILocalizationService _localizationService;
    private readonly IThemeContext _themeContext;
    private readonly NopwaraBoostService _boostService;
    private readonly NopwaraBoostSettings _settings;

    #endregion

    #region Ctor

    public NopwaraBoostViewComponent(INopFileProvider fileProvider,
        ILocalizationService localizationService,
        IThemeContext themeContext,
        NopwaraBoostService boostService,
        NopwaraBoostSettings settings)
    {
        _fileProvider = fileProvider;
        _localizationService = localizationService;
        _themeContext = themeContext;
        _boostService = boostService;
        _settings = settings;
    }

    #endregion

    #region Utilities

    private IViewComponentResult Render(string theme, string name, object model)
    {
        //themes can override the views at Themes/{theme}/Views/Plugins/NopwaraBoost/{name}.cshtml
        var themeView = $"~/Themes/{theme}/Views/Plugins/{NopwaraBoostDefaults.THEME_VIEWS_FOLDER}/{name}.cshtml";
        var view = _fileProvider.FileExists(_fileProvider.MapPath(themeView))
            ? themeView
            : $"~/Plugins/{NopwaraBoostDefaults.PLUGIN_FOLDER}/Views/{name}.cshtml";

        return View(view, model);
    }

    #endregion

    #region Methods

    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        //the markup is styled by the theme, so only render it for the allowed themes
        var theme = await _themeContext.GetWorkingThemeNameAsync();
        var allowed = (_settings.AllowedThemes ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Contains(theme, StringComparer.OrdinalIgnoreCase);
        if (!allowed)
            return Content(string.Empty);

        //free-shipping progress: cart page (order summary) and flyout cart
        if (widgetZone == PublicWidgetZones.OrderSummaryContentBefore || widgetZone == NopwaraBoostDefaults.FLYOUT_CART_ZONE)
        {
            var model = await _boostService.PrepareFreeShippingAsync(compact: widgetZone == NopwaraBoostDefaults.FLYOUT_CART_ZONE);
            return model == null ? Content(string.Empty) : Render(theme, "FreeShipping", model);
        }

        //low-stock badge: product page and (optionally) product boxes
        if (widgetZone == PublicWidgetZones.ProductDetailsOverviewTop && additionalData is ProductDetailsModel details)
        {
            var quantity = await _boostService.GetLowStockQuantityAsync(details.Id);
            return quantity == null ? Content(string.Empty) : Render(theme, "LowStock", new LowStockModel { Quantity = quantity.Value });
        }

        if (widgetZone == PublicWidgetZones.ProductBoxAddinfoMiddle && additionalData is ProductOverviewModel overview)
        {
            if (!_settings.LowStockOnProductBoxes)
                return Content(string.Empty);

            var quantity = await _boostService.GetLowStockQuantityAsync(overview.Id);
            return quantity == null ? Content(string.Empty) : Render(theme, "LowStock", new LowStockModel { Quantity = quantity.Value, Compact = true });
        }

        //sale countdown under the product price
        if (widgetZone == PublicWidgetZones.ProductPriceBottom && additionalData is ProductDetailsModel priced)
        {
            var end = await _boostService.GetSaleEndUtcAsync(priced.Id);
            return end == null ? Content(string.Empty) : Render(theme, "Countdown", new CountdownModel { EndsOnUtc = DateTime.SpecifyKind(end.Value, DateTimeKind.Utc).ToString("o") });
        }

        //sticky add-to-cart bar
        if (widgetZone == PublicWidgetZones.ProductDetailsBottom && additionalData is ProductDetailsModel product)
        {
            if (!_settings.StickyAddToCartEnabled || product.ProductType == ProductType.GroupedProduct || product.AddToCart.DisableBuyButton)
                return Content(string.Empty);

            var buttonText = product.AddToCart.IsRental
                ? await _localizationService.GetResourceAsync("ShoppingCart.Rent")
                : product.AddToCart.AvailableForPreOrder
                    ? await _localizationService.GetResourceAsync("ShoppingCart.PreOrder")
                    : await _localizationService.GetResourceAsync("ShoppingCart.AddToCart");

            return Render(theme, "StickyCart", new StickyCartModel
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = string.IsNullOrEmpty(product.ProductPrice.PriceWithDiscount) ? product.ProductPrice.Price : product.ProductPrice.PriceWithDiscount,
                PictureUrl = product.DefaultPictureModel?.ImageUrl,
                ButtonText = buttonText
            });
        }

        return Content(string.Empty);
    }

    #endregion
}
