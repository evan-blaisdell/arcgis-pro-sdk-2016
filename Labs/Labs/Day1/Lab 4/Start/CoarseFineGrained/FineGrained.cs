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

using System.Text;
using System.Windows;
using ArcGIS.Desktop.Framework.Contracts;
using MessageBox = ArcGIS.Desktop.Framework.Dialogs.MessageBox;

namespace CoarseFineGrained
{
    internal class FineGrained : Button
    {
        protected async override void OnClick()
        {
            // TODO: Step 5: Fine Grained - select features using where clause and then zoom to selection
            // TODO: Step 6: Fine Grained - correct the layer name and where clause
            var featureLayerName = "Crimes";
            var featureSelectClause = "Major_Offense_Type <> 'Larceny' And Neighborhood = 'CATHEDRAL PARK'";

            await Module1.SelectByAttributeAsync(featureLayerName, featureSelectClause)
            .ContinueWith(t =>
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
                        MessageBox.Show(sb.ToString(), "Error in SelectByAttributeAsync", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show(string.Format("{0} features selected", t.Result), "Feature Selection");
                    }
                });
        }
    }
}
