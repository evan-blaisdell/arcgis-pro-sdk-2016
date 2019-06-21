//Copyright 2014 Esri

//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
using ArcGIS.Desktop.Framework.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Events;
using System.Windows.Input;
using ArcGIS.Desktop.Mapping.Events;
using ArcGIS.Core.CIM;

namespace NavigateCamera
{
  /// <summary>
  /// View model for manipulating the camera properties based on the custom user control
  /// </summary>
  class CameraControlViewModel : CustomControl
  {
    /// <summary>
    /// Default constructor which initializes the RelayCommands, subscribes to events and sets the initial heading.
    /// </summary>
      public CameraControlViewModel()
      {
          //TODO initialize the zoom in and zoom out RelayCommands.
          _zoomInCmd = new RelayCommand(() => ZoomIn(), () => CanZoom());
          _zoomOutCmd = new RelayCommand(() => ZoomOut(), () => CanZoom());

          //TODO subscribe to MapViewCameraChangedEvent and ActiveMapViewChangedEvent.
          MapViewCameraChangedEvent.Subscribe(OnCameraChanged);
          ActiveMapViewChangedEvent.Subscribe(OnActiveViewChanged);

          //TODO initialize the pitch up and pitch down RelayCommands.
          _pitchDownCmd = new RelayCommand(() => PitchDown(), () => CanAdjustPitch());
          _pitchUpCmd = new RelayCommand(() => PitchUp(), () => CanAdjustPitch());

          //TODO initialize the heading by calling SetHeadingFromMapView and passing in the active map view.
          SetHeadingFromMapView(MapView.Active);
      }

    #region Zoom Commands

    /// <summary>
    /// ICommand for zoom in which is bound to from the custom control
    /// </summary>
    private RelayCommand _zoomInCmd;
    public ICommand ZoomInCmd
    {
        get { return _zoomInCmd; }
    }

    /// <summary>
    /// Zoom the active map view by in a fixed amount
    /// </summary>
    private void ZoomIn()
    {
        //TODO zoom the active view in by a fixed amount.
        MapView.Active.ZoomInFixedAsync(TimeSpan.FromSeconds(.25));
    }

    /// <summary>
    /// ICommand for zoom out which is bound to from the custom control
    /// </summary>
    private RelayCommand _zoomOutCmd;
    public ICommand ZoomOutCmd
    {
        get { return _zoomOutCmd; }
    }

    /// <summary>
    /// Zoom the active map view by out a fixed amount
    /// </summary>
    private void ZoomOut()
    {
        //TODO zoom the active view in by a fixed amount.
        MapView.Active.ZoomOutFixedAsync(TimeSpan.FromSeconds(.25));
    }

    /// <summary>
    /// Indicates if a zoom operation can be performed.
    /// </summary>
    private bool CanZoom()
    {
        //TODO return true if the active map view is not null.
        return MapView.Active != null;
    }

    #endregion

    #region Adjust Heading

    /// <summary>
    /// Property bound to by the heading slider IsEnabled property.
    /// </summary>
    private bool _enableCamera = false;
    public bool IsCameraEnabled
    {
        get
        {
            return _enableCamera;
        }
        set
        {
            SetProperty(ref _enableCamera, value, () => IsCameraEnabled);
        }
    }

    /// <summary>
    /// Property bound to by the heading slider Value property.
    /// </summary>
    private double _headingValue = 0;
    public double HeadingValue
    {
        get { return _headingValue; }
        set
        {
            double cameraHeading = value > 180 ? value - 360 : value;
            _headingValue = value;
            Camera.Heading = cameraHeading;
            //TODO get the active map view and zoom to the new camera.
            MapView activeMapView = MapView.Active;
            if (activeMapView != null)
                activeMapView.ZoomToAsync(Camera, TimeSpan.FromSeconds(.25));
        }
    }

    /// <summary>
    /// Current camera for the active map view.
    /// </summary>
    private Camera Camera { get; set; }

    /// <summary>
    /// Called when the camera changes in the map view.
    /// </summary>
    private void OnCameraChanged(MapViewCameraChangedEventArgs args)
    {
        Camera = args.CurrentCamera;
        SetHeading();
    }

    /// <summary>
    /// Called when the active map view changes.
    /// </summary>
    private void OnActiveViewChanged(ActiveMapViewChangedEventArgs args)
    {
        SetHeadingFromMapView(args.IncomingView);
    }

    /// <summary>
    /// Gets the camera from the map view and sets the heading
    /// </summary>
    private void SetHeadingFromMapView(MapView mapView)
    {
        //TODO test if mapView is null.
        //If not null set IsCameraEnabled to true
        //get the Camera from the mapView (mapView.GetCameraAsync())
        //call SetHeading().
        if (mapView != null)
        {
            Camera = mapView.Camera;
            IsCameraEnabled = true;
            SetHeading();
        }
        else
        {
            //If null set "IsCameraEnabled = false".
            IsCameraEnabled = false;
        }
    }

    /// <summary>
    /// Sets the HeadingValue backing field and raises a PropertyChanged event for the HeadingValue property.
    /// </summary>
    private void SetHeading()
    {
        if (Camera != null)
        {
            double viewHeading = Camera.Heading < 0 ? 360 + Camera.Heading : Camera.Heading;
            SetProperty(ref _headingValue, viewHeading, () => HeadingValue);
        }
    }

    #endregion

    #region Pitch Commands

    /// <summary>
    /// ICommand for pitch down which is bound to from the custom control.
    /// </summary>
    private RelayCommand _pitchDownCmd;
    public ICommand PitchDownCmd
    {
        get { return _pitchDownCmd; }
    }

    /// <summary>
    /// Subtracts 5 from the current pitch and zooms to the new camera.
    /// </summary>
    private void PitchDown()
    {
        //TODO if the camera's pitch is greater than -90 subtract 5 and zoom to the new camera.
        if (Camera.Pitch > -90)
        {
            Camera.Pitch -= 5;
            MapView.Active.ZoomToAsync(Camera, TimeSpan.FromSeconds(.25));
        }
    }

    /// <summary>
    /// ICommand for pitch up which is bound to from the custom control.
    /// </summary>
    private RelayCommand _pitchUpCmd;
    public ICommand PitchUpCmd
    {
        get { return _pitchUpCmd; }
    }

    /// <summary>
    /// Adds 5 to the current pitch and zooms to the new camera.
    /// </summary>
    private void PitchUp()
    {
        //TODO if the camera's pitch is less than 90 add 5 and zoom to the new camera.
        if (Camera.Pitch < 90)
        {
            Camera.Pitch += 5;
            MapView.Active.ZoomToAsync(Camera, TimeSpan.FromSeconds(.25));
        }
    }

    /// <summary>
    /// Indicates if the pitch can be adjusted.
    /// </summary>
    private bool CanAdjustPitch()
    {
        //TODO return true if there is an active map view and the view is a 3D view.
        return MapView.Active != null && MapView.Active.ViewingMode != MapViewingMode.Map;
    }

    #endregion

  }
}
