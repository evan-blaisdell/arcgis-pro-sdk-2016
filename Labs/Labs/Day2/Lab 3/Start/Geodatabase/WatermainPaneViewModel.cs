//Copyright 2015-2016 Esri

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
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Core.Data;
using System.Collections.ObjectModel;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Framework.Threading.Tasks;

namespace GeodatabaseExercises
{
    internal class WatermainPaneViewModel : DockPane
    {
        // build a dictionary containing the name of the layer and the names of the associated queries
        Dictionary<string, List<WatermainQuery>> queryMap = new Dictionary<string, List<WatermainQuery>>
        {
            {"Water Mains Life Expectancy", 
                new List<WatermainQuery>{ new UnkownMaterialQuery{Name = "Unknown Material"}, 
                    new ExcellentCastIronQuery{Name = "Iron in excellent condition"} 
                }
            }
        };

        private const string _dockPaneID = "GeodatabaseExercises_Watermainpane";

        private string _selectedQuery;
        private string _selectedLayerName;
        private FeatureLayer _selectedLayer = null;

        private ObservableCollection<string> _queries;
        private ObservableCollection<string> _layers;
        private ObservableCollection<WaterMainData> _waterMainData;

        protected override void OnActivate(bool isActive)
        {
            // when the dockpane is activated make sure the layers are updated
            if (isActive)
            {
                // .. in case there is no selected layer yet
                // this might happen if the dockpane is instantiated when the project opens
                if (_selectedLayer == null)
                    UpdateLayers();
            }
        }

        /// <summary>
        /// Show the DockPane.
        /// </summary>
        internal static void Show()
        {
            DockPane pane = FrameworkApplication.DockPaneManager.Find(_dockPaneID);
            if (pane == null)
                return;

            pane.Activate();

        }

        /// <summary>
        /// Text shown near the top of the DockPane.
        /// </summary>
        private string _heading = "Water main queries";
        public string Heading
        {
            get { return _heading; }
            set
            {
                SetProperty(ref _heading, value, () => Heading);
            }
        }

        /// <summary>
        /// The title of the currently selected query. 
        /// </summary>
        public string SelectedQuery
        {
            get { return _selectedQuery; }
            set
            {
                SetProperty(ref _selectedQuery, value, () => SelectedQuery);

                if (!String.IsNullOrEmpty(SelectedQuery))
                    RetrieveData(_selectedLayer);
            }
        }

        /// <summary>
        /// The name of the currently selected layer.
        /// </summary>
        public string SelectedLayer
        {
            get
            {
                return _selectedLayerName; }
            set
            {
                SetProperty(ref _selectedLayerName, value, () => SelectedLayer);

                if (MapView.Active == null || String.IsNullOrEmpty(value))
                    return;

                // based on the given name find the layer in the map
                _selectedLayer = MapView.Active.Map.GetLayersAsFlattenedList().OfType<FeatureLayer>().Where(lyr => lyr.Name.Equals(value)).FirstOrDefault();

                if (_selectedLayer == null)
                    return;

                // if the selected layer changes look for stored queries for that layer
                if (!String.IsNullOrEmpty(_selectedLayerName))
                    UpdateQueries(_selectedLayer);
            }
        }

        /// <summary>
        /// Collection of available queries.
        /// </summary>
        public ObservableCollection<string> Queries
        { 
            get { return _queries; }
            set {
                SetProperty(ref _queries, value, () => Queries);
            }
        }

        /// <summary>
        /// Collection of available feature layers.
        /// </summary>
        public ObservableCollection<string> Layers
        { 
            get { return _layers; }
            set 
            {
                SetProperty(ref _layers, value, () => Layers);
            }
        }

        /// <summary>
        /// Collection of information about the water mains
        /// </summary>
        public ObservableCollection<WaterMainData> FeatureData
        { 
            get { return _waterMainData; }
            set 
            {
                SetProperty(ref _waterMainData, value, () => FeatureData);
            }
        }

        /// <summary>
        /// This method will populate the Layers (bound to the LayersComboBox) with all the Feature Layers
        /// present in the Active Map View
        /// </summary>
        private void UpdateLayers()
        {
            if (MapView.Active == null)
            {
                Layers = new ObservableCollection<string>();
            }
            else
            {
                Layers = new ObservableCollection<string>(MapView.Active.Map.GetLayersAsFlattenedList()
                    .OfType<FeatureLayer>().Select(layer => layer.Name));
            }
        }

