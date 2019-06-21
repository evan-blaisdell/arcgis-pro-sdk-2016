using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;

namespace AdvancedCustomization {
    internal class ExampleCustomizationFilter : CustomizationFilter
    {

        private bool _filterOn = false;
        public ExampleCustomizationFilter()
        {
            //TODO - register the Customization Filter
            FrameworkApplication.RegisterCustomizationFilter(this);
        }

        public bool FilterOn
        {
            get { return _filterOn; }
            set { _filterOn = value; }
        }

        protected override bool OnCommandToExecute(string ID)
        {
            if (!FilterOn)
                return true;//don't filter
            //TODO return false for editing commands
            // return false;
            return !ID.StartsWith("esri_editing_");
        }
    }
}
