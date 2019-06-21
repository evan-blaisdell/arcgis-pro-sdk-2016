using System;
using System.Linq;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Framework.Dialogs;

namespace Mapping_Addin
{
    internal class ShowCount : Button
  {
    protected override void OnClick()
    {
      //Get the active map view
      var mapView = MapView.Active;
      if (mapView == null)
        return;

      //Get the feature layers from the map.
      var featureLayers = mapView.Map.GetLayersAsFlattenedList().OfType<FeatureLayer>();
      
      //Show an appropriate message box for the feature layers and number of selected features.
      if (featureLayers.Count() == 0)
        MessageBox.Show("No feature layers", "Feature Layers");
      else
        MessageBox.Show(String.Join("\n", featureLayers.Select(l => string.Format("{0}: {1}", l.Name, l.SelectionCount))), "Feature Layers");
    }
  }
}
