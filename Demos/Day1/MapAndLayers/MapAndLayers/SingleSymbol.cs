using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Core.CIM;

namespace MapAndLayers
{
  internal class SingleSymbol : Button
  {
    protected override async void OnClick()
    {
      //Get all styles in the project
      var styles = Project.Current.GetItems<StyleProjectItem>();

      //Get a specific style in the project
      StyleProjectItem style = styles.First(x => x.Name == "CustomStyle");
      var results = await style.SearchSymbolsAsync(StyleItemType.PointSymbol, "Tree");
      var treeSymbolItem = results.FirstOrDefault();
      if (treeSymbolItem == null)
        return;

      var layer = MapView.Active.Map.GetLayersAsFlattenedList().FirstOrDefault(l => l.Name == "Trees") as FeatureLayer;
      if (layer == null)
        return;

      Task t = QueuedTask.Run(() =>
      {
        var symbol = treeSymbolItem.Symbol as CIMPointSymbol;
        symbol.SymbolLayers = new CIMSymbolLayer[] { symbol.SymbolLayers[0] };
        symbol.SetSize(12.0);
        symbol.SetColor(ColorFactory.CreateRGBColor(24, 69, 59));

        var renderer = new SimpleRendererDefinition(symbol.MakeSymbolReference());

        //Update the feature layer renderer
        layer.SetRenderer(layer.CreateRenderer(renderer));
      });
    }
  }
}
