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

namespace MapAndLayers
{
  internal class UniqueValues : Button
  {
    protected override async void OnClick()
    {
      //Get all styles in the project
      var styles = Project.Current.GetItems<StyleProjectItem>();

      //Get a specific style in the project
      StyleProjectItem style = styles.First(x => x.Name == "CustomStyle");
      var pointSymbols = await style.SearchSymbolsAsync(StyleItemType.PointSymbol, "Tree");
      var treeSymbolItem = pointSymbols.FirstOrDefault();
      if (treeSymbolItem == null)
        return;

      var colorRamps = await style.SearchColorRampsAsync("UniqueValues");
      var colorRampItem = colorRamps.FirstOrDefault();
      if (colorRampItem == null)
        return;

      var layer = MapView.Active.Map.GetLayersAsFlattenedList().FirstOrDefault(l => l.Name == "Trees") as FeatureLayer;

      Task t = QueuedTask.Run(() =>
      {
        var symbol = treeSymbolItem.Symbol;
        symbol.SetSize(12.0);

        var renderer = new UniqueValueRendererDefinition()
        {
          UseDefaultSymbol = false,
          ValueFields = new string[] { "type" },
          SymbolTemplate = symbol.MakeSymbolReference(),
          ColorRamp = colorRampItem.ColorRamp
        };

        //Update the feature layer renderer
        layer.SetRenderer(layer.CreateRenderer(renderer));
      });
    }
  }
}
