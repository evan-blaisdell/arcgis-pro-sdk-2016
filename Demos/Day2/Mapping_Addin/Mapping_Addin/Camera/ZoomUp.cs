using System;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Core.CIM;

namespace Mapping_Addin
{
    internal class ZoomUp : Button
  {
    protected override void OnClick()
    {
      //Get the active map view
      var mapView = MapView.Active;
      if (mapView == null)
        return;

      //Get the camera for the view
      var camera = mapView.Camera;

      //Test if the view is 2D and adjust the Scale, otherwise adjust the Z
      if (mapView.ViewingMode == MapViewingMode.Map)
      {
        camera.Scale *= 1.25;
      }
      else
      {
        camera.Z *= 1.25;
      }

      //Zoom to the new camera
      mapView.ZoomToAsync(camera, TimeSpan.FromSeconds(.5));
    }
  }
}
