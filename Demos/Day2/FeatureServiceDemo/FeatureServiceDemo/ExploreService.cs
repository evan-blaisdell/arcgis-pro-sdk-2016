using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;

namespace FeatureServiceDemo
{
    internal class ExploreService : Button
    {
        protected override void OnClick()
        {
            // Approach 1
            // open feature service by known URL

            ServiceConnectionProperties svcsProperties = new ServiceConnectionProperties(
                new Uri("http://services.arcgis.com/6DIQcwlPy8knb6sg/arcgis/rest/services/Openstreetmap/FeatureServer"));

            QueuedTask.Run(() =>
            {
                using (FeatureService featureService = new FeatureService(svcsProperties))
                {
                    var footprints = featureService.OpenDataset<FeatureClass>(0);
                    var featureClassDefinition = footprints.GetDefinition();

                    var oidField = featureClassDefinition.GetObjectIDField();
                }
            });


            // Approach 2
            // open feature service from layer
            var featureLayer = MapView.Active.Map.GetLayersAsFlattenedList().OfType<FeatureLayer>().FirstOrDefault();

            QueuedTask.Run(() =>
            {
                using (FeatureService featureService = featureLayer.GetFeatureClass().GetDatastore() as FeatureService)
                {
                    var footprints = featureService.OpenDataset<FeatureClass>(0);
                    var footprintsFeatureClass = featureLayer.GetFeatureClass();
                    var featureClassDefinition = footprints.GetDefinition();

                    var oidField = featureClassDefinition.GetObjectIDField();

                    using (var featureCursor = footprints.Search(new QueryFilter() { WhereClause = "building = 'yes'" }, false))
                    {
                        while (featureCursor.MoveNext())
                        {
                            var currentFeature = featureCursor.Current as Feature;

                            System.Diagnostics.Debug.WriteLine(currentFeature["osmuser"]);
                        }
                    }
                }
            });

        }
    }
}
