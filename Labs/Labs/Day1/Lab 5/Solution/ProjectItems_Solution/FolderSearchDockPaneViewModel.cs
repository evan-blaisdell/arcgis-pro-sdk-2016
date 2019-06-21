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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Core;
using System.Windows.Input;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Framework.Threading.Tasks;

namespace ProjectItems_Solution
{
  /// <summary>
  /// View model for the bookmarks dockpane.  
  /// </summary>
  internal class FolderSearchDockPaneViewModel : DockPane
  {
    private const string _dockPaneID = "ProjectItems_Solution_FolderSearchDockPane";

    protected FolderSearchDockPaneViewModel() { }

    /// <summary>
    /// Show the DockPane.
    /// </summary>
    internal static void Show()
    {
      DockPane pane = FrameworkApplication.DockPaneManager.Find(_dockPaneID);
      if (pane == null)
        return;

      pane.Activate();
    }

    /// <summary>
    /// Text shown near the top of the DockPane.
    /// </summary>
    private string _heading = "Search and Import";
    public string Heading
    {
      get { return _heading; }
      set
      {
        SetProperty(ref _heading, value, () => Heading);
      }
    }


    /// <summary>
    /// Performed on uninitialization of the dockpane
    /// </summary>
    /// <returns></returns>
    protected override Task UninitializeAsync()
    {
      // disconnect all the commands
      if (_searchCommand != null)
        _searchCommand.Disconnect();
      _searchCommand = null;

      if (_importMxdCommand != null)
        _importMxdCommand.Disconnect();
      _importMxdCommand = null;

      return base.UninitializeAsync();
    }

    /// <summary>
    /// The folder to be searched
    /// </summary>
    private string _folder = Module1.FolderPath;
    public string Folder
    {
      get { return _folder; }
      set { SetProperty(ref _folder, value, () => Folder); }
    }

    /// <summary>
    /// The search string
    /// </summary>
    private string _searchString = "mxd";
    public string SearchString
    {
      get { return _searchString; }
      set { SetProperty(ref _searchString, value, () => SearchString); }
    }

    /// <summary>
    /// The search results
    /// </summary>
    private List<Item> _searchResults;
    public List<Item> SearchResults
    {
      get { return _searchResults; }
      set { SetProperty(ref _searchResults, value, () => SearchResults); }
    }

    /// <summary>
    /// a RelayCommand to search the folder
    /// </summary>
    private RelayCommand _searchCommand;
    public ICommand SearchCommand
    {
      get
      {
        if (_searchCommand == null)
          _searchCommand = new RelayCommand(() => this.Search());
        return _searchCommand;
      }
    }

    /// <summary>
    /// Performs the search
    /// </summary>
    /// <returns>A Task to the search function</returns>
    private async Task Search()
    {
      // reset the results
      SearchResults = null;

      // verify there is a folder
      if (string.IsNullOrEmpty(Folder))
        return;

      // find the folder project item
      FolderConnectionProjectItem folder = Project.Current.GetItems<FolderConnectionProjectItem>().FirstOrDefault(f => f.Path == Folder);
      if (folder == null)
        return;

      // do the search
      IEnumerable<Item> results = await folder.SearchAsync(SearchString);

      // assign to the results variable
      SearchResults = results.ToList();
    }

    /// <summary>
    /// Is there a selected mxd item
    /// </summary>
    private Item _selectedItem;
    public Item SelectedMxd
    {
      get { return _selectedItem; }
      set
      {
        SetProperty(ref _selectedItem, value, () => SelectedMxd);
        // refresh the HasMxdSelected for the view
        NotifyPropertyChanged(() => HasMxdSelected);
      }
    }

    /// <summary>
    /// Has an Mxd been selected
    /// </summary>
    public bool HasMxdSelected
    {
      get { return _selectedItem != null; }
    }

    /// <summary>
    /// Implement a RelayCommand to import the selected mxd
    /// </summary>
    private RelayCommand _importMxdCommand;
    public ICommand ImportMxdCommand
    {
      get
      {
        if (_importMxdCommand == null)
          _importMxdCommand = new RelayCommand(() => this.ImportMxd());
        return _importMxdCommand;
      }
    }

    /// <summary>
    /// Imports the selected mxd to the project
    /// </summary>
    internal async Task ImportMxd()
    {
      // verify that a map can be created from the selection
      if (MapFactory.CanCreateMapFrom(SelectedMxd))
      {
        // creates a new map and adds it to the project.  Also opens the mapview
        Map map = await MapFactory.CreateMapAsync(SelectedMxd); 
      }
    }
  }

  /// <summary>
  /// Button implementation to show the DockPane.
  /// </summary>
  internal class FolderSearchDockPane_ShowButton : Button
  {
    protected override void OnClick()
    {
      FolderSearchDockPaneViewModel.Show();
    }
  }
}
