using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Payments.BankWire.Models
{
    public class PaymentInfoModel : BaseNopModel
    {
        public string DescriptionText { get; set; }
    }
}