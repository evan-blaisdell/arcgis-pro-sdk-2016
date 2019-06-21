using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using System.Threading.Tasks;

namespace ConditionsAndState {
    internal class Module1 : Module {
        private static Module1 _this = null;
        private static readonly string MyStateID = "example_state";

        /// <summary>
        /// Retrieve the singleton instance to this module here
        /// </summary>
        public static Module1 Current {
            get {
                return _this ?? (_this = (Module1)FrameworkApplication.FindModule("Lab2_start_ConditionsAndState"));
            }
        }

        //TODO - add an internal static method to act as your button OnClick handler
        //it will return void. It should activate and deactivate your custom state
        //internal static void XXXXXXX() { .....

        
        internal static void ToggleState()
        {
            if (FrameworkApplication.State.Contains(MyStateID))
            {
                FrameworkApplication.State.Deactivate(MyStateID);
            }
            else
            {
                FrameworkApplication.State.Activate(MyStateID);
            }
        }

        #region Overrides
        /// <summary>
        /// Called by Framework when ArcGIS Pro is closing
        /// </summary>
        /// <returns>False to prevent Pro from closing, otherwise True</returns>
        protected override bool CanUnload() {
            //TODO - add your business logic
            //return false to ~cancel~ Application close
            return true;
        }

        #endregion Overrides

    }
}
