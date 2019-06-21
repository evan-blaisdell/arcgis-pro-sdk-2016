##Lab 3: Geodatabase

#####In this lab you will learn how to
* Open a geodatabase and feature classes
* Add attachments 
* Use a query filter for retrieving features

*******
* [Step 1: Add attachments to features](#step-1-add-attachments-to-features)
* [Step 2: Use a query filter for feature selection](#step-2-use-a-query-filter-for-feature-selection)

**Estimated completion time: 30 minutes**
****

####Step 1: Add attachments to features
* Navigate in your cloned arcgis-pro-sdk-workshop repo to this folder:
C:\ProSDKWorkshop\arcgis-pro-sdk-workshop-2day-master\Labs\Day 2\Lab 3\Start.

* Open the "GeodatabaseExercises" solution in the Start folder.

* Check the _References_ item in your solution explorer. In case the ArcGIS reference contain an exclamation mark, right-click the project _GeodatabaseExercises_ and click _Pro Fix References_ from the context menu.

* Open the _OpenDataButton.cs_ file from the solution explorer.

* Note in the _OnClick_ method that you are setting up the following code to be executed on the main CIM thread.

```c#
QueuedTask.Run(() => GetFeaturesAndAddAttachment());
```

* Move down to the GetFeaturesAndAddAttachment method.  We will be adding code here.

* Open the file geodatabase located in _c:\ProSDKWorkshop\data\generic.gdb_. Remember to free unmanaged resources after usage with either the using statement or calling 'Dispose()' explicitly.

```c#
// TODO
// open the file geodatabase c:\ProSDKWorkshop\data\generic.gdb
using (Geodatabase geodatabase = new Geodatabase(@"c:\ProSDKWorkshop\data\generic.gdb"))
{

}
```

* Next open the feature class _SamplePoints_. Place the following code inside the previous using statement.

```c#
using (FeatureClass pointFeatureClass = geodatabase.OpenDataset<FeatureClass>("SamplePoints"))
{

}
```

* Now with access to the feature class, retrieve all features by calling the 'Search' method on the feature class. You don't need to specifiy any search arguments as we want to retrieve all the features from the feature class. Place the following code inside the previous using statement.

```c#
using (RowCursor features = pointFeatureClass.Search())
{ 

}
```

* The 'Search' function returns a 'RowCursor' providing access to the results. The row cursor can only advance sequentially forward. If the cursor has successfully advanced by calling 'MoveNext' the 'Current' property points to the current row within the cursor. Since the 'Feature' class derives from 'Row' the current row for a feature class can be cast to a 'Feature'.

```c#
while (features.MoveNext())
{
    Feature currentFeature = features.Current as Feature;
}
```
Your current ```GetFeaturesAndAddAttachment``` code should look like this (with or without TODO comments)

```c#
using (Geodatabase geodatabase = new Geodatabase(@"c:\ProSDKWorkshop\data\generic.gdb"))
{
    using (FeatureClass pointFeatureClass = geodatabase.OpenDataset<FeatureClass>("SamplePoints"))
    {
        using (RowCursor features = pointFeatureClass.Search())
        {
            while (features.MoveNext())
            {
                Feature currentFeature = features.Current as Feature;
            }
        }
    }
}
```

* Now add the following line of code within the ```while (features.MoveNext())``` loop after the currentFeature declaration to add the attachment to the current feature.

```c#
//// add the sample picture as an attachment
currentFeature.AddAttachment(new Attachment("SamplePicture", "image/png",
    CreateMemoryStreamFromContentsOf(@"C:\ProSDKWorkshop\data\redlands.png")));
```

Your while loop should look like this
```c#
while (features.MoveNext())
{
    Feature currentFeature = features.Current as Feature;
    currentFeature.AddAttachment(new Attachment("SamplePicture", "image/png",
          CreateMemoryStreamFromContentsOf(@"C:\ProSDKWorkshop\data\redlands.png")));
}
```

The ```CreateMemoryStreamFromContentsOf``` method has been provided.

* At the conclusion of the ```GetFeaturesAndAddAttachment``` method, the altered &quot;SamplePoints&quot; feature class is added to the current map as a feature layer by the LayerFactory.

```c#
  // add the feature class as a layer to the active map
  LayerFactory.CreateFeatureLayer(new Uri(@"c:\ProSDKWorkshop\data\generic.gdb\SamplePoints"), MapView.Active.Map);
```

* Build the solution and run it. It should open the Geodatabase project at C:\ProSDKWorkshop\Data\Projects\Geodatabase.

* Place breakpoints in the ```GetFeaturesAndAddAttachment``` method if you want to debug the code and investigate it.

* Navigate to the _ADD-IN_ tab and click the _Open Data_ button.

* Once the layer is added, zoom to the extent of the layer and click the points using the explore tool to show a pop-up and see the attachments.

* Close the Pro application or stop debugging.

####Step 2: Use a query filter for feature selection
* Open the _WatermainPaneViewModel.cs_ file from the solution explorer.

* In this exercise you are programmatically adding two query definitions to the layer in the map. The results of the query are displayed in a data grid inside a dock pane.

* At the beginning of the class you'll see a dictionary that references the type of queries associated with the name of the layer.

```c#
// build a dictionary containing the name of the layer and the names of the associated queries
Dictionary<string, List<WatermainQuery>> queryMap = new Dictionary<string, List<WatermainQuery>>
{
    {"Water Mains Life Expectancy", 
        new List<WatermainQuery>{ new UnkownMaterialQuery{Name = "Unknown Material"}, 
            new ExcellentCastIronQuery{Name = "Iron in excellent condition"} 
        }
    }
};
```

* The queries are implemented as classes containing the query filter to return only certain features. 

* Look for the `UnkownMaterialQuery` and the `ExcellentCastIronQuery` classes at the end of the _WatermainPaneViewModel.cs_ file and uncomment the lines for the `QueryFilter` properties. Explore the properties that are specified. 

```c#
internal class UnkownMaterialQuery : WatermainQuery
{
...
  var queryFilter = new QueryFilter
  {
      // uncomment lines below
      //WhereClause = "SCORE = 0",
      //PrefixClause = "DISTINCT",
      //PostfixClause = "ORDER BY FACILITYID",
      //SubFields = "FACILITYID,REMLIF,SCORE,MATERIAL"
  };

...

internal class ExcellentCastIronQuery : WatermainQuery
{
..
    var queryFilter = new QueryFilter
    {
        // uncomment lines below
        //WhereClause = "SCORE = 1 AND MATERIAL = 'CAS'",
        //PrefixClause = "DISTINCT",
        //PostfixClause = "ORDER BY REMLIF",
        //SubFields = "FACILITYID,REMLIF,SCORE,MATERIAL"
    };
```

* Locate the `WatermainQuery` class. Its `PopulateWatermainData` method is used to execute the filter against the feature class.

* Examine the following section within the ```PopulateWatermainData``` method and uncomment the lines. Here you are reading the attribute values from each of the selected rows from the ```RowCursor``` and storing them into a custom data type, ```WaterMainData```, that will subsequently be stored in the `FeatureData` property of our view model. `FeatureData` is bound to a WPF `DataGrid` inside the dockpane.

```c#
// Uncomment the lines below 
// read the feature attribute values and use them initialize the water main instance
//FacilityID = Convert.ToString(current["FACILITYID"]),
//RemainingLifeExpectancy = Convert.ToInt32(current["REMLIF"]),
//RelativeScore = Convert.ToString(current["SCORE"]),
//PipeMaterial = Convert.ToString(current["MATERIAL"])
```

* Build the solution and run it. 

* Navigate to the _ADD-IN_ tab and click the _Show Watermain Pane_ button.

* Once the dock pane is open, select a layer and then select one of the queries. Change between queries to see the values in the datagrid change.

* Look for the ```RetrieveData``` method in _WatermainPaneViewModel.cs_. Notice how the function retrieves the stored query class based on the currently selected layer.

* ```Execute``` is called on the retrieved query class which executes the stored query filter on the selected layer's table.

```c#
public void RetrieveData(FeatureLayer selectedLayer)
        {
            if (selectedLayer == null)
            {
...
...
FeatureData = new ObservableCollection<WaterMainData>
      (queryMap[selectedLayer.Name].First(
        query => query.Name.Equals(SelectedQuery))
                   .Execute(table));
```

* Place a breakpoint in the ```PopulateWatermainData``` method of the `WatermainQuery` base class to debug the query filter execution.

```c#
internal abstract class WatermainQuery 
    {
        //
        public abstract List<WaterMainData> Execute(Table table);
        public string Name { get; set; }

        protected List<WaterMainData> PopulateWatermainData(Table table, QueryFilter queryFilter)
        {
            var list = new List<WaterMainData>();
            using (RowCursor rowCursor = table.Search(queryFilter, false))
            {
             ...

```

* Examine the _WatermainPane.xaml_. See how the ```DataGrid``` ItemsSource is bound to the view model ```FeatureData``` property. As mentioned in the discussion of ```RetrieveData``` and ```PopulateWatermainData```, the FeatureData property is updated every time a layer is selected and its stored query executes.

```xml
<DataGrid  Height="Auto" Grid.Row="2" ItemsSource="{Binding FeatureData}" Width="Auto" ScrollViewer.VerticalScrollBarVisibility="Auto"/>
```

* Close the Pro application or stop debugging
