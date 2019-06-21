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
using ArcGIS.Desktop.Framework.Contracts;

namespace CustomAnimation
{
  /// <summary>
  /// ViewModel for the path options custom control.
  /// </summary>
  internal class PathOptionsControlViewModel : CustomControl
  {
    private double _heightAbove = AnimationModule.Settings.HeightAbove;
    public double HeightAbove
    {
      get { return _heightAbove; }
      set
      {
        if (SetProperty(ref _heightAbove, Math.Round(value, 2), () => HeightAbove))
          AnimationModule.Settings.HeightAbove = _heightAbove;
      }
    }

    private double _keyEvery = AnimationModule.Settings.KeyEvery;
    public double KeyEvery
    {
      get { return _keyEvery; }
      set
      {
        if (SetProperty(ref _keyEvery, Math.Round(Math.Abs(value), 2), () => KeyEvery))
          AnimationModule.Settings.KeyEvery = _keyEvery;
      }
    }

    private bool _verticesOnly = AnimationModule.Settings.VerticesOnly;
    public bool VerticesOnly
    {
      get { return _verticesOnly; }
      set
      {
        if (SetProperty(ref _verticesOnly, value, () => VerticesOnly))
          AnimationModule.Settings.VerticesOnly = _verticesOnly;
      }
    }

    private double _pitch = AnimationModule.Settings.Pitch;
    public double Pitch
    {
      get { return _pitch; }
      set
      {
        if (SetProperty(ref _pitch, Math.Round(value, 2), () => Pitch))
          AnimationModule.Settings.Pitch = _pitch;
      }
    }

    private bool _useLinePitch = AnimationModule.Settings.UseLinePitch;
    public bool UseLinePitch
    {
      get { return _useLinePitch; }
      set
      {
        if (SetProperty(ref _useLinePitch, value, () => UseLinePitch))
          AnimationModule.Settings.UseLinePitch = _useLinePitch;
      }
    }
  }
}
