## Lab 4: Working with the Core.Geometry and Core.Data API

#####In this lab you will learn how to
* Construct and manipulate geometries
* Use GeometryEngine functionality
* Search and retrieve features

********
* [Step 1: Create random point features](#step-1-create-random-point-features)
* [Step 2: Create polyline features from points](#step-2-create-polyline-features-from-points)
* [Step 3: Advanced geometry manipulation (optional)](#step-3-advanced-geometry-manipulation-optional)

**Estimated completion time: 30 minutes**

##Step 1: Create random point features
* Navigate in your local copy of arcgis-pro-sdk-workshop repo to this folder: C:\ProSDKWorkshop\arcgis-pro-sdk-workshop-2day-master\Labs\Day 2\Lab 4\Start
* Open the "GeometryEditingExercises" solution.
* Check the _References_ item in your solution explorer. In case the ArcGIS reference contain an exclamation mark, right-click the project _GeometryEditingExercises_ and click _Pro Fix References_ from the context menu.
* Open the _createPoints.cs_ file from the solution explorer.
* In the _OnClick()_ function for the button look for  ```// retrieve the first point layer in the map```

```c#
protected override void OnClick()
{
    // to work in the context of the active display retrieve the current map 
    Map activeMap = MapView.Active.Map;

    // retrieve the first point layer in the map
    FeatureLayer pointFeatureLayer = null;
    //var pointFeatureLayer = activeMap.GetLayersAsFlattenedList().OfType<FeatureLayer>().Where(
    //    lyr => lyr.ShapeType == ArcGIS.Core.CIM.esriGeometryType.esriGeometryPoint).First();

    // construct 20 random map point in this layer
    constructSamplePoints(pointFeatureLayer);
}
```

* Comment out the line where ```pointFeatureLayer``` is currently defined and uncomment the line below. From the _**Map**_ we are getting a collection of all map layers. "Flattened" means that the content of group layers is unraveled and presented in an enumerable container of layers. The code is generating a sub-list containing only layers of type _**FeatureLayer**_ where the type of geometry is _**esriGeometryPoint**_. Out of this list we are selecting the first layer.
* With the variable of ```pointFeatureLayer``` as the target continue to the ```private void constructSamplePoints(FeatureLayer pointFeatureLayer)``` function to construct the points.
* Change the method declaration to this:

```c#
private Task<bool> constructSamplePoints(FeatureLayer pointFeatureLayer) {
```

Note the _**Task**_. We are going to be implementing an asynchronous function.
* Find, and uncomment this statement in the constructSamplePoints method. Read **and understand** the comment.
	
```c#	
// the database and geometry interactions are considered fine-grained and must be executed on
// the main CIM thread
//return QueuedTask.Run (() =>
//{
```

* Find, and uncomment this statement at the bottom of the method. This completes the QueuedTask (QT) closure (and provides the correct return type for the function).

```c#
       // execute the operation
       //return createOperation.ExecuteAsync();
   //});
```

* Uncomment the following line to retrieve the extent (of the active map view) in which the random points will be generated.
	
```c#
//var areaOfInterest  = MapView.Active.Extent;
```

* Get the data source from the feature layer and cast it to a _**FeatureClass**_.
	
```c#	
// get the feature class associated with the layer
var featureClass = pointLayer.GetTable() as FeatureClass;
```

* Retrieve the schema definition for the feature class.
	
```c#
// retrieve the class definition of the point feature class
var classDefinition = featureClass.Definition as FeatureClassDefinition;
```

* Next store the spatial reference in its own variable ```spatialReference```.
	
```c#
// store the spatial reference as its own variable
var spatialReference = classDefinition.GetSpatialReference();
```

* Define an edit operation that will facilitate the creation of the point features. Edit operations are a logical unit that
	becomes part of the undo/redo stack.
	
```c#
// start an edit operation to create new (random) point features
var createOperation = new EditOperation();
createOperation.Name = "Generate points";
```

* Generate 20 point geometries using the provided extension method for the _**Random**_ class and add the create instruction to the stack of the edit operation.

```c#
// create 20 new point geometries and queue them for creation
for (int i = 0; i < 20; i++)
{
    MapPoint newMapPoint = null;

    // generate either 2D or 3D geometries
    if (classDefinition.HasZ())
        newMapPoint = MapPointBuilder.CreateMapPoint (randomGenerator.NextCoordinate(areaOfInterest, true), spatialReference);
    else
        newMapPoint = MapPointBuilder.CreateMapPoint (randomGenerator.NextCoordinate(areaOfInterest, false), spatialReference);

    // queue feature creation edit
    createOperation.Create(pointFeatureLayer, newMapPoint);
}
```

* Once the new features are queued as part of the edit operation, **execute** the operation itself.

```c#
// execute the edit (feature creation) operation
return createOperation.ExecuteAsync();
```

(Note, the return type is a boolean variable indicating if the edit operation was executed successfully.)

* Start a debug session and use the _WorkingWithGeometryAndData_ project. Activate the _ADD-IN TAB_ and from the _Geometry/Editing (Exercises)_ group click the left most button labeled _Create Points_.

* You should see points being generated in the current map extent of the active map.

##Step 2: Create polyline features from points
* Open the _createPolylines.cs_ file in Visual Studio solution explorer.
* Retrieve the point feature layer and the line feature layer from the active map. We will use the features and their respective geometries from the point layer as vertices for the polylines. Refer to the code in Step 1 on finding the layer.

Remember?:

```c#
protected override async void OnClick() {
   ...
   // retrieve the first point layer in the map
   FeatureLayer pointFeatureLayer = activeMap.GetLayersAsFlattenedList().OfType<FeatureLayer>()....
```

Hint: point layers have shape type ```ArcGIS.Core.CIM.esriGeometryType.esriGeometryPoint``` and polyline layers have shape type ```ArcGIS.Core.CIM.esriGeometryType.esriGeometryPolyline```.

* Continue to the _constructSamplePolylines_ function retrieving the features from the point layer and constructing the geometries for the polyline features.

* Be aware that the interactions for _**ArcGIS.Core.Geometry**_ and _**ArcGIS.Core.Data**_ are executed on the main CIM thread.

* As in step1, change the signature of the constructSamplePolylines() function to this (note the return ```Task<bool>```):

```c#
private Task<bool> constructSamplePolylines(FeatureLayer polylineLayer, FeatureLayer pointLayer)
```

* Uncomment the QT block and the return statement at the bottom of the function:

```c#
// execute the fine grained API calls on the CIM main thread
//return QueuedTask.Run (() => {

    // execute the edit (create) operation
    //return createOperation.ExecuteAsync();
//});
```

* Get the polyline feature class as the target for the new geometries and the point feature class from which to retrieve the geometries.

```c#
// get the underlying feature class for each layer
var polylineFeatureClass = polylineLayer.GetTable() as FeatureClass;
var pointFeatureClass = pointLayer.GetTable() as FeatureClass;

// retrieve the feature class schema information for the feature classes
var polylineDefinition = polylineFeatureClass.GetDefinition() as FeatureClassDefinition;
var pointDefinition = pointFeatureClass.GetDefinition() as FeatureClassDefinition;
```

* Get the features by creating a cursor on the point feature class. In our example we don't need to specify a _**QueryFilter**_ and use the null keyword to fill the cursor with all features.

```c#
var pointCursor = pointFeatureClass.Search(null, false);
```

* Use five point geometries in the construction of a polyline as outlined by the code. For each five point geometry use a list of coordinates to construct the polyline.
* Be aware that the spatial reference between the point feature layer and the line feature layer is different. Use the _**GeometryEngine**_ class for the re-projection of the geometry. Explore the code within the loop and change the code in the TODO section.

```c#
// TODO
// add the feature point geometry as a coordinate into the vertex list of the line
// - ensure that the projection of the point geometry is converted to match the spatial reference of the line
// HINT: use GeometryEngine.ProjectEx to project the point
// lineCoordinates.Add(((MapPoint)pointFeature.GetShape()).Coordinate);
```

* Execute the queued creates on the edit operation.
* Start a debug session and open the _WorkingWithGeometryAndData_ project. Navigate to the _ADD-IN TAB_ and click the _Create Points_ and _Create Polylines_ buttons in succession.

##Step 3: Advanced geometry manipulation (optional)

* In  the previous steps we covered using the geometry builders and the usage of the geometry engine. In this step we are using the basics to explore the geometry API a little further.

* Open the CreateGreenforTrees.cs file.

* The file contains a button implementation CreateGreenforTrees that calls a sketch tool CreateGreenSketchTool. Explore the code as it currently exists.

* Here is the goal for this exercise: Imagine we are designing a luscious green area for a park (a polygon). However the area already has trees (the points) planted and they cannot be disturbed by our new green. What does the suggested area for the green need to look like?

* Using the sketch tool with a lasso sketch type we are provided with the polygon for the green. The geometry of the polygon now needs to be modified in a way that there are cutouts around the point geometries/tree locations.

* Two potential ways to solve the problem:
1. Use the geometry engine to buffer the points and then subtract the buffer polygons from the polygon geometry.
2. Think of the polygon for the green as a collection of rings. Clockwise rings (with a positive value for area) are considered outer rings and counter-clockwise rings are inner rings or holes.