        /// <summary>
        /// Based on the selected layer name, the query Map is used to populate the Queries Combobox
        /// </summary>
        /// <param name="selectedItem"></param>
        public void UpdateQueries(FeatureLayer selectedLayer)
        {
            if (selectedLayer == null)
            {
                Queries = new ObservableCollection<string>();
                return;
            }

            QueuedTask.Run(() =>
            {
                using (Table table = selectedLayer.GetTable())
                {
                    if (queryMap.ContainsKey(selectedLayer.Name))
                        Queries = new ObservableCollection<string>(queryMap[selectedLayer.Name].Select(query => query.Name));
                    else
                        Queries = new ObservableCollection<string>();
                }
            });
        }

        /// <summary>
        /// Based on the Query Selected, the FeatureData observable collection (bound to the DataGrid) is
        /// populated with the results of the Query
        /// </summary>
        public void RetrieveData(FeatureLayer selectedLayer)
        {
            if (selectedLayer == null)
            {
                FeatureData = new ObservableCollection<WaterMainData>();
                return;
            }

            QueuedTask.Run(() =>
            {
                using (Table table = selectedLayer.GetTable())
                {
                    if (queryMap.ContainsKey(selectedLayer.Name))
                        FeatureData = new ObservableCollection<WaterMainData>(queryMap[selectedLayer.Name]
                            .First(query => query.Name.Equals(SelectedQuery)).Execute(table));
                    else
                        FeatureData = new ObservableCollection<WaterMainData>();
                }
            });
        }
    }

    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class WatermainPane_ShowButton : Button
    {
        protected override void OnClick()
        {
            WatermainPaneViewModel.Show();
        }
    }


    /// <summary>
    /// Class for defining and executing the queries against the Naperville dataset
    /// </summary>
    internal abstract class WatermainQuery 
    {
        //
        public abstract List<WaterMainData> Execute(Table table);
        public string Name { get; set; }

        protected List<WaterMainData> PopulateWatermainData(Table table, QueryFilter queryFilter)
        {
            var list = new List<WaterMainData>();
            using (RowCursor rowCursor = table.Search(queryFilter, false))
            {
                while (rowCursor.MoveNext())
                {
                    using (Row current = rowCursor.Current)
                    {
                        list.Add(new WaterMainData
                        {
                            // Uncomment the lines below 
                            // read the feature attribute values and use them initialize the water main instance
                            FacilityID = Convert.ToString(current["FACILITYID"]),
                            RemainingLifeExpectancy = Convert.ToInt32(current["REMLIF"]),
                            RelativeScore = Convert.ToString(current["SCORE"]),
                            PipeMaterial = Convert.ToString(current["MATERIAL"])
                        });
                    }
                }
            }
            return list;
        }
    }

    /// <summary>
    /// Class representing a query for water mains of unknown material sorted by facility ID
    /// </summary>
    internal class UnkownMaterialQuery : WatermainQuery
    {
        public override List<WaterMainData> Execute(Table table)
        {
            var queryFilter = new QueryFilter
            {
                // uncomment lines below
                WhereClause = "SCORE = 0",
                PrefixClause = "DISTINCT",
                PostfixClause = "ORDER BY FACILITYID",
                SubFields = "FACILITYID,REMLIF,SCORE,MATERIAL"
            };

            return base.PopulateWatermainData(table, queryFilter);
        }
    }

    /// <summary>
    /// Class representing a query for iron cast water mains of excellent condition
    /// </summary>
    internal class ExcellentCastIronQuery : WatermainQuery
    {
        public override List<WaterMainData> Execute(Table table)
        {
            var queryFilter = new QueryFilter
            {
                // uncomment lines below
                WhereClause = "SCORE = 1 AND MATERIAL = 'CAS'",
                PrefixClause = "DISTINCT",
                PostfixClause = "ORDER BY REMLIF",
                SubFields = "FACILITYID,REMLIF,SCORE,MATERIAL"
            };

            return base.PopulateWatermainData(table, queryFilter);
        }
    }

    /// <summary>
    /// Class representing information about the water mains. 
    /// </summary>
    internal class WaterMainData
    {
        public string FacilityID { get; set; }
        public int RemainingLifeExpectancy { get; set; }
        public string RelativeScore { get; set; }
        public string PipeMaterial { get; set; }
    }
}
