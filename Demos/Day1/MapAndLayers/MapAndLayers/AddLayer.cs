using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;

namespace MapAndLayers
{
  internal class AddLayer : Button
  {
    protected override void OnClick()
    {
      OpenItemDialog openDlg = new OpenItemDialog();
      openDlg.Title = "Add Layers";
      openDlg.InitialLocation = Project.Current.HomeFolderPath;
      openDlg.Filter = ItemFilters.default_addToMap;
      openDlg.MultiSelect = true;

      if (openDlg.ShowDialog() ?? false)
      {
        QueuedTask.Run(() =>
        {
          IEnumerable<ArcGIS.Desktop.Core.Item> selectedItems = openDlg.Items;
          foreach (var item in selectedItems)
          {
            if (LayerFactory.CanCreateLayerFrom(item))
              LayerFactory.CreateLayer(item, MapView.Active.Map);
          }
        });
      }
    }
  }
}
