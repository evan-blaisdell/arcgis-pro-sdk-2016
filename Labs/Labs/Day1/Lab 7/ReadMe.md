##Lab 7: Working with Layer and Renderer Definitions

#####In this lab you will learn how to
* Get the styles from the project
* Search for symbols in a style
* Apply a renderer to a feature layer using a given symbol

*******
* [Step 1: Get styles for the current project](#step-1-get-styles-for-the-current-project)
* [Step 2: Search for and display symbols that match search crieria](#step-2-search-for-and-display-symbols-that-match-search-crieria)
* [Step 3: Apply the symbol to the selected layers in the map](#step-3-apply-the-symbol-to-the-selected-layers-in-the-map)
* [Step 4: Apply a unique value renderer using the selected symbol (bonus)](#step-4-apply-a-unique-value-renderer-using-the-selected-symbol-bonus)

**Estimated completion time: 30 minutes**
****

####Step 1: Get styles for the current project
* Navigate in your copy of arcgis-pro-sdk-workshop repo to this folder:
C:\ProSDKWorkshop\arcgis-pro-sdk-workshop-2day-master\Labs\Day 1\Lab 7\Start

* Open the "CustomSymbolPicker" solution in the Start folder.

* The add-in contains a dock pane that will be used to search for symbols and apply a symbol to the selected layers in the map.

* Open the SymbolPickerViewModel.cs file, this is the view model for the dock pane.

* First we need to add the implementation to the UpdateStyleList method. This is the method we will call when we need to refresh the list of styles for the current project. This list is bound to the itemsource property of a combo box in the view. Which will allow us to pick a style from the project. The code needs to first check if the current project is not null and if it is not we need to get the styles project items and add them to the StyleProjectItems collection. The code should look something like this: 

```c#
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
```

* Next we need to call this method when the pane is constructed and when a project is opened to ensure the list reflects the current project. Call this method within the view model's constructor.

* In addition, we also need to subscribe to the ProjectOpenedEvent within the constructor and call UpdateStyleList whenever this event is published to update the list for the new project.

```c#
ProjectOpenedEvent.Subscribe(OnProjectOpened);
```

* Finally, call UpdateStyleList within OnProjectOpened.

* Additionally we would likely want to subscribe to the ProjectItemsChangedEvent to update our list if a style was added or removed from the project. We will not worry about this in this exercise.

* Build the add-in and run it. This should open ArcGIS Pro and the Interacting with Maps project.

* When the project opens go to the add-in tab and click the Custom Symbol Picker button. This should open the dock pane. Open the first combo box, it should display all the styles referenced by the current project. This was populated by the code we just completed above. Now open the second combo box, this contains 3 items for the type of symbol, Point, Line and Polygon. These are hardcoded values set by the SymbolTypes property in the view model.

####Step 2: Search for and display symbols that match search crieria

* Next we want to search and display symbols that are within the selected style, are the selected symbol type, and match any search text the user passes in.
 
* In the SymbolPickerViewModel we need to add behavior to the SearchForSymbols method. This method should first check whether the SelectedSymbolType or SelectedStyle are null. This would mean that either of the first 2 combo boxes have no selection and as a result we should clear the collection of symbols. 

```c#
if (SelectedSymbolType == null || SelectedStyle == null)
{
  _symbols.Clear();
}
```

* If they are both not null, we can do a search for symbols. StyleProjectItem provides a SearchSymbolsAsync method that takes 2 parameters, a StyleItemType and a search string. StyleItemType is an enumeration that contains values like PointSymbol, LineSymbol, etc. Based on the users selection in the first combo box we need to select the correct StyleItemType. 

```c#
else
        {
            //Get results and populate symbol gallery
            StyleItemType _itemTypeToSearch;
            if (_selectedSymbolType == "Line symbols") 
                _itemTypeToSearch = StyleItemType.LineSymbol;
            else 
                if (_selectedSymbolType == "Polygon symbols") 
                    _itemTypeToSearch = StyleItemType.PolygonSymbol;
            else _itemTypeToSearch = StyleItemType.PointSymbol;
        }
```

* There is also another property in the view model call SearchString that will contain the text we can pass in as the second parameter to this method. Call the SearchSymbolsAsync method and set the result to the _symbols backing field. This is the backing field for the Symbols property which the view's ListBox ItemSource property is bound to. 

```c#
//Search for symbols in the selected style
_symbols = await SelectedStyle.SearchSymbolsAsync(_itemTypeToSearch, _searchString);
```

* Finally we need to call NotifyPropertyChanged and pass in the Symbols property to notify the binding that the collection of symbols has been updated. This complete code should look something like below:

```c#
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
```

* Now we need to call this method anytime the selected symbol type, selected style, or search text are changed. To do this call SearchForSymbols inside the setter for SelectedSymbolType, SelectedStyle, and Search string.
 
* You may see a warning because the call is not awaited execution of the current method continues before the call is completed. This is because the SearchSymbol returns a Task. In this case we don't need to await the method so we can ignore the warning.

* Build the solution and run it.

* In the dock pane select a symbol type and a style. The list box should refresh with the symbols of the selected type for the selected style. Enter some text into the search text box. The results should update to reflect symbols whose names or tags match the search text.

* When you've finished exploring the functionality, close ArcGIS Pro and return to Visual Studio.

####Step 3: Apply the symbol to the selected layers in the map

* Finally, the last step is we want to be able to select a symbol in our pane and apply it to the selected layers in the TOC.

* Find the ApplySelectedSymbol method.

* The first check we need to add is to test whether the _selectedSymbol property is null, if it is we should just return from the method.

```c#
if (_selectedSymbol == null)
    return Task.FromResult(0);
```

* Within QueuedTask.Run first get the CIMSymbol from _selectedSymbol using the Symbol property.

```c#
return QueuedTask.Run(() =>
{
  //Get symbol from symbol style item. 
  var symbol = _selectedSymbol.Symbol;
```

* Next we need to get the active map view using the MapView.Active static property. If it is null we should return from the method.

```c#
var mapView = MapView.Active;
if (mapView == null)
  return;
```

* MapView contains a method GetSelectedLayers which returns the collection of layers that are selected in the TOC. From this collection we want to get only the Feature Layers whose geometry type matches the symbol type. We can use a linq expression like below to do this. In the example below the IsMatchingShapeType is a helper method to test if the geometry types match and returns a boolean. 

```c#
var selectedLayers = mapView.GetSelectedLayers().OfType<FeatureLayer>().Where(l => IsMatchingShapeType(l, symbol));
```

* If the collection of layers returned is empty we should show a message box letting the user know that there are no matching selected layers and return from the method.

```c#
if(selectedLayers.Count() == 0)
{
  ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Select at least one feature layer in the TOC whose geometry type matches the symbol's geometry type.", "No Valid Layer Selected");
  return;
}
```

* Finally we need to create a new simple renderer using the symbol and set the renderer on the layer.

```c#
var symbolReference = SymbolFactory.MakeSymbolReference(symbol);
foreach (var layer in selectedLayers)
{
  var renderer = layer.CreateRenderer(new SimpleRendererDefinition(symbolReference));
  layer.SetRenderer(renderer);
}
```
* The full code will look like the following:

```c#
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
    var renderer = layer.CreateRenderer(new SimpleRendererDefinition(symbolReference));
    layer.SetRenderer(renderer);
  }
});
```

* The last step is to call ApplySelectedSymbol in the setter for the SelectedSymbol property.

* Build the solution and run it. Try selecting some layers and selecting symbols in the dock pane and verify that the renderer is updated correctly using the new symbol.

####Step 4: Apply a unique value renderer using the selected symbol (bonus)

* Try creating a unique value renderer for the layer using the selected symbol. (Hint there is a snippet in the Map Authoring snippets that shows how to create and apply a unique value renderer.)

* Use the first string field as the field for the unique value renderer. (Hint take a look at the GetFieldDescriptions method off of the layer.)
