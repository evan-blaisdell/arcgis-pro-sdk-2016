```
You may not distribute copies of the content of this repository.
COPYRIGHT 2016 ESRI

TRADE SECRETS: ESRI PROPRIETARY AND CONFIDENTIAL
Unpublished material - all rights reserved under the
Copyright Laws of the United States and applicable international
laws, treaties, and conventions.

For additional information, contact:
Environmental Systems Research Institute, Inc.
Attn: Contracts and Legal Services Department
380 New York Street
Redlands, California, 92373
USA

email: contracts@esri.com

```


## ArcGIS Pro SDK for .NET Palm Springs 2016 Dev Summit Workshop Agenda

Sunday, March 6th - Monday, March 7th, Hard Rock Hotel, Woodstock Room, 150 S Indian Canyon Dr, Palm Springs, CA 92262

### Prerequisites

* All attendees must bring their own laptops   
* All attendees must have an ArcGIS account with access to ArcGIS Pro or an ArcGIS Pro Trial Account
* All attendees must have Visual Studio 2013 or Visual Studio 2015 installed
  *  VS 2013: Community, Professional, Premium, or Ultimate - Update 5 or later recommended
  *  VS 2015: Professional, Enterprise, and Community Editions - Update 1 recommended
* All attendees must have *ArcGIS Pro 1.2* installed 
* All attendees must have *ArcGIS Pro SDK for .NET 1.2* installed

###Login Information

**Login to GitHub**   
* Browse to the [Esri Organization on GitHub](https://github.com/Esri/).
* Click the "Sign In" button in the top right.
* Use the following credentials to log in:  

```
username: sdkstudent 
password: sdk.student2015 
```

## Day 1

### 8:00 Doors open. 

Check machines, software configuration, installs, etc.

### 08:45 Welcome  
* Introductions and overview of the workshop goals  
* Overview of, and familiarization with the Pro SDK  
 
### 09:00 Lecture 1: Introduction to the ArcGIS Pro SDK  
* Introduction to Pro Add-ins and the Module
* Pro SDK Fundamentals
   * Module
   * DAML and declarative UIs
      * Customize the Pro UI
   * Conditions and States
   * Delegate commands

* Morning **Lab1**: Working with DAML
* Morning **Lab2**: Conditions and States using a Delegate Command
   
### 10:30 Lecture 2: Customization Patterns

   * Coarse grained and Fine grained APIs
   * Events
   * Hooking Commands
   * MVVM

* **Lab3**: MVVM
* **Lab4**: Coarse and Fine grained functions

Note: Lunch is on your own. Please take lunch anytime between 12 noon and lecture start at 13:30.

### 13:30 Lecture 3: Project Content and Items  
 * Project
   * Project methods and properties
 * Project Content
   * Items and ProjectItem
   * Browse and Search
 * EsriHttpClient and Portal Integration 
   
* **Lab5**: Project Browse Dockpane
* **Lab6**: EsriHttpClient

### 15:00 Lecture 4: Introduction to Map and Layer  
* Map
   * Properties, Create, Open
* Layer
   * Layer creation and LayerFactory
   * Layer properties
   * Layer cache and cache management
* Feature Layer
   * Datasource and Fields
   * Renderers and Symbology

* **Lab7**: Working with Layer and Renderer Definitions

## Day 2

### 8:30 Doors open. 

Continue on Lecture 4 lab, etc.

### 09:00 Lecture 5: Interacting with the Map View  
* Working with MapView and Cameras
   * 2D and 3D Considerations
* The "Tool" pattern in Pro and Viewer Interaction
   * Feature selection using your sketch
   * Tool overlays and customization

* Demo only: <u>**Pro 1.2**</u> popup customization and embeddable control

* **Lab1**: Create a Custom Map Tool
* **Lab2**: Navigating the map view using the camera

### 10:30 Lecture 6: Geodatabase API

* Geodatabase
* Datasets
  * Table
  * FeatureClass
  * Schemas and Definitions
  * Versions
* Search and Retrieve Features
   * QueryFilter
   * Rows and Features
   * Related records

* Demo only: <u>**Pro 1.2**</u> CoreHost and GDB API.

* **Lab3**: Working with Datasets

Note: Lunch is on your own. Please take lunch anytime between 12 noon and lecture start at 13:30.

### 13:30 Lecture 7: Editing and Geometry 
* Introduction to the Geometry API
   * Create, Edit, Analyze
* Editing 
   * Edit Operations
   * Attributes and the Feature Inspector
   * Edit Events

*  **Lab4**: Geometry exercises
*  **Lab5**: Editing exercises

### 15:30 Lecture 8: Advanced Customization

* Custom Splash Screen
* Global DAML Filters
* Drag drop
* Command Filters

*  **Lab6**: Customize Pro

### 16:30 Wrap Up - All  
* Q & A

## Requirements

The requirements for the machine on which you develop your ArcGIS Pro add-ins are listed here.

####ArcGIS Pro

* ArcGIS Pro 1.2

####Supported platforms

* Windows 10 Professional and Enterprise (64 bit)  
* Windows 8.1 Basic, Professional, and Enterprise (64 bit [EM64T])   
* Windows 8 Basic, Professional, and Enterprise (64 bit [EM64T]) 
* Windows 7 SP1 Ultimate, Enterprise, Professional, and Home Premium (64 bit [EM64T])   

####Supported .NET framework

* 4.6  
* 4.5.2 
* 4.5.1 
* 4.5 

####Supported IDEs

* Visual Studio 2013: Professional, Premium, Ultimate, and Community Editions   
* Visual Studio 2015: Professional, Enterprise, and Community Editions  

## Resources

* [API Reference online](http://pro.arcgis.com/en/pro-app/sdk/api-reference)
* <a href="http://pro.arcgis.com/en/pro-app/sdk/" target="_blank">ArcGIS Pro SDK for .NET (pro.arcgis.com)</a>
* [arcgis-pro-sdk-community-samples](http://github.com/Esri/arcgis-pro-sdk-community-samples)
* [FAQ](http://github.com/Esri/arcgis-pro-sdk/wiki/FAQ)
* [ArcGIS Pro SDK icons](https://github.com/Esri/arcgis-pro-sdk/releases/tag/1.1.0.3308) - Scroll down the page to the "Download" section to access the icons.


## Issues


Find a bug or want to request a new feature?  Please let us know by submitting an issue.


## Contributing


Esri welcomes contributions from anyone and everyone. Please see our [guidelines for contributing](https://github.com/esri/contributing).


## Licensing
Copyright 2016 Esri


Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at


   http://www.apache.org/licenses/LICENSE-2.0


Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.


A copy of the license is available in the repository's [license.txt]( https://raw.github.com/Esri/quickstart-map-js/master/license.txt) file.


[](Esri Tags: ArcGIS Pro SDK)
[](Esri Language: C-Sharp, Visual Basic)​​​​​​​​​​​​​​​

