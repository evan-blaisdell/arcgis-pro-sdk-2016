//   Copyright 2015 Esri
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
using ArcGIS.Desktop.Framework.Controls;
using System.Xml.Linq;
using ArcGIS.Desktop.Mapping;
using System.Collections.ObjectModel;
using ArcGIS.Desktop.Framework.Threading.Tasks;

namespace Mapping_Addin
{
    /// <summary>
    /// ViewModel for the embeddable control.
    /// </summary>
    internal class EmbeddedControlViewModel : EmbeddableControl
  {
    private object _lock = new object();

    public EmbeddedControlViewModel(XElement options) : base(options) 
    {
      System.Windows.Data.BindingOperations.EnableCollectionSynchronization(_selectionResults, _lock);
    }

    private ObservableCollection<Result> _selectionResults = new ObservableCollection<Result>();
    public ObservableCollection<Result> SelectionResults
    {
      get { return _selectionResults; }
    }

    private Result _selectedResult;
    public Result SelectedResult
    {
      get { return _selectedResult; }
      set
      {
        SetProperty(ref _selectedResult, value, () => SelectedResult);
        var mapView = MapView.Active;
        if (mapView != null && _selectedResult != null)
            mapView.FlashFeature(new Dictionary<BasicFeatureLayer, List<long>>() { { _selectedResult.Layer, _selectedResult.OIDs } });
      }
    }

    /// <summary>
    /// Update the observable collection of results using the return from SelectFeatures
    /// </summary>
    public void UpdateResults(Dictionary<BasicFeatureLayer, List<long>> results)
    {
      lock (_lock)
      {
        _selectionResults.Clear();
        foreach (var result in results)
        {
          _selectionResults.Add(new Result(result.Key, result.Value));
        }
      }

    }
  }

  /// <summary>
  /// Represents a single identify result for a layer and its corresponding object ids
  /// </summary>
  internal class Result
  {
    public Result(BasicFeatureLayer layer, List<long> oids)
    {
      Layer = layer;
      OIDs = oids;
    }

    internal BasicFeatureLayer Layer { get; set; }

    internal List<long> OIDs { get; set; }

    public override string ToString()
    {
      return string.Format("{0}: {1}", Layer.Name, OIDs.Count);
    }
  }
}
