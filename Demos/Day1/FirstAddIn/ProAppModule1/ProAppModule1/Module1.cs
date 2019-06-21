using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using System.Threading.Tasks;

namespace ProAppModule1
{
    internal class Module1 : Module
    {
        private static Module1 _this = null;

        /// <summary>
        /// Retrieve the singleton instance to this module here
        /// </summary>
        public static Module1 Current
        {
            get
            {
                return _this ?? (_this = (Module1)FrameworkApplication.FindModule("ProAppModule1_Module"));
            }
        }

        #region Overrides
        /// <summary>
        /// Called by Framework when ArcGIS Pro is closing
        /// </summary>
        /// <returns>False to prevent Pro from closing, otherwise True</returns>
        protected override bool CanUnload()
        {
            //TODO - add your business logic
            //return false to ~cancel~ Application close
            return true;
        }

        #endregion Overrides
        internal static void OnCustomButtonClick()
        {
            IPlugInWrapper wrapper = FrameworkApplication.GetPlugInWrapper("ProAppModule1_Button1");
            wrapper.Caption = "New Caption";
            wrapper.Tooltip = "New Tooltip";
            //TODO - something on click...
        }

        //internal static bool CanOnCustomButtonClick
        //{
        //    get { return true; }
        //}
    }
}
