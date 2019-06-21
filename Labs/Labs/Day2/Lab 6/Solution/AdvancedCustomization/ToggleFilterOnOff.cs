using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;

namespace AdvancedCustomization {
    internal class ToggleFilterOnOff : Button
    {

        private ExampleCustomizationFilter _filter = null;
        protected override void OnClick()
        {
            if (_filter == null)
                _filter = new ExampleCustomizationFilter();
            _filter.FilterOn = !_filter.FilterOn;
            this.Caption = _filter.FilterOn ? "Turn Filter Off" : "Turn Filter On";
            this.Tooltip = _filter.FilterOn ? "Turn the filter off" : "Turn the filter on";
        }
    }
}
