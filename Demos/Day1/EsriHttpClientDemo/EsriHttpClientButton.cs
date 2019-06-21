using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using Newtonsoft.Json.Linq;

namespace EsriHttpClientDemo {
    internal class EsriHttpClientButton : Button {
        protected override async void OnClick() {
            UriBuilder searchURL = new UriBuilder(PortalManager.GetActivePortal());
            searchURL.Path = "sharing/rest/search";
            string layers = "(type:\"Map Service\" OR type:\"Image Service\" OR type:\"Feature Service\" OR type:\"WMS\" OR type:\"KML\")";
            //any public layer content
            searchURL.Query = string.Format("q={0}&f=json", layers);

            EsriHttpClient httpClient = new EsriHttpClient();

            var searchResponse = httpClient.Get(searchURL.Uri.ToString());
            dynamic resultItems = JObject.Parse(await searchResponse.Content.ReadAsStringAsync());

            long numberOfTotalItems = resultItems.total.Value;
            if (numberOfTotalItems == 0)
                return;

            List<dynamic> resultItemList = new List<dynamic>();
            resultItemList.AddRange(resultItems.results);
            //get the first result
            dynamic item = resultItemList[0];

            string itemID = item.id;
            Item currentItem = ItemFactory.Create(itemID, ItemFactory.ItemType.PortalItem);

            await QueuedTask.Run(() => {
                // if we have an item that can be turned into a layer
                // add it to the map
                if (LayerFactory.CanCreateLayerFrom(currentItem))
                    LayerFactory.CreateLayer(currentItem, MapView.Active.Map);
            });
        }
    }
}
