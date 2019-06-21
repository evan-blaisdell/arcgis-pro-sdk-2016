using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Core;
using ArcGIS.Core.CIM;

namespace MapAndLayers
{
  internal class HeatMap : Button
  {
    protected override async void OnClick()
    {
      //Get all styles in the project
      var styles = Project.Current.GetItems<StyleProjectItem>();

      //Get a specific style in the project
      StyleProjectItem style = styles.First(x => x.Name == "CustomStyle");
      var colorRamps = await style.SearchColorRampsAsync("HeatMap");
      var colorRampItem = colorRamps.FirstOrDefault();
      if (colorRampItem == null)
        return;

      var layer = MapView.Active.Map.GetLayersAsFlattenedList().FirstOrDefault(l => l.Name == "Trees") as FeatureLayer;

      Task t = QueuedTask.Run(() =>
      {
        var renderer = new CIMHeatMapRenderer()
        {
          ColorScheme = colorRampItem.ColorRamp,
          Field = "height",
          Radius = 25,
          RendererQuality = 5,
          Heading = "Height",
          MinLabel = "Sparse",
          MaxLabel = "Dense"
        };

        //Update the feature layer renderer
        layer.SetRenderer(renderer);
      });
    }
  }
}
