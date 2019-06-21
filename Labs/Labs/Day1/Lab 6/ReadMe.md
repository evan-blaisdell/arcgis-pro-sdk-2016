##Lab 6: EsriHttpClient

#####In this lab you will learn how to
* Get the URL of the Active Portal
* Initialize EsriHttpClient
* Retrieve Content from ArcGIS Online
* Add Content from ArcGIS Online to your Map

You will be using EsriHTTPClient to query ArcGIS Online for Feature Service content. You will be adding a FeatureService to your Map.

*******
Note: for this lab you will need a reference to Newtonsoft.Json library. If you do not have this assembly on your desktop you will need to download it. The simplest way is to use the [NuGet Package Manager](http://docs.nuget.org/consume/installing-nuget). This is a free download within the Visual Studio Extensions and Updates dialog. Otherwise simply download the latest release of Newtonsoft from [here](https://github.com/JamesNK/Newtonsoft.Json/releases).


* [Step 1: Configure your Project](#step-1-configure-your-project)
* [Step 2: Add EsriHTTPClient and Configure the REST request](#step-2-add-esrihttpclient-and-configure-the-rest-request)
* [Step 3: Parse the Results](#step-3-parse-the-results)
* [Step 4: Add One of the Results to the Map](#step-4-add-one-of-the-results-to-the-map)
* [Step5: Execute your Add-in](#step-5-execute-your-add-in)

**Estimated completion time: 20 minutes**
****

####Step 1: Configure your Project
* Open the EsriHttpClientDemo solution in the `Labs\Day1\Lab 6` folder. Take a minute to look at the solution. See that there is an empty button called AddLayer. Check the config.daml file to see how the button is configured.

* Add a reference to Newtonsoft.Json. You will need to Browse to the location of the dll if you downloaded it from the Newtonsoft.git. Otherwise, use the `Manage NuGet Packages...` menu item on the Project Context menu.

After you have added the reference, add the following using statement to AddLayer.cs
`using Newtonsoft.Json.Linq;`

####Step 2: Add EsriHTTPClient and Configure the REST request

* In AddLayer.cs, retrieve the URL for the active portal. Notice the line `string portalUrl = "";`. Populate it using the Portal Manager (hint: `PortalManager.GetActivePortal()`)

* Use EsriHTTPClient's Get method to execute the provided REST query (hint: `httpClient.Get(requestUri)`)

* Get returns a `EsriHttpResponseMessage`. Use the `EsriHttpResponseMessage` to retrieve the JSON content of the response. Hint, examine the EsriHttpResponseMessage `.Content` property. It has methods to retriieve the response JSON.

####Step 3: Parse the Results

* Use the Newtonsoft.Json `JObject.Parse()` method. Use a dynamic type to get the parsed results. Hint: you code should look something like this `dynamic resultItems = JObject.Parse(....);`

* Create a list of results from the parsed JSON. Note: the type of the items will be `dynamic`. Add the `resultItems.results` to the list. Your code should look something similar to this:

```c#
dynamic resultItems = JObject.Parse(...

List<dynamic> resultItemList = new List<dynamic>();
resultItemList.AddRange(resultItems.results);

```

####Step 4: Add One of the Results to the Map

* Get one of the result Items from the list of results. Remember, it will be of type `dynamic`

* Get the item id from the result item. `string itemID = item.id;`

* Use the ItemFactory class to create an `Item` using the item id. The type to use for the factory create method will be `ItemFactory.ItemType.PortalItem`.

* Within the `QueuedTask.Run` anonymous delegate. use the `LayerFactory.CreateLayer` to create a layer and add it to the `MapView.Active.Map`.

####Step 5: Execute your Add-in

* Execute the Add-in.

* Create a New project using the Map.aptx template.

* The Add layers button will be on the Add In Tab. Click on the button and debug your code.

* Stop the debugger.
