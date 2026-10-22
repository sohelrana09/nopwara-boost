using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.NopwaraBoost.Models;

/// <summary>
/// Represents the free-shipping progress bar
/// </summary>
public record FreeShippingModel : BaseNopModel
{
    /// <summary>
    /// Gets or sets a value indicating whether the cart already ships for free
    /// </summary>
    public bool Reached { get; set; }

    /// <summary>
    /// Gets or sets the formatted amount still needed for free shipping
    /// </summary>
    public string Remaining { get; set; }

    /// <summary>
    /// Gets or sets the progress in percent (0-100)
    /// </summary>
    public int Percent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the bar is rendered in the flyout cart (compact layout)
    /// </summary>
    public bool Compact { get; set; }
}

/// <summary>
/// Represents the sticky add-to-cart bar
/// </summary>
public record StickyCartModel : BaseNopModel
{
    public int ProductId { get; set; }

    public string Name { get; set; }

    public string Price { get; set; }

    public string PictureUrl { get; set; }

    public string ButtonText { get; set; }
}

/// <summary>
/// Represents the low-stock badge
/// </summary>
public record LowStockModel : BaseNopModel
{
    public int Quantity { get; set; }

    public bool Compact { get; set; }
}

/// <summary>
/// Represents the sale countdown
/// </summary>
public record CountdownModel : BaseNopModel
{
    /// <summary>
    /// Gets or sets the sale end (UTC, ISO 8601)
    /// </summary>
    public string EndsOnUtc { get; set; }
}
