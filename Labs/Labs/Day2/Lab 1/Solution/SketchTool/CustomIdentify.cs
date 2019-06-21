﻿//   Copyright 2015 Esri

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
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;

namespace SketchTool
{
  class CustomIdentify : MapTool
  {
    public CustomIdentify()
    {
      IsSketchTool = true;
      SketchType = SketchGeometryType.Rectangle;
      SketchOutputMode = SketchOutputMode.Screen;
      OverlayControlID = "SketchTool_EmbeddableControl";
    }

    protected override Task<bool> OnSketchCompleteAsync(Geometry geometry)
    {
      return QueuedTask.Run(() =>
      {
        //Get the features that intersect the sketch geometry
        var result = MapView.Active.GetFeatures(geometry);
        MapView.Active.FlashFeature(result);
        
        //Get the instance of the overlay control view model
        var vm = OverlayEmbeddableControl as EmbeddedControlViewModel;
        if (vm == null)
          return true;

        //Update the results in the view model
        vm.UpdateResults(result);
        return true;
      });
    }
  }
}
