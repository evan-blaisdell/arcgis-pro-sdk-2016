﻿//Copyright 2015-2016 Esri

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
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Editing.Attributes;
using ArcGIS.Desktop.Framework.Threading.Tasks;

namespace EditingExercisesSolution
{
    /// <summary>
    /// Sample sketch tool on updating attributes
    /// </summary>
    internal class UpdateAttributes : MapTool
    {
        public UpdateAttributes()
        {
            IsSketchTool = true;
            // Select the type of construction tool you wish to implement.  
            // Make sure that the tool is correctly registered with the correct component category type in the daml 
            SketchType = SketchGeometryType.Rectangle;

            SketchOutputMode = ArcGIS.Desktop.Mapping.SketchOutputMode.Map;
        }

        /// <summary>
        /// Called when the sketch finishes. This is where we will create the sketch operation and then execute it.
        /// </summary>
        /// <param name="geometry">The geometry created by the sketch.</param>
        /// <returns>A Task returning a Boolean indicating if the sketch complete event was successfully handled.</returns>
        protected override Task<bool> OnSketchCompleteAsync(Geometry geometry)
        {
            if (geometry == null)
                return Task.FromResult(false);

            var featureLayer = MapView.Active.GetSelectedLayers()[0] as FeatureLayer;

            if (featureLayer == null)
            {
                MessageBox.Show("Please ensure that the selected layer is a feature layer.");
                return Task.FromResult(false);
            }

            return QueuedTask.Run(() =>
            {
                var desiredField = featureLayer.GetFieldDescriptions().Where(fldDesc => fldDesc.Name == "Comments").FirstOrDefault();

                if (desiredField == null)
                {
                  MessageBox.Show("No field with Name 'Comments' found.");
                  return Task.FromResult(false);
                }

                var features = MapView.Active.GetFeatures(geometry);
                if (features.Count == 0)
                  return Task.FromResult(false);

                var featureOIDs = features.Where(results => results.Key == featureLayer).Select(results => results.Value).First();

                var featureInspector = new Inspector(true);
                featureInspector.Load(featureLayer, featureOIDs);
                featureInspector["Comments"] = "ArcGIS Pro SDK Sample";

                var modifyOperation = new EditOperation();
                modifyOperation.Name = "Update attribute";
                modifyOperation.SelectModifiedFeatures = true;
                modifyOperation.SelectNewFeatures = false;

                modifyOperation.Modify(featureInspector);

                // apply the edits
                return modifyOperation.ExecuteAsync();
            });
        }
    }
}
