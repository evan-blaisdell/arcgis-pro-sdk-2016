//   Copyright 2016 Esri
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 

using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectItems
{
  /// <summary>
  /// Button to add a new folder connection
  /// </summary>
  internal class AddFolderConnection : Button
  {
    protected override async void OnClick()
    {
        // add a folder connection
        Item item = ItemFactory.Create(Module1.FolderPath);
        IEnumerable<ProjectItem> folders = await Project.Current.AddAsync(item);

        // double check - ensure it has been created
        FolderConnectionProjectItem folder = folders.First() as FolderConnectionProjectItem;
        if (folder != null)
            ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Folder added " + folder.Path, "Project Items Lab", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        else
            ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("No folder added", "Project Items Lab", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);

    }
  }
}
