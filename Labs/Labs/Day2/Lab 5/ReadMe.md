## Lab 5: Working with the Editing API

#####In this lab you will learn how to
* Create construction tools
* Use sketch geometries
* Modify feature attributes

********
* [Step 1: Construction tool template](#step-1-construction-tool-template)
* [Step 2: Use sketch geometry to perform edit](#step-2-use-sketch-geometry-to-perform-edit)
* [Step 3: Modify feature attributes (optional)](#step-3-modify-feature-attributes-optional)

**Estimated completion time: 30 minutes**

## Step 1: Construction tool template

* Navigate in your local copy of arcgis-pro-sdk-workshop repo to this folder: C:\ProSDKWorkshop\arcgis-pro-sdk-workshop-2day-master\Labs\Day 2\Lab 5\Start

* Open the "EditingExercises" solution in the Start folder.

* Open the _SimpleSketchTool.cs_ file in the solution explorer.

* The core ArcGIS Pro application provides a base class implementation for a _**MapTool**_. As learned in a previous lab the _MapTool_ base class provides the foundation for user interaction with the _MapView_. With ```IsSketchTool``` set to ```true``` the tool will provide a sketch feedback mechanism. Using the ```SketchType``` property we can set the type of geometry for the feedback.

* Explore the constructor for the tool.

```c#
public SimpleSketchTool()
{
    IsSketchTool = true;
    UseSnapping = true;
    // Select the type of construction tool you wish to implement.  
    // Make sure that the tool is correctly registered with the correct component category type in the daml 
    SketchType = SketchGeometryType.Point;
    // and the sketch geometry should be in map coordinates
    SketchOutputMode = ArcGIS.Desktop.Mapping.SketchOutputMode.Map;
}
```

* Once the user finishes the sketch on the screen the ```OnSketchCompleteAsync(Geometry geometry)``` method is called.

* Now we can setup the feature creation to an edit operation. Consider an edit operation as a logical unit to organize your edits. Within an edit operation you can have multiple edits. It is important that you organize your feature manipulation through edit operations such that the operations become part of the redo/undo stack.

```c#
// create an edit operation
var createOperation = new EditOperation();
```

* Next give the operation a meaningful title to clearly identify it in the undo/redo stack.

* Provide additional messages for ```ProgressMessage```, ```CancelMessage```, and ```ErrorMessage```.

* Queue the edit using the sketch geometry as the shape of the feature and any attribute configurations from the editing template.

```c#
// queue the edit using the sketch geometry as the shape of the feature and any attribute 
// configurations from the editing template
editOperation.Create(CurrentTemplate, geometry);
```

* Execute the edit operation. Note that we are using the return type of ```Task<bool>``` from the operation execution as a success indicator for the completed action of the sketch geometry.

* For our example we would like to place our construction in the context of point feature layers. This is done by associating the sketch tool with the point geometry type in the _Config.daml_ file.

```xml
<!-- the category refID and content are specifying the create feature section within ArcGIS Pro -->        
<tool id="EditingExercises_SimpleSketchTool" categoryRefID="esri_editing_construction_point"
  caption="SimpleSketchTool " className="SimpleSketchTool" loadOnClick="true" 
  smallImage="Images\GenericButtonRed16.png" largeImage="Images\GenericButtonRed32.png">
    <!--note: use esri_editing_construction_polyline,  esri_editing_construction_polygon for categoryRefID as needed-->
    <tooltip heading="Tooltip Heading">Tooltip text<disabledText /></tooltip>
</tool>
```

* Start a debug session and use the previously opened _WorkingWithGeometryAndData_ project. Activate (or maybe even open) the _Editing Samples_ map. Open the _Create Features_ pane. Click the _construction_points_ entry next to the graphics icon on the _Create Features_ pane. The new construction tool is the right most tool with the generic red button icon.

* Click the tool and digitize a handful of points.

* Stop the debug session.


## Step 2: Use sketch geometry to perform edit

* Open the _CutWithoutSelection.cs_ file. This idea for this tool is based on a previous ArcObjects example to demonstrate how this functinality can be achieved with the new API. Please be aware that this functionality is already part of the core ArcGIS Pro application and only serves as a code sample to familiarize you with the editing API.

* Familiarize yourself with the basic flow of the tool. We are creating a sketch and as such we are inheriting from _**MapTool**_. Examine the tool constructor to see what properties on the base class are set. The content should  be familiar from the previous step.

* With the complete custom workflow starting in the ```OnSketchCompleteAsync``` method we are executing the ```ExecuteCut``` method on the CIM thread.

* As the first steps in our method are setting up the edit operation.

```c#
// create an edit operation
EEditOperation op = new EditOperation();
op.Name = "Cut Elements";
op.ProgressMessage = "Working...";
op.CancelMessage = "Operation canceled.";
op.ErrorMessage = "Error cutting polygons";
op.SelectModifiedFeatures = false;
op.SelectNewFeatures = false;
```

* Next we are getting a collection of polygon layers we would like to use in our workflow. Since our tool works without a designated feature selection, we are using all editable polygon layers.

```c#
// create a collection of feature layers that can be edited
var editableLayers = MappingModule.ActiveMapView.Map.GetLayersAsFlattenedList().
    OfType<FeatureLayer>().Where(lyr => lyr.CanEditData() == true).
    Where(lyr => lyr.ShapeType == ArcGIS.Core.CIM.esriGeometryType.esriGeometryPolygon);

```

* For each of the layers, get the source feature class using the extension method on the layer.

```c#
// TODO
// get the feature class...HINT: use the extension method from the layer
FeatureClass fc = null;
```

* For each of the feature classes, perform a spatial query to get the features potentionally considered for the cut. Explore the extension method ```Search``` and complete the spatial filter with the provided arguments.

```c#
public static RowCursor Search(this Table searchTable, Geometry searchGeometry, SpatialRelationship spatialRelationship)
{
    RowCursor rowCursor = null;

    // TODO 
    // define a spatial query filter using the provided spatial relationship and search geometry
    SpatialQueryFilter spatialQueryFilter = new SpatialQueryFilter();

    // apply the spatial filter to the feature class in question
    rowCursor = searchTable.Search(spatialQueryFilter);

    return rowCursor;
}
```

* For cutting a feature the cut line itself is expected to completely _cross_ the feature, meaning that there are at least two intersection points. The code uses the GeometryEngine to perform an intersection and then to count the number of intersection points. If the number is larger than 1 we use the polygon for our edits. Uncomment the following lines.

```c#
//var intersectionPoints = intersectionGeometry as MultiPoint;
//// we are only interested in feature IDs where the count of intersection points is larger than 1
//// i.e., at least one entry and one exit
//if (intersectionPoints.Coordinates.Count > 1)
//{
//    // add the current feature to the overall list of features to cut
//    cutOIDs.Add(rc.Current.ObjectID);
//}
```

* Queue the edits cutting the completely crossed polygons described by the list of ```cutOIDS```.

```c#
// add the elements to cut into the edit operation
//op.Cut(..., cutOIDs, geometry);
```

* Open the _Config.daml_ file in the solution explorer and find the following section.

```xml
<tool id="EditingExercises_CutWithoutSelection" caption="Cut Without Selection " ...>
  <!-- add content -->
</tool>
```

* For the tool element itself set the following attribute ```categoryRefID="esri_editing_CommandList"```. This places our tool in the category of edit tools. Next add the content area to place the tool such that the complete declarative statement looks like this.

```xml
<tool id="EditingExercises_CutWithoutSelection" caption="Cut Without Selection" className="CutWithoutSelection"
loadOnClick="true" smallImage="Images\GenericButtonRed16.png" largeImage="Images\GenericButtonRed32.png"
categoryRefID="esri_editing_CommandList" >
  <tooltip heading="ArcGIS Pro SDK - Workshop">
    Sketch a line to cut underlying polygons without a feature selection.<disabledText /></tooltip>
  <content L_group="Workshop (Exercises)" gallery2d="true" />
</tool>
```

* Start a debug session and use the previously opened _WorkingWithGeometryAndData_ project. Activate (or maybe even open) the _Editing Samples_ map. Activate the _EDIT_ tab and click the _Modify_ button. The _Modify Features_ pane opens and you'll find a new section labeled _Workshop (Exercises)_ at the bottom containing our _Cut Without Selection_ tool. Click the tool and try a couple of cut lines against the polyon feature in the map view.

* Stop the debug session


## Step 3: Modify feature attributes (optional)

* Open the _UpdateAttributes.cs_ file. In this code sample you will change a feature attribute for a set of selected features.

* As in the previous examples you are using the _Construction Tool_ template as a starting point.

* Check in the _Config.daml_ file the tool with ```id="EditingExercises_UpdateAttributes"```.

* This tool has ```categoryRefID="esri_editing_CommandList"``` set meaning that it will be listed under _Modify Tools_. In addition it has the condition of ```esri_mapping_singleLayerSelectedCondition```; that is the tool will only become activated if there is only one layer selected.

```xml
<tool id="EditingExercises_UpdateAttributes" categoryRefID="esri_editing_CommandList" caption="Update Attributes"
className="UpdateAttributes" loadOnClick="true" smallImage="Images\GenericButtonRed16.png"
largeImage="Images\GenericButtonRed32.png" condition="esri_mapping_singleLayerSelectedCondition">
  <tooltip heading="ArcGIS Pro SDK - Workshop">
    Sample showing attribute editing.<disabledText /></tooltip>
  <content L_group="Workshop (Exercises)" gallery2d="true" />
</tool>
```

* Based on the condition you can be sure that you are only working against the selected layer. However you don't know what type of layer is selected. A selected raster layer could fullfil the condition as well. In the OnSketchCompleteAsync method, change the code below to ensure the variable ```featureLayer``` contains a valid reference to a feature layer.

```c#
// TODO - check if the selected layer is of type feature layer, if not then exit
// HINT - use the first item of MapView.Active.GetSelectedLayers()
FeatureLayer featureLayer = null;
```

* In the next step write code to ensure the feature layer contains a field of name _Comments_.

```c#
// TODO - check if the featurelayer contains a attribute field with the name "Comments". If not then exit.
// HINT - use featureLayer.GetFieldDescriptions()

```

* Using the sketch geometry you are selecting the features that are wholly within the sketch geometry.

```c#
// retrieve the features that intersect the sketch geometry
var features = MapView.Active.GetFeatures(geometry);
```

* Next you need to assemble a list of affected feature OIDs. You can take advantage of the fact you are operating against a single layer. The ```First()``` method will return a single list of OIDs for the layer.

```c#
var featureOIDs = features.Where(results => results.Key == featureLayer).Select(results => results.Value).First();
```

* You then load the list of object IDs into the inspector and assign a new value into the _Comment_ field. Please uncomment the corresponding lines of code as outlined below.

```c#
var featureInspector = new Inspector(true);
// load the feature by oid into the feature inspector
featureInspector.Load(featureLayer, featureOIDs);
// advise a new value for the field named "Comment"
featureInspector["Comment"] = "ArcGIS Pro SDK Sample";
```

* Using another edit operation you are committing the changes to the database. Use the modify method in combination with the inspector object and then execute the operation.

```c#
//modifyOperation.Modify(...);
```

* Start a debug session and use the previously opened _WorkingWithGeometryAndData_ project. Activate (or maybe even open) the _Editing Samples_ map. Activate the _EDIT_ tab and click the _Modify_ button. The _Modify Features_ pane opens. You'll find a new section labeled _Workshop (Exercises)_ at the bottom containing our _Update Attributes_ tool. Notice how it is disabled.  Highlight the _construction_points_ layer in the TOC and see the tool enable. Click the tool, sketch a rectangle around a point and then inspect the attributes of the selected features.

* Stop the debug session

