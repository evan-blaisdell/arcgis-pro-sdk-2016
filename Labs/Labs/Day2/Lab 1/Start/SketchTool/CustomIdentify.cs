using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Mapping;
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
            //Set the tool's OverlayControlID property to the DAML ID of the Embeddable Control
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
                var vm = this.OverlayEmbeddableControl as EmbeddedControlViewModel;
                if (vm == null)
                    return true;

                //Update the results in the view model
                vm.UpdateResults(result);
                return true;
            });
        }
    }
}
