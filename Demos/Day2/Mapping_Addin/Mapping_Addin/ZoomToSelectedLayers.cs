using System;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;

namespace Mapping_Addin
{
    internal class ZoomToSelectedLayers : Button
  {
    private bool maintainViewDirection = false;

    protected override void OnClick()
    {
      //Get the active map view
      var mapView = MapView.Active;
      if (mapView == null)
        return;

      //Get the selected layers in the TOC
      var layers = mapView.GetSelectedLayers();

      //Zoom to the selected features for the layers
      mapView.ZoomToAsync(layers, true, TimeSpan.FromSeconds(2), maintainViewDirection);
    }
  }
}
