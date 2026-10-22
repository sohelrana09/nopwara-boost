using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.NopwaraBoost;

/// <summary>
/// Represents the plugin settings
/// </summary>
public class NopwaraBoostSettings : ISettings
{
    /// <summary>
    /// Gets or sets comma-separated theme system names the widgets are rendered for (their markup is styled by the theme)
    /// </summary>
    public string AllowedThemes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show the free-shipping progress bar (cart page and flyout cart);
    /// it uses the core "Free shipping over X" shipping setting
    /// </summary>
    public bool FreeShippingBarEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show the sticky add-to-cart bar on product pages
    /// </summary>
    public bool StickyAddToCartEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show the low-stock badge
    /// </summary>
    public bool LowStockEnabled { get; set; }

    /// <summary>
    /// Gets or sets the stock quantity at or below which the low-stock badge is shown
    /// </summary>
    public int LowStockThreshold { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show the low-stock badge in product boxes too (not only on product pages)
    /// </summary>
    public bool LowStockOnProductBoxes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show the sale countdown on product pages
    /// </summary>
    public bool CountdownEnabled { get; set; }

    /// <summary>
    /// Gets or sets the number of days before a discount ends from which the countdown is shown
    /// </summary>
    public int CountdownMaxDays { get; set; }
}
