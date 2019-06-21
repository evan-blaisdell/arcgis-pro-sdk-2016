using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Framework.Dialogs;

namespace Mapping_Addin
{
  internal class ShowSelectedLayers : Button
  {
    protected override void OnClick()
    {
      var layers = MapView.Active.GetSelectedLayers();
      if (layers.Count() == 0)
        MessageBox.Show("No selected layers", "Selected Layers");
      else
        MapView.Active.ZoomToAsync(layers);
    }
  }
}
