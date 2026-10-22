using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widgets.NopwaraBoost.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.NopwaraBoost.Controllers;

[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
[AutoValidateAntiforgeryToken]
public class NopwaraBoostAdminController : BasePluginController
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly ISettingService _settingService;
    private readonly NopwaraBoostSettings _settings;

    #endregion

    #region Ctor

    public NopwaraBoostAdminController(ILocalizationService localizationService,
        INotificationService notificationService,
        ISettingService settingService,
        NopwaraBoostSettings settings)
    {
        _localizationService = localizationService;
        _notificationService = notificationService;
        _settingService = settingService;
        _settings = settings;
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public IActionResult Configure()
    {
        var model = new ConfigurationModel
        {
            AllowedThemes = _settings.AllowedThemes,
            FreeShippingBarEnabled = _settings.FreeShippingBarEnabled,
            StickyAddToCartEnabled = _settings.StickyAddToCartEnabled,
            LowStockEnabled = _settings.LowStockEnabled,
            LowStockThreshold = _settings.LowStockThreshold,
            LowStockOnProductBoxes = _settings.LowStockOnProductBoxes,
            CountdownEnabled = _settings.CountdownEnabled,
            CountdownMaxDays = _settings.CountdownMaxDays
        };

        return View($"~/Plugins/{NopwaraBoostDefaults.PLUGIN_FOLDER}/Views/Admin/Configure.cshtml", model);
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return Configure();

        _settings.AllowedThemes = model.AllowedThemes?.Trim();
        _settings.FreeShippingBarEnabled = model.FreeShippingBarEnabled;
        _settings.StickyAddToCartEnabled = model.StickyAddToCartEnabled;
        _settings.LowStockEnabled = model.LowStockEnabled;
        _settings.LowStockThreshold = Math.Max(0, model.LowStockThreshold);
        _settings.LowStockOnProductBoxes = model.LowStockOnProductBoxes;
        _settings.CountdownEnabled = model.CountdownEnabled;
        _settings.CountdownMaxDays = Math.Max(0, model.CountdownMaxDays);
        await _settingService.SaveSettingAsync(_settings);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return Configure();
    }

    #endregion
}
