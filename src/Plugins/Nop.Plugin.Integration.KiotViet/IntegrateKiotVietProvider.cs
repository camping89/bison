using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Plugins;
using Nop.Services.Localization;

namespace Nop.Plugin.Integration.KiotViet
{
    public class IntegrateKiotVietProvider : BasePlugin
    {
        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //locales
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Integration.KiotViet.Error", "You can use Nop.Plugin.Integration.KiotViet");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Nop.Plugin.Integration.KiotViet.Error");

            base.Uninstall();
        }

    }
}
