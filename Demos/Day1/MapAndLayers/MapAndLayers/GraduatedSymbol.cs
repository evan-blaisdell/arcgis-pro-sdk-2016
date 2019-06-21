using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Core.CIM;

namespace MapAndLayers
{
  internal class GraduatedSymbol : Button
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
        symbol.SetSize(12.0);
        symbol.SetColor(ColorFactory.CreateRGBColor(24, 69, 59));

        var renderer = new GraduatedSymbolsRendererDefinition()
        {
          BreakCount = 5,
          ClassificationField = "height",
          ClassificationMethod = ClassificationMethod.EqualInterval,
          SymbolTemplate = symbol.MakeSymbolReference(),
          MinimumSymbolSize = 6.0,
          MaximumSymbolSize = 20.0
        };

        //Update the feature layer renderer
        layer.SetRenderer(layer.CreateRenderer(renderer));
      });
    }
  }
}
