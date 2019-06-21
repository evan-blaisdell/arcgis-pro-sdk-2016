//Copyright 2015 Esri

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
using System.Linq;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Core.Data;

namespace CoarseFineGrained
{
    internal class Module1 : Module
    {
        private static Module1 _this = null;

        /// <summary>
        /// Retrieve the singleton instance to this module here
        /// </summary>
        public static Module1 Current
        {
            get
            {
                return _this ?? (_this = (Module1)FrameworkApplication.FindModule("CoarseFineGrained_Module"));
            }
        }

        /// <summary>
        /// coarse grained function that selects features (given feature class name) using a where clause
        /// </summary>
        /// <param name="featureLayerName">Name of the feature layer to select features from</param>
        /// <param name="selectionWhereClause">where clause to use for selectoin</param>
        /// <returns>the resulting selection count </returns>
        public static Task<int> SelectByAttributeAsync(string featureLayerName, string selectionWhereClause)
        {
            return QueuedTask.Run(() =>
            {
                var firstLyr =
                    MapView.Active.Map.FindLayers(featureLayerName).FirstOrDefault() as FeatureLayer;
                if (firstLyr == null) throw new Exception(string.Format("The feature class: {0} does not exist", featureLayerName));
                var qf = new QueryFilter()
                {
                    WhereClause = selectionWhereClause,
                    SubFields = "*"
                };
                var selectionCount = firstLyr.Select(qf, SelectionCombinationMethod.New).GetCount();
                MapView.Active.ZoomToSelected(new TimeSpan(0, 0, 3), true);
                return selectionCount;
            });
        }

        #region Overrides
        /// <summary>
        /// Called by Framework when ArcGIS Pro is closing
        /// </summary>
        /// <returns>False to prevent Pro from closing, otherwise True</returns>
        protected override bool CanUnload()
        {
            //TODO - add your business logic
            //return false to ~cancel~ Application close
            return true;
        }

        #endregion Overrides

    }
}
