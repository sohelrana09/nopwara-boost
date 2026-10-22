using Nop.Core.Domain.Cms;
using Nop.Plugin.Widgets.NopwaraBoost.Components;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Widgets.NopwaraBoost;

/// <summary>
/// Represents the Nopwara conversion widgets plugin
/// </summary>
public class NopwaraBoostPlugin : BasePlugin, IWidgetPlugin
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly ISettingService _settingService;
    private readonly WidgetSettings _widgetSettings;

    #endregion

    #region Ctor

    public NopwaraBoostPlugin(ILocalizationService localizationService,
        INopUrlHelper nopUrlHelper,
        ISettingService settingService,
        WidgetSettings widgetSettings)
    {
        _localizationService = localizationService;
        _nopUrlHelper = nopUrlHelper;
        _settingService = settingService;
        _widgetSettings = widgetSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets widget zones where this widget should be rendered
    /// </summary>
    public Task<IList<string>> GetWidgetZonesAsync()
    {
        return Task.FromResult<IList<string>>(new List<string>
        {
            //free-shipping progress: cart page and the Nopwara flyout cart
            PublicWidgetZones.OrderSummaryContentBefore,
            NopwaraBoostDefaults.FLYOUT_CART_ZONE,
            //low-stock badge: product page and product boxes
            PublicWidgetZones.ProductDetailsOverviewTop,
            PublicWidgetZones.ProductBoxAddinfoMiddle,
            //sale countdown
            PublicWidgetZones.ProductPriceBottom,
            //sticky add-to-cart bar
            PublicWidgetZones.ProductDetailsBottom
        });
    }

    /// <summary>
    /// Gets a type of a view component for displaying widget
    /// </summary>
    public Type GetWidgetViewComponent(string widgetZone)
    {
        return typeof(NopwaraBoostViewComponent);
    }

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return _nopUrlHelper.RouteUrl(NopwaraBoostDefaults.CONFIGURATION_ROUTE_NAME);
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    public override async Task InstallAsync()
    {
        await _settingService.SaveSettingAsync(new NopwaraBoostSettings
        {
            AllowedThemes = "Nopwara",
            FreeShippingBarEnabled = true,
            StickyAddToCartEnabled = true,
            LowStockEnabled = true,
            LowStockThreshold = 10,
            LowStockOnProductBoxes = true,
            CountdownEnabled = true,
            CountdownMaxDays = 7
        });

        if (!_widgetSettings.ActiveWidgetSystemNames.Contains(NopwaraBoostDefaults.SYSTEM_NAME))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(NopwaraBoostDefaults.SYSTEM_NAME);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            //admin
            ["Plugins.Widgets.NopwaraBoost.Instructions"] = "Conversion widgets for the Nopwara theme. The free-shipping bar uses the \"Free shipping over X\" option of Configuration > Settings > Shipping settings and is only shown when it is enabled. The sale countdown uses the end date of discounts assigned to products (without coupon code). Activate or deactivate the widgets in Configuration > Widgets.",
            ["Plugins.Widgets.NopwaraBoost.Fields.AllowedThemes"] = "Themes",
            ["Plugins.Widgets.NopwaraBoost.Fields.AllowedThemes.Hint"] = "Comma-separated theme system names the widgets are used with (their markup is styled by the theme).",
            ["Plugins.Widgets.NopwaraBoost.Fields.FreeShippingBarEnabled"] = "Free-shipping progress bar",
            ["Plugins.Widgets.NopwaraBoost.Fields.FreeShippingBarEnabled.Hint"] = "Show how much more the customer needs to spend for free shipping (cart page and flyout cart).",
            ["Plugins.Widgets.NopwaraBoost.Fields.StickyAddToCartEnabled"] = "Sticky add-to-cart bar",
            ["Plugins.Widgets.NopwaraBoost.Fields.StickyAddToCartEnabled.Hint"] = "Show a bar with the price and an add-to-cart button when the product page button scrolls out of view.",
            ["Plugins.Widgets.NopwaraBoost.Fields.LowStockEnabled"] = "Low-stock badge",
            ["Plugins.Widgets.NopwaraBoost.Fields.LowStockEnabled.Hint"] = "Show \"Only N left\" for products with stock management when the stock is low.",
            ["Plugins.Widgets.NopwaraBoost.Fields.LowStockThreshold"] = "Low-stock threshold",
            ["Plugins.Widgets.NopwaraBoost.Fields.LowStockThreshold.Hint"] = "The badge is shown when the stock quantity is at or below this number.",
            ["Plugins.Widgets.NopwaraBoost.Fields.LowStockOnProductBoxes"] = "Low-stock badge in product boxes",
            ["Plugins.Widgets.NopwaraBoost.Fields.LowStockOnProductBoxes.Hint"] = "Show the badge in catalog product boxes too.",
            ["Plugins.Widgets.NopwaraBoost.Fields.CountdownEnabled"] = "Sale countdown",
            ["Plugins.Widgets.NopwaraBoost.Fields.CountdownEnabled.Hint"] = "Show a countdown under the price when a discount on the product ends soon.",
            ["Plugins.Widgets.NopwaraBoost.Fields.CountdownMaxDays"] = "Countdown from (days)",
            ["Plugins.Widgets.NopwaraBoost.Fields.CountdownMaxDays.Hint"] = "Show the countdown when the discount ends within this number of days.",

            //storefront
            ["Plugins.Widgets.NopwaraBoost.FreeShipping.Remaining"] = "Spend {0} more to get free shipping",
            ["Plugins.Widgets.NopwaraBoost.FreeShipping.Reached"] = "You've got free shipping!",
            ["Plugins.Widgets.NopwaraBoost.LowStock"] = "Only {0} left in stock",
            ["Plugins.Widgets.NopwaraBoost.LowStock.Short"] = "Only {0} left",
            ["Plugins.Widgets.NopwaraBoost.Countdown"] = "Sale ends in",
            ["Plugins.Widgets.NopwaraBoost.Countdown.Days"] = "d"
        });

        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    public override async Task UninstallAsync()
    {
        if (_widgetSettings.ActiveWidgetSystemNames.Contains(NopwaraBoostDefaults.SYSTEM_NAME))
        {
            _widgetSettings.ActiveWidgetSystemNames.Remove(NopwaraBoostDefaults.SYSTEM_NAME);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        await _settingService.DeleteSettingAsync<NopwaraBoostSettings>();
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Widgets.NopwaraQuickView");

        await base.UninstallAsync();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
    /// </summary>
    public bool HideInWidgetList => false;

    #endregion
}
