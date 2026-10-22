# Nopwara Conversion Widgets for nopCommerce

Four small conversion widgets for nopCommerce, built with **Tailwind CSS** and **Alpine.js** for the Nopwara theme. Each one can be turned on or off separately.

| Widget | Where | Data |
|---|---|---|
| **Free-shipping progress bar**: "Spend $X more to get free shipping" / "You've got free shipping!" | Cart page (`order_summary_content_before`) and the flyout cart (`nopwara_flyoutcart_top`) | Core *Free shipping over X* (Shipping settings), compared the way core does it (`IsFreeShippingAsync`) |
| **Sticky add-to-cart bar** | Product page (`productdetails_bottom`) | Appears once the page's Add to cart button scrolls out of view, then clicks that button, so attributes, quantity and validation stay the page's own. The price follows attribute changes. |
| **Low-stock badge**: "Only 5 left in stock" | Product page (`productdetails_overview_top`) and product boxes (`productbox_addinfo_middle`) | Products that use *Manage stock*, with `GetTotalStockQuantityAsync` at or below the threshold |
| **Sale countdown**: "Sale ends in 2d 17:58:09" | Under the price (`productdetails_price_bottom`) | The earliest end date among active discounts assigned to the product, without a coupon, that end within N days |

## Requirements

- nopCommerce **5.00**
- The Nopwara theme. Other themes can render the two custom zones:

```razor
@await Component.InvokeAsync(typeof(WidgetViewComponent), new { widgetZone = "nopwara_flyoutcart_top", additionalData = Model })
```

## Install

1. Copy this folder to `src/Plugins/`, add the project to `NopCommerce.sln` and build.
2. Install **Nopwara conversion widgets** (Configuration → Local plugins) and restart. The widget is activated on install.
3. Add this line to the theme's Tailwind entry CSS, then rebuild the CSS:
   `@source "../../../../../Plugins/Nop.Plugin.Widgets.NopwaraBoost/Views";`

## Settings

- Themes (default `Nopwara`)
- Free-shipping bar
- Sticky add-to-cart
- Low-stock badge, threshold (10) and product boxes on/off
- Sale countdown, and how many days before the end it starts showing (7)

## Overriding the markup

Copy any of `Views/FreeShipping.cshtml`, `StickyCart.cshtml`, `LowStock.cshtml` or `Countdown.cshtml` to `Themes/{YourTheme}/Views/Plugins/NopwaraBoost/`.

## License

Copyright (C) 2026 Sohel Rana

Licensed under the [GNU Affero General Public License v3.0](LICENSE), the license nopCommerce itself is built on (the [nopCommerce Public License](https://www.nopcommerce.com/license) is AGPL v3 plus its additional terms). Parts of this project are adapted from nopCommerce source code. A store that uses it must keep nopCommerce's "Powered by nopCommerce" link, as the nopCommerce Public License requires.
