//   Copyright 2015 Esri
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
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Core.CIM;
using System.Windows.Data;
using ArcGIS.Desktop.Core;
using System.Collections.ObjectModel;
using ArcGIS.Desktop.Core.Events;

namespace CustomSymbolPicker
{
  internal class SymbolPickerViewModel : DockPane
  {
    private const string _dockPaneID = "CustomSymbolPicker_SymbolPickerDockpane";

    /// <summary>
    /// Constructor for the dock pane
    /// </summary>
    protected SymbolPickerViewModel() 
    {
      UpdateStyleList();
      ProjectOpenedEvent.Subscribe(OnProjectOpened);
    }

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
    /// Valid types of symbols that can be searched for in a style.
    /// </summary>
    private List<string> _symbolTypes = new List<string>() { "Point symbols", "Line symbols", "Polygon symbols"};
    public List<string> SymbolTypes
    {
      get { return _symbolTypes; }   
    }

    /// <summary>
    /// Selected symbol type.
    /// </summary>
    private string _selectedSymbolType;
    public string SelectedSymbolType
    {
      get { return _selectedSymbolType; }
      set
      {
        SetProperty(ref _selectedSymbolType, value, () => SelectedSymbolType);
        SearchForSymbols();
      }
    }

    /// <summary>
    /// List of styles on the project.
    /// </summary> 
    private List<StyleProjectItem> _styleProjectItems = new List<StyleProjectItem>();
    public List<StyleProjectItem> StyleProjectItems
    {
      get
      {
        return _styleProjectItems;
      }
    }

    /// <summary>
    /// Selected style from which we will search for symbols.
    /// </summary>
    private StyleProjectItem _selectedStyle;
    public StyleProjectItem SelectedStyle
    {
      get
      {
        return _selectedStyle;
      }
      set 
      {
        SetProperty(ref _selectedStyle, value, () => SelectedStyle);
        SearchForSymbols();
      }
    }

    /// <summary>
    /// Search string used to limit the returned symbols.
    /// </summary>
    private string _searchString = "";
    public string SearchString
    {
      get
      {
        return _searchString;
      }
      set
      {
        SetProperty(ref _searchString, value, () => SearchString);
        SearchForSymbols();
      }
    }

    /// <summary>
    /// Symbols returned from the search.
    /// </summary>
    private IList<SymbolStyleItem> _symbols = new List<SymbolStyleItem>();
    public IList<SymbolStyleItem> Symbols
    {
      get
      {
        return _symbols;
      }
    }

    /// <summary>
    /// Selected symbol.
    /// </summary>
    private SymbolStyleItem _selectedSymbol = null;
    public SymbolStyleItem SelectedSymbol
    {
      get
      {
        return _selectedSymbol;
      }
      set
      {
        _selectedSymbol = value;
        ApplySelectedSymbol();
      }
    }

    #region Private methods
    
    /// <summary>
    /// Update the list of styles for the current project.
    /// </summary>
    private void UpdateStyleList()
    {
      if (Project.Current != null)
      {
        _styleProjectItems.Clear();

        //Update list using currently referenced styles in project
        var projectStyleContainer = Project.Current.GetItems<StyleProjectItem>();
        foreach (var style in projectStyleContainer)
        {
          _styleProjectItems.Add(style);
        }
        NotifyPropertyChanged(() => StyleProjectItems);
      }
    }

    /// <summary>
    /// Update the list of symbols given the current symbol geometry type, style, and search text.
    /// </summary>
    private async Task SearchForSymbols()
    {
      if (SelectedSymbolType == null || SelectedStyle == null)
      {
        _symbols.Clear();
      }
      else
      {
        //Get results and populate symbol gallery
        StyleItemType _itemTypeToSearch;
        if (_selectedSymbolType == "Line symbols") _itemTypeToSearch = StyleItemType.LineSymbol;
        else if (_selectedSymbolType == "Polygon symbols") _itemTypeToSearch = StyleItemType.PolygonSymbol;
        else _itemTypeToSearch = StyleItemType.PointSymbol;

        //Search for symbols in the selected style
        _symbols = await SelectedStyle.SearchSymbolsAsync(_itemTypeToSearch, _searchString);
      }
      NotifyPropertyChanged(() => Symbols);
    }

    /// <summary>
    /// Apply the selected symbol to the selected feature layers in the TOC whose geometry type match the symbol type.
    /// </summary>
    private Task ApplySelectedSymbol()
    {
      if (_selectedSymbol == null)
          return Task.FromResult(0);

      return QueuedTask.Run(() =>
      {
        //Get symbol from symbol style item. 
        var symbol = _selectedSymbol.Symbol;

        var mapView = MapView.Active;
        if (mapView == null)
          return;

        //Get the feature layer currently selected in the Contents pane
        var selectedLayers = mapView.GetSelectedLayers().OfType<FeatureLayer>().Where(l => IsMatchingShapeType(l, symbol));

        //Only one feature layer should be selected in the Contents pane
        if(selectedLayers.Count() == 0)
        {
          ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Select at least one feature layer in the TOC whose geometry type matches the symbol's geometry type.", "No Valid Layer Selected");
          return;
        }

        var symbolReference = SymbolFactory.MakeSymbolReference(symbol);
        foreach (var layer in selectedLayers)
        {
          //Create a simple renderer
          var renderer = layer.CreateRenderer(new SimpleRendererDefinition(symbolReference));
          layer.SetRenderer(renderer);

          ////Create a unique value renderer
          //var fieldDescription = layer.GetFieldDescriptions().FirstOrDefault(f => f.Type == ArcGIS.Core.Data.FieldType.String);
          //if (fieldDescription != null)
          //{
          //  var renderer = layer.CreateRenderer(new UniqueValueRendererDefinition(new string[1] {fieldDescription.Name}, symbolReference));
          //  layer.SetRenderer(renderer);
          //}
        }
      });
    }

    /// <summary>
    /// Test if the feature layer's geometry type matches the symbol type.
    /// </summary>
    private bool IsMatchingShapeType(FeatureLayer layer, CIMSymbol symbol)
    {
      if (symbol is CIMPointSymbol && layer.ShapeType == esriGeometryType.esriGeometryPoint)
        return true;
      if (symbol is CIMLineSymbol && layer.ShapeType == esriGeometryType.esriGeometryPolyline)
        return true;
      if (symbol is CIMPolygonSymbol && layer.ShapeType == esriGeometryType.esriGeometryPolygon)
        return true;
      return false;
    }

    /// <summary>
    /// Delegate method called when the OnProjectOpened event is published. 
    /// </summary>
    private void OnProjectOpened(ProjectEventArgs obj)
    {
      //Update the list of styles for the current project.
      UpdateStyleList();
    }

    #endregion
  }

        
  /// <summary>
  /// Button implementation to show the DockPane.
  /// </summary>
  internal class GalleryDockpane_ShowButton : Button
  {
    protected override void OnClick()
    {
      SymbolPickerViewModel.Show();
    }
  }

}
