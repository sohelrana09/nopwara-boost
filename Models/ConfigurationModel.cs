using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.NopwaraBoost.Models;

/// <summary>
/// Represents the plugin configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
    [NopResourceDisplayName("Plugins.Widgets.NopwaraBoost.Fields.AllowedThemes")]
    public string AllowedThemes { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraBoost.Fields.FreeShippingBarEnabled")]
    public bool FreeShippingBarEnabled { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraBoost.Fields.StickyAddToCartEnabled")]
    public bool StickyAddToCartEnabled { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraBoost.Fields.LowStockEnabled")]
    public bool LowStockEnabled { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraBoost.Fields.LowStockThreshold")]
    public int LowStockThreshold { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraBoost.Fields.LowStockOnProductBoxes")]
    public bool LowStockOnProductBoxes { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraBoost.Fields.CountdownEnabled")]
    public bool CountdownEnabled { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraBoost.Fields.CountdownMaxDays")]
    public int CountdownMaxDays { get; set; }
}
