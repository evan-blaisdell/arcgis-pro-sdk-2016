using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Core;

namespace MapAndLayers
{
  internal class CreateMap : Button
  {
    protected override async void OnClick()
    {
      CreateMapDialog mapDialog = new CreateMapDialog();
      if (mapDialog.ShowDialog() ?? false)
      {
        var mapName = mapDialog.MapName;
        var map = await QueuedTask.Run(() => MapFactory.CreateMap(mapName, 
          ArcGIS.Core.CIM.MapType.Map, ArcGIS.Core.CIM.MapViewingMode.Map, Basemap.Topographic));
        Task t = ProApp.Panes.CreateMapPaneAsync(map);
      }
    }
  }
}
