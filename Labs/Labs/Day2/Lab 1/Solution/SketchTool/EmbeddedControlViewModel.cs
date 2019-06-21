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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Controls;
using System.Xml.Linq;
using ArcGIS.Desktop.Mapping;
using System.Collections.ObjectModel;
using ArcGIS.Desktop.Framework.Threading.Tasks;

namespace SketchTool
{
  /// <summary>
  /// ViewModel for the embeddable control.
  /// </summary>
  internal class EmbeddedControlViewModel : EmbeddableControl
  {
    private object _lock = new object();

    public EmbeddedControlViewModel(XElement options) : base(options) 
    {
      System.Windows.Data.BindingOperations.EnableCollectionSynchronization(_identifyResults, _lock);
    }

    private ObservableCollection<Result> _identifyResults = new ObservableCollection<Result>();
    public ObservableCollection<Result> IdentifyResults
    {
      get { return _identifyResults; }
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
        {
          ////Flash the associated features in the selected result.
          //var result = new Dictionary<BasicFeatureLayer, List<long>>();
          //result.Add(_selectedResult.Layer, _selectedResult.OIDs);
          //mapView.FlashFeature(result);

          QueuedTask.Run(() =>
          {
            //Clear the existing selection in the map.
            mapView.Map.SetSelection(null);

            //Construct a query filter using the OIDs from the selected result.
            var oidField = _selectedResult.Layer.GetTable().GetDefinition().GetObjectIDField();
            var qf = new ArcGIS.Core.Data.QueryFilter() { WhereClause = string.Format("({0} in ({1}))", oidField, string.Join(",", _selectedResult.OIDs)) };

            //Create a new selection using the query filter.
            _selectedResult.Layer.Select(qf);

            mapView.ZoomToSelectedAsync(TimeSpan.FromSeconds(1));
          });
        }
      }
    }

    /// <summary>
    /// Update the observable collection of results using the return from GetFeatures
    /// </summary>
    public void UpdateResults(Dictionary<BasicFeatureLayer, List<long>> results)
    {
      lock (_lock)
      {
        _identifyResults.Clear();
        foreach (var result in results)
        {
          _identifyResults.Add(new Result(result.Key, result.Value));
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
