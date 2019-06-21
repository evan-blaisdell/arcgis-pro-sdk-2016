using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;
using ArcGIS.Desktop.Framework.Controls;

namespace AttributeToolWithInspector {
    internal class OverlayControlViewModel : EmbeddableControl
    {

        private EmbeddableControl _inspectorViewModel = null;
        private UserControl _inspectorView = null;
        public OverlayControlViewModel(XElement options) : base(options)   {
        }

        public EmbeddableControl InspectorViewModel
        {
            get { return _inspectorViewModel; }
            set
            {
                if (value != null)
                {
                    _inspectorViewModel = value;
                    _inspectorViewModel.OpenAsync();
                    
                }
                else if (_inspectorViewModel != null)
                {
                    _inspectorViewModel.CloseAsync();
                    _inspectorViewModel = value;
                }
                NotifyPropertyChanged(() => InspectorViewModel);
            }
        }

        public UserControl InspectorView
        {
            get { return _inspectorView;  }
            set { SetProperty(ref _inspectorView, value, () => InspectorView); }
        }
    }
}
