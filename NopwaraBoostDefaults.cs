namespace Nop.Plugin.Widgets.NopwaraBoost;

/// <summary>
/// Represents plugin constants
/// </summary>
public static class NopwaraBoostDefaults
{
    /// <summary>
    /// Gets the plugin system name
    /// </summary>
    public const string SYSTEM_NAME = "Widgets.NopwaraBoost";

    /// <summary>
    /// Gets the folder (under Nop.Web/Plugins) the plugin is deployed to
    /// </summary>
    public const string PLUGIN_FOLDER = "Widgets.NopwaraBoost";

    /// <summary>
    /// Gets the folder name themes use to override the plugin views (Themes/{theme}/Views/Plugins/{folder})
    /// </summary>
    public const string THEME_VIEWS_FOLDER = "NopwaraBoost";

    /// <summary>
    /// Gets the widget zone the Nopwara flyout cart renders above its items
    /// </summary>
    public const string FLYOUT_CART_ZONE = "nopwara_flyoutcart_top";

    /// <summary>
    /// Gets the name of the admin configuration route
    /// </summary>
    public const string CONFIGURATION_ROUTE_NAME = "Plugin.Widgets.NopwaraBoost.Configure";
}
