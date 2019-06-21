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
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Editing;

namespace GeometryEditingSolution
{
    /// <summary>
    /// A sample button implementation generating lines from the previously generated point locations.
    /// </summary>
    internal class createPolylines : Button
    {
        protected override async void OnClick()
        {
            // to work in the context of the active display retrieve the current map 
            Map activeMap = MapView.Active.Map;

            // retrieve the first point layer in the map
            FeatureLayer pointFeatureLayer = activeMap.GetLayersAsFlattenedList().OfType<FeatureLayer>().Where(
                lyr => lyr.ShapeType == ArcGIS.Core.CIM.esriGeometryType.esriGeometryPoint).FirstOrDefault();

            // retrieve the first polyline feature layer in the map
            FeatureLayer polylineFeatureLayer = activeMap.GetLayersAsFlattenedList().OfType<FeatureLayer>().Where(
                lyr => lyr.ShapeType == ArcGIS.Core.CIM.esriGeometryType.esriGeometryPolyline).FirstOrDefault();

            // construct polyline from points
            await constructSamplePolylines(polylineFeatureLayer, pointFeatureLayer);
        }

        /// <summary>
        /// Create sample polyline feature using the geometries from the point feature layer.
        /// </summary>
        /// <param name="polylineLayer">Polyline geometry feature layer used to add the new features.</param>
        /// <param name="pointLayer">The geometries from the point layer are used as vertices for the new line features.</param>
        /// <returns></returns>
        private Task<bool> constructSamplePolylines(FeatureLayer polylineLayer, FeatureLayer pointLayer)
        {

            // execute the fine grained API calls on the CIM main thread
            return QueuedTask.Run(() =>
            {
                // get the underlying feature class for each layer
                var polylineFeatureClass = polylineLayer.GetTable() as FeatureClass;
                var pointFeatureClass = pointLayer.GetTable() as FeatureClass;

                // retrieve the feature class schema information for the feature classes
                var polylineDefinition = polylineFeatureClass.GetDefinition() as FeatureClassDefinition;
                var pointDefinition = pointFeatureClass.GetDefinition() as FeatureClassDefinition;

                // construct a cursor for all point features, since we want all feature there is no
                // QueryFilter required
                var pointCursor = pointFeatureClass.Search(null, false);

                // initialize a counter variable
                int pointCounter = 0;
                // initialize a list to hold 5 coordinates that are used as vertices for the polyline
                var lineCoordinates = new List<Coordinate>(5);

                // set up the edit operation for the feature creation
                var createOperation = new EditOperation();
                createOperation.Name = "Create polylines";
                createOperation.SelectNewFeatures = false;

                // set up the datum transformation to be used in the projection
                ProjectionTransformation transformation = ProjectionTransformation.CreateFromEnvironment(pointDefinition.GetSpatialReference(),
                    polylineDefinition.GetSpatialReference());

                // loop through the point features
                while (pointCursor.MoveNext())
                {
                    pointCounter++;

                    var pointFeature = pointCursor.Current as Feature;
                    // add the feature point geometry as a coordinate into the vertex list of the line
                    // - ensure that the projection of the point geometry is converted to match the spatial reference of the line
                    // with a datum transformation considering the different spheroids
                    lineCoordinates.Add(((MapPoint)GeometryEngine.ProjectEx(pointFeature.GetShape(), transformation)).Coordinate);

                    // for every 5 geometries, construct a new polyline and queue a feature create
                    if (pointCounter % 5 == 0)
                    {
                        // construct a new polyline by using the 5 point coordinate in the current list
                        var newPolyline = PolylineBuilder.CreatePolyline(lineCoordinates, polylineDefinition.GetSpatialReference());
                        // queue the create operation as part of the edit operation
                        createOperation.Create(polylineLayer, newPolyline);
                        // reset the list of coordinates
                        lineCoordinates = new List<Coordinate>(5);
                    }
                }

                // execute the edit (create) operation
                return createOperation.ExecuteAsync();
            });
        }

    }
}
