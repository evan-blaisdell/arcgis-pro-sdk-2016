using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;

namespace ProAppModule1
{
    internal class Button1 : Button
    {
        private int clickCounts = 0;
        protected override void OnClick()
        {
            clickCounts++;
            this.Tooltip = clickCounts.ToString();
        }
    }
}
