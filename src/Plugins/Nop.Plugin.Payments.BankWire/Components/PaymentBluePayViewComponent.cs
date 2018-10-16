using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.BankWire.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.BankWire.Components
{
    [ViewComponent(Name = "PaymentBankWire")]
    public class PaymentBankWireViewComponent : NopViewComponent
    {
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        public PaymentBankWireViewComponent(IWorkContext workContext,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            this._workContext = workContext;
            this._settingService = settingService;
            this._storeContext = storeContext;
        }

        public IViewComponentResult Invoke()
        {
            var bankWirePaymentSettings = _settingService.LoadSetting<BankWirePaymentSettings>(_storeContext.CurrentStore.Id);

            var model = new PaymentInfoModel
            {
                DescriptionText = bankWirePaymentSettings.GetLocalizedSetting(x => x.DescriptionText, _workContext.WorkingLanguage.Id, 0)
            };

            return View("~/Plugins/Payments.BankWire/Views/PaymentInfo.cshtml", model);
        }
    }
}
