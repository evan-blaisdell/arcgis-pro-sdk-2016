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

namespace CustomAnimation
{
  /// <summary>
  /// ViewModel for the timing options control.
  /// </summary>
  internal class TimingControlViewModel : CustomControl
  {

    public TimingControlViewModel()
    {

    }

    private bool _isAtTime = !AnimationModule.Settings.IsAfterTime;
    public bool IsAtTime
    {
      get { return _isAtTime; }
      set
      {
        if (SetProperty(ref _isAtTime, value, () => IsAtTime))
          AnimationModule.Settings.IsAfterTime = !_isAtTime;
      }
    }

    private bool _isAfterTime = AnimationModule.Settings.IsAfterTime;
    public bool IsAfterTime
    {
      get { return _isAfterTime; }
      set
      {
        if (SetProperty(ref _isAfterTime, value, () => IsAfterTime))
          AnimationModule.Settings.IsAfterTime = _isAfterTime;
      }
    }

    private double _atTime = AnimationModule.Settings.AtTime;
    public double AtTime
    {
      get { return _atTime; }
      set
      {
        if (SetProperty(ref _atTime, Math.Round(Math.Abs(value), 3), () => AtTime))
          AnimationModule.Settings.AtTime = _atTime;
      }
    }

    private double _afterTime = AnimationModule.Settings.AfterTime;
    public double AfterTime
    {
      get { return _afterTime; }
      set
      {
        if (SetProperty(ref _afterTime, Math.Round(Math.Abs(value), 3), () => AfterTime))
          AnimationModule.Settings.AfterTime = _afterTime;
      }
    }

    private double _duration = AnimationModule.Settings.Duration;
    public double Duration
    {
      get { return _duration; }
      set
      {
        if (SetProperty(ref _duration, Math.Round(Math.Abs(value), 3), () => Duration))
          AnimationModule.Settings.Duration = _duration;
      }
    }
  }
}
