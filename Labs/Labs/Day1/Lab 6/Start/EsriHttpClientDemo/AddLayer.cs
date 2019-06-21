//Copyright 2016 Esri

//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
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
using Newtonsoft.Json.Linq;
using ArcGIS.Desktop.Core;
using Newtonsoft.Json.Linq;

namespace EsriHttpClientDemo {
    internal class AddLayer : Button {
        protected async override void OnClick() {
            //TODO - Get the URL of the Active Portal
            //HINT: Use PortalManager
            string portalUrl = PortalManager.GetActivePortal().ToString();
            
            UriBuilder searchURL = new UriBuilder(portalUrl);

            searchURL.Path = "sharing/rest/search";
            string layers = "(type:\"Map Service\" OR type:\"Image Service\" OR type:\"Feature Service\" OR type:\"WMS\" OR type:\"KML\")";
            //any public layer content
            searchURL.Query = string.Format("q={0}&f=json", layers);

            EsriHttpClient httpClient = new EsriHttpClient();

            var searchResponse = httpClient.Get(searchURL.Uri.ToString());
            dynamic resultItems = JObject.Parse(await searchResponse.Content.ReadAsStringAsync());
            
            //Use EsriHttpClient to call Online - Use the GET method

            //TODO get the JSON result from the searchResponse
            //HINT - examine methoods of searchResponse.Content

            //TODO Parse the returned JSON - here you will use the Newtonsoft library's JObject
            //examine JObject.Parse. Use a dynamic type to get the results of the Parse
            long numberOfTotalItems = resultItems.total.Value;
            if (numberOfTotalItems == 0)
                return;

            List<dynamic> resultItemList = new List<dynamic>();
            resultItemList.AddRange(resultItems.results);
            //TODO add the ".results" of the returned JSON.
            //eg: resultItemList.AddRange(resultItems.results);

            //get the first result from resultItemList (or an index of your choice)
            //dynamic item = ....
            dynamic item = resultItemList[0];

            
            //TODO create an Item via the ItemFactory. Use the ItemFactory.ItemType.PortalItem item type.
            string itemID = item.id;
            Item currentItem = ItemFactory.Create(itemID, ItemFactory.ItemType.PortalItem);

            await QueuedTask.Run(() => {
                // if we have an item that can be turned into a layer
                // add it to the map
                //TODO use the LayerFactory.CreateLayer and the MapView.Active.Map
                if (LayerFactory.CanCreateLayerFrom(currentItem))
                    LayerFactory.CreateLayer(currentItem, MapView.Active.Map);

            });
        }
    }
}
