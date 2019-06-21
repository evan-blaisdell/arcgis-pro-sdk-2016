using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using System.Windows;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Editing.Attributes;
using ArcGIS.Desktop.Framework.Threading.Tasks;

namespace AttributeToolWithInspector {
    internal class AttributeToolWithInspector : MapTool
    {
        private OverlayControlViewModel _vm = null;
        private CIMPointSymbol _overlaySymbol = null;
        private IDisposable _disposer = null;
        private DelayedInvoker _invoker = new DelayedInvoker(10);
        private BasicFeatureLayer _trees = null;
        private Point _lastLocation;
        private static readonly object _lock = new object();

        private Inspector _theInspector = null;

        public AttributeToolWithInspector() {
            this.OverlayControlID = "AttributeToolOverlayControl_EmbeddableControl";
        }
        protected override Task OnToolActivateAsync(bool active) {
            if (_vm == null) {
                _vm = this.OverlayEmbeddableControl as OverlayControlViewModel;
            }
            if (_overlaySymbol == null) {
                QueuedTask.Run(() => {
                    _overlaySymbol = SymbolFactory.ConstructPointSymbol(ColorFactory.Red, 12.0, SimpleMarkerStyle.Circle);
                });
            }
            if (_trees == null) {
                _trees = ActiveMapView.Map.GetLayersAsFlattenedList().FirstOrDefault((lyr) => lyr.Name == "Tree") as BasicFeatureLayer;
            }
            if (_theInspector == null)
            {
                _theInspector = new Inspector();
                var tuple = _theInspector.CreateEmbeddableControl();
                _vm.InspectorView = tuple.Item2;
                _vm.InspectorViewModel = tuple.Item1;
            }
            return base.OnToolActivateAsync(active);
        }

        protected override Task OnToolDeactivateAsync(bool hasMapViewChanged)
        {
            _trees = null;
            if (_disposer != null) {
                lock (_lock) {
                    _disposer.Dispose();
                    _disposer = null;
                }
            }
            if (_vm != null)
            {
                _vm.InspectorViewModel = null;
                _vm.InspectorView = null;
            }
            _theInspector = null;
            return base.OnToolDeactivateAsync(hasMapViewChanged);
        }

        protected override async void OnToolMouseMove(MapViewMouseEventArgs e) {
            if (_lastLocation == e.ClientPoint)
                return;
            _lastLocation = e.ClientPoint;

            _invoker.Invoke(async () => {
                var tuple = await AddFeatureToOverlay(e);
                lock (_lock)
                {
                    long oid = tuple.Item2;
                    if (oid >= 0)
                        _theInspector.LoadAsync(_trees, oid);
                    else
                        _theInspector.ClearAsync();

                    if (_disposer != null) _disposer.Dispose();
                    _disposer = tuple.Item1;
                }
            });
        }

        private Task<Tuple<IDisposable, long>> AddFeatureToOverlay(MapViewMouseEventArgs e) {
            return QueuedTask.Run(() => {

                double llx = e.ClientPoint.X - 3;
                double lly = e.ClientPoint.Y - 3;
                double urx = e.ClientPoint.X + 3;
                double ury = e.ClientPoint.Y + 3;

                EnvelopeBuilder envBuilder = new EnvelopeBuilder(ActiveMapView.ClientToMap(new Point(llx, lly)),
                    ActiveMapView.ClientToMap(new Point(urx, ury)));

                MapPoint mp = ActiveMapView.ClientToMap(e.ClientPoint);
                var cursor = _trees.Search(new SpatialQueryFilter() {
                    FilterGeometry = envBuilder.ToGeometry(), SpatialRelationship = SpatialRelationship.Intersects
                });
                if (cursor.MoveNext())
                {
                    return new Tuple<IDisposable, long>(
                        ActiveMapView.AddOverlay(ActiveMapView.ClientToMap(e.ClientPoint),
                            _overlaySymbol.MakeSymbolReference()),
                            cursor.Current.GetObjectID());
                }

                //var select = _trees.Select(new SpatialQueryFilter() {
                //    FilterGeometry = envBuilder.ToGeometry(),
                //    SpatialRelationship = SpatialRelationship.Intersects
                //});
                //if (select.GetCount() > 0) {
                //    return ActiveMapView.AddOverlay(ActiveMapView.ClientToMap(e.ClientPoint), _overlaySymbol.MakeSymbolReference());
                //}

                return new Tuple<IDisposable, long>(null, -1);
            });
        }

        protected override void OnToolMouseDown(MapViewMouseButtonEventArgs e) {
            base.OnToolMouseDown(e);
        }
    }
}
