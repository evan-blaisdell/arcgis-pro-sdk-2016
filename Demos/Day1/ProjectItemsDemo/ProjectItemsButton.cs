using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;

namespace ProjectItemsDemo {
    internal class ProjectItemsButton : Button {
        protected override void OnClick()
        {
            //This code shows how to activate a specific Map View
            var mapProjectItems = Project.Current.GetItems<MapProjectItem>();
            var mapItem = mapProjectItems.FirstOrDefault((mi) => {
                return mi.Name == "Map";
            });
            QueuedTask.Run(() => {
                var map = mapItem.GetMap();
                //We have to make a pane for the map which, in turn,
                //creates the MapView
                ProApp.Panes.CreateMapPaneAsync(map);
            });
        }
    }
}
