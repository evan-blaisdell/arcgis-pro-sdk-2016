##Lab 1: Create a custom map tool

#####In this lab you will learn how to
* Add a custom sketch tool to work with map views
* Create a custom identify tool for 2D and 3D
* Select and zoom to features in the map

*******
* [Step 1: Add a custom identify tool](#step-1-add-a-custom-identify-tool)
* [Step 2: Show the identify results in an overlay control](#step-2-show-the-identify-results-in-an-overlay-control)
* [Step 3: Select and zoom to features (bonus)](#step-3-select-and-zoom-to-features-bonus)

**Estimated completion time: 20 minutes**
****

####Step 1: Add a custom identify tool
* Navigate in your cloned arcgis-pro-sdk-workshop repo to this folder: C:\ProSDKWorkshop\arcgis-pro-sdk-workshop-2day-master\Labs\Day 2\Lab 1\Start.

* Open the "SketchTool" solution in the Start folder.

* In Visual Studio right-click the SketchTool project and select add new item.

* Browse to the ArcGIS Pro templates and select the ArcGIS Pro Sketch Tool template. Name the file CustomIdentify.cs and click Add to add the tool.

* Examine the CustomIdentify.cs. The class inherits from MapTool and by default sets properties in the constructor.

* IsSketchTool sets whether the default behavior of left click should be to create a sketch.

* SketchType sets the shape of geometry to create.

* SketchOutputMode sets whether the geometry is in map coordinates or screen coordinates.

* **Change the SketchOutputMode to Screen**. 3D only supports selection and identify using geometries in screen coordinates. 2D supports both. So to work in both 2D and 3D we should use screen
```
public CustomIdentify() {
    IsSketchTool = true;
    SketchType = SketchGeometryType.Rectangle;
    SketchOutputMode = SketchOutputMode.Screen ;
}
```

* The OnSketchCompleteAsync method will be called after you finish a sketch with the tool and it will pass in the geometry from the sketch as a parameter. This is where we will add the behavior to the tool.

* MapView has a method GetFeatures which takes a geometry and returns a Dictionary of Layers and ObjectIDs that represents the features that intersected the geometry.

* The result of a GetFeatures can be passed to the FlashFeature method off of MapView.

* Replace the default implementation of OnSketchCompleteAsync with the code below. This adds behavior to the tool to flash the features that intersect the sketch geometry.

```c#
    protected override Task<bool> OnSketchCompleteAsync(Geometry geometry)
    {
      return QueuedTask.Run(() =>
      {
        //Get the features that intersect the sketch geometry
        var result = MapView.Active.GetFeatures(geometry);
        MapView.Active.FlashFeature(result);
        return true;
      });
    }
```

Note: add a using statement for ArcGIS.Desktop.Framework.Threading.Tasks to the top of the class file.
```
using ArcGIS.Desktop.Framework.Threading.Tasks;
```

####Step 2: Show the identify results in an overlay control.

* In addition to flashing the features, we are going to use an overlay control on the map view to show the identify results on the screen.

* A new embeddable control has been declared in the config.daml for you. Embeddable Controls can be used in a variety of ways by the application, and in this case we will use the control to provide the overlay on top of the map view. Embeddable controls follow the MVVM patern with a View that inherits from UserControl and a ViewModel that inherits from EmbeddableControl. Notice that both the View ```EmbeddedControl.xaml``` and the View Model ```EmbeddedControlViewModel.cs``` have been added to the solution.

* Locate the following code into your Addin config.daml. It updates the ```esri_embeddableControls``` category with a new embeddable control. It references both the View and View Model in the daml. 

```xml
  <categories>
    <updateCategory refID="esri_embeddableControls">
      <insertComponent id="SketchTool_EmbeddableControl" className="EmbeddedControlViewModel">
        <content className="EmbeddedControlView" />
      </insertComponent>
    </updateCategory>
  </categories>
```

* Locate the User Control ```EmbeddedControl.xaml``` in your solution explorer. Examine the XAML definition.

* Locate the View Model ```EmbeddedControlViewModel.cs``` in your solution explorer. Examine the implementation.

* In the sketch tool constructor set the tool's OverlayControlID property to the DAML ID of the Embeddable Control.

```c#
    public CustomIdentify()
    {
      IsSketchTool = true;
      SketchType = SketchGeometryType.Rectangle;
      SketchOutputMode = SketchOutputMode.Screen;
	  //Set the tool's OverlayControlID property to the DAML ID of the Embeddable Control
      OverlayControlID = "SketchTool_EmbeddableControl";
    }
```

* Now in OnSketchComplete we need to get the instance of the Embeddable Control's view model and populate it with the identify results.

* Modify your existing OnSketchCompleteAsync implementation to look like this: Note the new code at the end of the method that retrieves an instance of the ```OverlayEmbeddableControl``` and casts it to ```EmbeddedControlViewModel``` and calls ```UpdateResults```.

```c#
    protected override Task<bool> OnSketchCompleteAsync(Geometry geometry)
    {
      return QueuedTask.Run(() =>
      {
        //Get the features that intersect the sketch geometry
        var result = MapView.Active.GetFeatures(geometry);
        MapView.Active.FlashFeature(result);
        
        //Get the instance of the overlay control view model
        var vm = this.OverlayEmbeddableControl as EmbeddedControlViewModel;
        if (vm == null)
          return true;

        //Update the results in the view model
        vm.UpdateResults(result);
        return true;
      });
    }
```

* Build the add-in and run it. This should open ArcGIS Pro and the &quot;C:\ProSDKWorkshop\Data\Projects\Interacting with Maps\Interacting with Maps.aprx&quot; project.

* On the Sketch tab activate the tool and try the tool in 2D and 3D. Don't forget that the sketchType is defined as Rectangle, so click and drag a rectangle on the map. After you identify the results should be populated in the overlay control and you should be able to click the items in the control and flash the associated features.
 
####Step 3: Select and zoom to features (bonus)

* Instead of flashing the features when they are selected let's select the features in that layer and zoom to the selection.

* Open the EmbeddedControlViewModel.cs and look at the setter for the SelectedResult property.

* This is where we are currently flashing the features when we **select an item in the overlay control**. Comment the code out associated with flashing the feature.

```
  set
  {
    SetProperty(ref _selectedResult, value, () => SelectedResult);
    var mapView = MapView.Active;
    if (mapView != null && _selectedResult != null)
    {
      //Flash the associated features in the selected result.
      //var result = new Dictionary<BasicFeatureLayer, List<long>>();
      //result.Add(_selectedResult.Layer, _selectedResult.OIDs);
      //mapView.FlashFeature(result);
    }
  }
```

* Instead inside of a QueuedTask we need to first clear the map selection. This can be accomplished by calling SetSelection off of the map and passing in null. Add a QueuedTask.Run into the setter.

```c#
if (mapView != null && _selectedResult != null)
{
  ////Flash the associated features in the selected result.
  //var result = new Dictionary<BasicFeatureLayer, List<long>>();
  //result.Add(_selectedResult.Layer, _selectedResult.OIDs);
  //mapView.FlashFeature(result);
  
    QueuedTask.Run(() =>
    {
        mapView.Map.SetSelection(null);
    });
 }
```

* Next from _selectedResult.Layer property we need to get its ObjectID field. We can find this by getting the Table from the layer, and then the defintion from the table.

```c#
var oidField = _selectedResult.Layer.GetTable().GetDefinition().GetObjectIDField();
```

* Using the ObjectID field we can construct a query filter where clause which will be used to select all the features in the layer who match the list of ObjectIDs in the selection result.

```c#
var qf = new ArcGIS.Core.Data.QueryFilter() { WhereClause = string.Format("({0} in ({1}))", oidField, string.Join(",", _selectedResult.OIDs)) };
```

* From the layer we can then call the Select method and pass in the query filter to select the features that satisfy the filer.

```c#
_selectedResult.Layer.Select(qf);
```

* Finally we can zoom to the selected features and pass in a timespan to set how long to take to navigate to the selected features.

```c#
mapView.ZoomToSelectedAsync(TimeSpan.FromSeconds(1));
```

The complete code will look something like below:

```c#
set 
{
SetProperty(ref _selectedResult, value, () => SelectedResult);
var mapView = MapView.Active;
if (mapView != null && _selectedResult != null)
{
  ////Flash the associated features in the selected result.
  //var result = new Dictionary<BasicFeatureLayer, List<long>>();
  //result.Add(_selectedResult.Layer, _selectedResult.OIDs);
  //mapView.FlashFeature(result);

  QueuedTask.Run(() =>
  {
    //Clear the existing selection in the map.
    mapView.Map.SetSelection(null);

    //Construct a query filter using the OIDs from the selected result.
    var oidField = _selectedResult.Layer.GetTable().GetDefinition().GetObjectIDField();
    var qf = new ArcGIS.Core.Data.QueryFilter() { WhereClause = string.Format("({0} in ({1}))", oidField, string.Join(",", _selectedResult.OIDs)) };

    //Create a new selection using the query filter.
    _selectedResult.Layer.Select(qf);

    mapView.ZoomToSelectedAsync(TimeSpan.FromSeconds(1));
  });
}
```

* Build the add-in and run it. This should open ArcGIS Pro and the Interacting with Maps project.

* On the Sketch tab activate the tool and try the tool in 2D and 3D. After you identify the results should be populated in the overlay control. Now when you click the items entry in the overlay control those features should be selected and the view should zoom to those features.
