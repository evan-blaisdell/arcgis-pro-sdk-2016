using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;

namespace ProjectItemsDemo {
    internal class ProjectItemsButton2 : Button {
        protected override async void OnClick()
        {
            string folderPath = @"C:\ProSDkWorkshop\Data\MXDs";
            var fcItem = ItemFactory.Create(folderPath, ItemFactory.ItemType.PathItem);
            var projectItems = await Project.Current.AddAsync(fcItem);

            //FolderConnectionProjectItem
            var folderProjectItem = projectItems.First();
            
            var mxds = await folderProjectItem.SearchAsync(".mxd");
            await Project.Current.AddAsync(mxds.First());
        }
    }
}
