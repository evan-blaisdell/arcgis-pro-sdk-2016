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
using System.Text;
using System.Windows;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Core.Geoprocessing;
using ArcGIS.Desktop.Mapping;
using MessageBox = ArcGIS.Desktop.Framework.Dialogs.MessageBox;

namespace CoarseFineGrained
{
    //
    internal class CoarseGrained : Button
    {
        protected async override void OnClick()
        {
            // TODO: Step 2: Coarse Grained - select features using where clause and then zoom to selection
            // TODO: Step 3: Coarse Grained - correct the layer name and where clause
            await Geoprocessing.ExecuteToolAsync(
                "SelectLayerByAttribute_management", new string[] {
                    "Crimes","NEW_SELECTION","Major_Offense_Type = 'Larceny' And Neighborhood = 'CATHEDRAL PARK'"
                }).ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        // if an exception was thrown in the async task we can process the result here:
                        var aggException = t.Exception.Flatten();
                        var sb = new StringBuilder();
                        foreach (var exception in aggException.InnerExceptions)
                        {
                            sb.AppendLine(exception.Message);
                        }
                        MessageBox.Show(sb.ToString(), "Error in ExecuteToolAsync", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        // Examime the results of the GP task
                        if (t.Result.ErrorCode != 0)
                        {
                            var sb = new StringBuilder();
                            foreach (var msg in t.Result.Messages)
                            {
                                sb.AppendLine(msg.Text);
                            }
                            MessageBox.Show(sb.ToString(), "Error in ExecuteToolAsync", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("GP tool status: {0}", t.Status));
                        }
                    }
                });
            // TODO: Step 4: Coarse Grained - Zoom to the selection
            await MapView.Active.ZoomToSelectedAsync(new TimeSpan(0, 0, 3), true);
            MessageBox.Show("CoarseGrained calls completed!");
        }
    }
}
