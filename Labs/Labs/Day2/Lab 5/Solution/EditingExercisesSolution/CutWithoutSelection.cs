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
using System.Windows;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Core.Data;

namespace EditingExercisesSolution
{
    /// <summary>
    /// A sample sketch tool that uses the sketch line geometry to cut underlying polygons.
    /// </summary>
    internal class CutWithoutSelection : MapTool
    {
        public CutWithoutSelection()
        {
            // select the type of construction tool you wish to implement.  
            // Make sure that the tool is correctly registered with the correct component category type in the daml
            SketchType = SketchGeometryType.Line;
            // a sketch feedback is need
            IsSketchTool = true;
            // the geometry is needed in map coordinates
            SketchOutputMode = ArcGIS.Desktop.Mapping.SketchOutputMode.Map;
        }

        /// <summary>
        /// Called when the sketch finishes. This is where we will create the sketch operation and then execute it.
        /// </summary>
        /// <param name="geometry">The geometry created by the sketch.</param>
        /// <returns>A Task returning a Boolean indicating if the sketch complete event was successfully handled.</returns>
        protected override Task<bool> OnSketchCompleteAsync(Geometry geometry)
        {
            return QueuedTask.Run(() => ExecuteCut(geometry));
        }

        protected Task<bool> ExecuteCut(Geometry geometry)
        {
            if (geometry == null)
                return Task.FromResult(false);

            // create an edit operation
            EditOperation op = new EditOperation();
            op.Name = "Cut Elements";
            op.ProgressMessage = "Working...";
            op.CancelMessage = "Operation canceled.";
            op.ErrorMessage = "Error cutting polygons";
            op.SelectModifiedFeatures = false;
            op.SelectNewFeatures = false;

            // create a collection of feature layers that can be edited
            var editableLayers = ActiveMapView.Map.GetLayersAsFlattenedList().OfType<FeatureLayer>()
                .Where(lyr => lyr.CanEditData() == true).Where(lyr => lyr.ShapeType == ArcGIS.Core.CIM.esriGeometryType.esriGeometryPolygon);

            // for each of the layers 
            foreach (FeatureLayer editableFeatureLayer in editableLayers)
            {
                // get the feature class associated with the layer
                Table fc = editableFeatureLayer.GetTable();

                // initialize a list of ObjectIDs that need to be cut
                var cutOIDs = new List<long>();

                // find the features crossed by the sketch geometry
                var rowCursor = fc.Search(geometry, SpatialRelationship.Crosses);

                // add the feature IDs into our prepared list
                while (rowCursor.MoveNext())
                {
                    var feature = rowCursor.Current as Feature;

                    if (feature == null)
                        break;

                    if (feature.GetShape() != null)
                    {
                        // we are interested in the intersection points
                        // in case there is only one intersection then the sketch geometry doesn't enter and leave the 
                        // base geometry and the cut operation won't work.
                        Geometry intersectionGeometry = GeometryEngine.Intersection(feature.GetShape(), geometry, GeometryDimension.esriGeometry0Dimension);
                        if (intersectionGeometry is Multipoint)
                        {
                            var intersectionPoints = intersectionGeometry as Multipoint;
                            // we are only interested in feature IDs where the count of intersection points is larger than 1
                            // i.e., at least one entry and one exit
                            if (intersectionPoints.Points.Count > 1)
                            {
                                // add the current feature to the overall list of features to cut
                                cutOIDs.Add(rowCursor.Current.GetObjectID());
                            }
                        }
                    }
                }

                // add the elements to cut into the edit operation
                op.Cut(editableFeatureLayer, cutOIDs, geometry);

            }

            //execute the operation
            return op.ExecuteAsync();

        }
    }

    /// <summary>
    /// Extension method to search and retrieve rows
    /// </summary>
    public static class TableExtensions
    {
        /// <summary>
        /// Performs a spatial query against the table/feature class.
        /// </summary>
        /// <remarks>It is assumed that the feature class and the search geometry are using the same spatial reference.</remarks>
        /// <param name="searchTable">The table/feature class to be searched.</param>
        /// <param name="searchGeometry">The geometry used to perform the spatial query.</param>
        /// <param name="spatialRelationship">The spatial relationship used by the spatial filter.</param>
        /// <returns></returns>
        public static RowCursor Search(this Table searchTable, Geometry searchGeometry, SpatialRelationship spatialRelationship)
        {
            RowCursor rowCursor = null;

            // define a spatial query filter
            var spatialQueryFilter = new SpatialQueryFilter
            {
                // passing the search geometry to the spatial filter
                FilterGeometry = searchGeometry,
                // define the spatial relationship between search geometry and feature class
                SpatialRelationship = spatialRelationship
            };

            // apply the spatial filter to the feature class in question
            rowCursor = searchTable.Search(spatialQueryFilter);

            return rowCursor;
        }
    }
}
