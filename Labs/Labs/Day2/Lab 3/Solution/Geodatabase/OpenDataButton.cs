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
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using System.IO;
using ArcGIS.Desktop.Mapping;

namespace GeodatabaseSolution
{
    internal class OpenDataButton : Button
    {
        protected override void OnClick()
        {
            QueuedTask.Run(() => GetFeaturesAndAddAttachment());
        }

        private void GetFeaturesAndAddAttachment()
        {
            // open the geodatabase
            using (var geodatabase = new Geodatabase(@"c:\ProSDKWorkshop\data\generic.gdb"))
            {
                // open the SamplePoints feature class
                var pointFeatureClass = geodatabase.OpenDataset<FeatureClass>("SamplePoints");

                // retrieve all features from the feature class
                using (var rowCursor = pointFeatureClass.Search())
                {
                    while (rowCursor.MoveNext())
                    {
                        // for each feature 
                        using (var currentFeature = rowCursor.Current as Feature)
                        {
                            // add the sample picture as an attachment
                            currentFeature.AddAttachment(new Attachment("SamplePicture", "image/png", 
                                CreateMemoryStreamFromContentsOf(@"c:\ProSDKWorkshop\data\redlands.png")));
                        }
                    }
                }
            }

            // add the feature class as a layer to the active map
            LayerFactory.CreateFeatureLayer(new Uri(@"c:\ProSDKWorkshop\data\generic.gdb\SamplePoints"), MapView.Active.Map);
        }

        /// <summary>
        /// This is one way of converting the content of any file into a MemoryStream
        /// </summary>
        /// <param name="fileNameWithPath"></param>
        /// <returns></returns>
        private static MemoryStream CreateMemoryStreamFromContentsOf(String fileNameWithPath)
        {
            MemoryStream memoryStream = new MemoryStream();

            using (FileStream file = new FileStream(fileNameWithPath, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[file.Length];
                file.Read(bytes, 0, (int)file.Length);
                memoryStream.Write(bytes, 0, (int)file.Length);
            }

            return memoryStream;
        }  
    }
}
