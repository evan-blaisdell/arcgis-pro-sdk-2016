using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Mapping;

namespace Mapping_Addin
{
  class CustomSelection : MapTool
  {
    public CustomSelection()
    {
      IsSketchTool = true;
      SketchType = SketchGeometryType.Radial;
      SketchOutputMode = SketchOutputMode.Screen;
      OverlayControlID = "Mapping_Addin_EmbeddableControl";
    }

    protected override Task<bool> OnSketchCompleteAsync(Geometry geometry)
    {
      return ArcGIS.Desktop.Framework.Threading.Tasks.QueuedTask.Run(() =>
      {
        //Get the active map view
        var mapView = MapView.Active;
        if (mapView == null)
          return true;

        //Get the features that intersect the sketch geometry
        var result = mapView.SelectFeatures(geometry);

        //Get the instance of the overlay control view model
        var vm = OverlayEmbeddableControl as EmbeddedControlViewModel;
        if (vm == null)
          return true;

        //Update the results in the view model
        vm.UpdateResults(result);
        return true;
      });
    }
  }
}
