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
using System.Windows.Input;
using ArcGIS.Desktop.Editing;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Framework.Threading.Tasks;

namespace GeometryEditingSolution
{
    /// <summary>
    /// A button implementation triggering the map tool to sketch the area for the green
    /// </summary>
    internal class CreateGreenforTrees : Button
    {
        protected override void OnClick()
        {
            var iCommand = FrameworkApplication.GetPlugInWrapper("GeometryEditingSolution_CreateGreenSketchTool") as ICommand;
            if (iCommand != null)
            {
                // Let ArcGIS Pro do the work for us
                if (iCommand.CanExecute(null)) iCommand.Execute(null);
            }
        }
    }

    /// <summary>
    /// A map tool using the sketch geometry to define an area for a green but considers buffer zones
    /// around the point locations.
    /// </summary>
    internal class CreateGreenSketchTool : MapTool
    {
        public CreateGreenSketchTool()
        {
            IsSketchTool = true;
            SketchType = SketchGeometryType.Lasso;
        }

        protected override Task<bool> OnSketchCompleteAsync(Geometry geometry)
        {
            return QueuedTask.Run(() => createFunnyGarden(geometry));
        }

        private Task<bool> createFunnyGarden(Geometry geometry)
        {
            if (geometry == null)
                return Task.FromResult(false);

            // get the point layer from the active map
            var pointLayer = MapView.Active.Map.GetLayersAsFlattenedList().OfType<FeatureLayer>()
                .Where(lyr => lyr.ShapeType == ArcGIS.Core.CIM.esriGeometryType.esriGeometryPoint).FirstOrDefault();

            if (pointLayer == null)
                return Task.FromResult(false);

            // get the polygon layer from the active map
            var polygonLayer = MapView.Active.Map.GetLayersAsFlattenedList().OfType<FeatureLayer>()
                .Where(lyr => lyr.ShapeType == ArcGIS.Core.CIM.esriGeometryType.esriGeometryPolygon).FirstOrDefault();

            if (polygonLayer == null)
                return Task.FromResult(false);

            // Create an edit operation
            var createOperation = new EditOperation();
            createOperation.Name = string.Format("Create green.");

            // we'll use the sketch geometry to create a spatial filter
            SpatialQueryFilter spatialQueryFilter = new SpatialQueryFilter();
            spatialQueryFilter.FilterGeometry = geometry;
            spatialQueryFilter.SpatialRelationship = SpatialRelationship.Contains;

            // retrieve the point features that are inside the sketch geometry
            var searchCursor = pointLayer.Search(spatialQueryFilter);

            // Construct a polygon placeholder for the point buffers
            var treeBuffers = PolygonBuilder.CreatePolygon(geometry.SpatialReference);

            // for each found feature
            while (searchCursor.MoveNext())
            {
                // retrieve the geometry
                var treeFeature = searchCursor.Current as Feature;
                var treePoint = treeFeature.GetShape() as MapPoint;

                // buffer the the location
                var treeBuffer = GeometryEngine.Buffer(treePoint, MapView.Active.Extent.Width / 50);

                // and union the new buffer to the existing buffer polygons
                treeBuffers = GeometryEngine.Union(treeBuffer, treeBuffers) as Polygon;
            }

            if (!GeometryEngine.IsSimpleAsFeature(geometry, true))
                geometry = GeometryEngine.SimplifyAsFeature(geometry);

            // construct the difference geometry between the sketch geometry and the buffered point polygons
            var greenGeometry = GeometryEngine.Difference(geometry, treeBuffers);

            // cue the create 
            createOperation.Create(polygonLayer, greenGeometry);

            // execute the operation
            return createOperation.ExecuteAsync();
        }
    }
}
