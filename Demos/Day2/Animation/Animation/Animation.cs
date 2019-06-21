using System;
using System.Collections.Generic;
using System.Linq;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Core.CIM;

namespace CustomAnimation
{
  internal class AnimationModule : Module
  {
    private static AnimationModule _this = null;
    private static AnimationSettings _settings = AnimationSettings.Default;

    /// <summary>
    /// Retrieve the singleton instance to this module here
    /// </summary>
    public static AnimationModule Current
    {
        get
        {
            return _this ?? (_this = (AnimationModule)FrameworkApplication.FindModule("RollerCoaster_Module"));
        }
    }

    #region Overrides
    /// <summary>
    /// Called by Framework when ArcGIS Pro is closing
    /// </summary>
    /// <returns>False to prevent Pro from closing, otherwise True</returns>
    protected override bool CanUnload()
    {
        //TODO - add your business logic
        //return false to ~cancel~ Application close
        return true;
    }

    #endregion Overrides

    /// <summary>
    /// Get the settings persisted with the user profile for the add-in.
    /// </summary>
    public static AnimationSettings Settings
    {
      get { return _settings; }
    }

    /// <summary>
    /// Creates keyframes along the path using the user defined settings.
    /// </summary>
    /// <param name="line">The geometry of the line to fly along.</param>
    /// <param name="verticalUnit">The elevation unit of the 3D layer</param>
    internal Task CreateKeyframesAlongPath(Polyline line, Unit verticalUnit)
    {
      return QueuedTask.Run(() =>
      {
        var mapView = MapView.Active;
        if (mapView == null)
          return;

        //Get the camera track from the active map's animation.
        //There will always be only one camera track in the animation.
        var cameraTrack = mapView.Map.Animation.Tracks.OfType<CameraTrack>().First();

        //Get some of the user settings for constructing the keyframes alone the path.
        var densifyDistance = AnimationModule.Settings.KeyEvery;
        var verticalOffset = AnimationModule.Settings.HeightAbove / ((mapView.Map.SpatialReference.IsGeographic) ? 1.0 : mapView.Map.SpatialReference.Unit.ConversionFactor); //1 meter
        double currentTimeSeconds = GetInsertTime(mapView.Map.Animation);

        //We need to project the line to a projected coordinate system to calculate the line's length in 3D 
        //as well as more accurately calculated heading and pitch along the path. 
        if (line.SpatialReference.IsGeographic)
        {
          if (mapView.Map.SpatialReference.IsGeographic)
          {
            var transformation = ProjectionTransformation.Create(line.SpatialReference, SpatialReferences.WebMercator, line.Extent);
            line = GeometryEngine.ProjectEx(line, transformation) as Polyline;
          }
          else
          {
            var transformation = ProjectionTransformation.Create(line.SpatialReference, mapView.Map.SpatialReference, line.Extent);
            line = GeometryEngine.ProjectEx(line, transformation) as Polyline;
          }
        }

        //If the user has specified to create keyframes at additional locations than just the vertices 
        //we will densify the line by the distance the user specified. 
        if (!AnimationModule.Settings.VerticesOnly)
          line = GeometryEngine.DensifyByLength(line, densifyDistance / line.SpatialReference.Unit.ConversionFactor) as Polyline;

        //To maintain a constant speed we need to divide the total time we want the animation to take by the length of the line.
        var duration = AnimationModule.Settings.Duration;
        var secondsPerUnit = duration / line.Length3D;
        Camera prevCamera = null;

        //Loop over each vertex in the line and create a new keyframe at each.
        for (int i = 0; i < line.PointCount; i++)
        {
          #region Camera

          MapPoint cameraPoint = line.Points[i];

          //If the point is not in the same spatial reference of the map we need to project it.
          if (cameraPoint.SpatialReference.Wkid != mapView.Map.SpatialReference.Wkid)
          {
            var transformation = ProjectionTransformation.Create(cameraPoint.SpatialReference, mapView.Map.SpatialReference);
            cameraPoint = GeometryEngine.Project(cameraPoint, mapView.Map.SpatialReference) as MapPoint;
          }

          //Construct a new camera from the point.
          var camera = new Camera(cameraPoint.X, cameraPoint.Y, cameraPoint.Z,
            AnimationModule.Settings.Pitch, 0.0, cameraPoint.SpatialReference, CameraViewpoint.LookFrom);

          //Convert the Z unit to meters if the camera is not in a geographic coordinate system.
          if (!camera.SpatialReference.IsGeographic)
            camera.Z /= camera.SpatialReference.Unit.ConversionFactor;

          //Convert the Z to the unit of the layer's elevation unit and then add the user defined offset from the line.
          camera.Z *= verticalUnit.ConversionFactor;
          camera.Z += verticalOffset;

          //If this is the last point in the collection use the same heading and pitch from the previous camera.
          if (i + 1 == line.Points.Count)
          {
            camera.Heading = prevCamera.Heading;
            camera.Pitch = prevCamera.Pitch;
          }
          else
          {
            var currentPoint = line.Points[i];
            var nextPoint = line.Points[i + 1];

            #region Heading

            //Calculate the heading from the current point to the next point in the path.
            var difX = nextPoint.X - currentPoint.X;
            var difY = nextPoint.Y - currentPoint.Y;
            var radian = Math.Atan2(difX, difY);
            var heading = radian * -180 / Math.PI;
            camera.Heading = heading;

            #endregion

            #region Pitch

            //If the user doesn't want to hardcode the pitch, calculate the pitch based on the current point to the next point.
            if (AnimationModule.Settings.UseLinePitch)
            {
              var hypotenuse = Math.Sqrt(Math.Pow(difX, 2) + Math.Pow(difY, 2));
              var difZ = nextPoint.Z - currentPoint.Z;
              //If the line's unit is not the same as the elevation unit of the layer we need to convert the Z so they are in the same unit.
              if (line.SpatialReference.Unit.ConversionFactor != verticalUnit.ConversionFactor)
                difZ *= (verticalUnit.ConversionFactor / line.SpatialReference.Unit.ConversionFactor);
              radian = Math.Atan2(difZ, hypotenuse);
              var pitch = radian * 180 / Math.PI;
              camera.Pitch = pitch;
            }
            else
            {
              camera.Pitch = AnimationModule.Settings.Pitch;
            }

            #endregion
          }

          #endregion

          #region Time

          //The first point will have a time of 0 seconds, after that we need to set the time based on the 3D distance between the points.
          if (i > 0)
          {
            var lineSegment = PolylineBuilder.CreatePolyline(new List<MapPoint>() { line.Points[i - 1], line.Points[i] },
              line.SpatialReference);
            var length = lineSegment.Length3D;
            currentTimeSeconds += length * secondsPerUnit;
          }

          #endregion

          //Create a new keyframe using the camera and the time.
          cameraTrack.CreateKeyframe(camera, TimeSpan.FromSeconds(currentTimeSeconds), AnimationTransition.Linear);
          prevCamera = camera;
        }
      });
    }

    /// <summary>
    /// Get the time time to begin inserting new keyframes and shift any existing keyframes if necessary.
    /// </summary>
    /// <param name="animation">The animation to be modified.</param>
    private double GetInsertTime(ArcGIS.Desktop.Mapping.Animation animation)
    {
      var duration = AnimationModule.Settings.Duration;
      double currentTimeSeconds = 0;
      if (animation.Duration > TimeSpan.Zero)
      {
        if (AnimationModule.Settings.IsAfterTime)
        {
          currentTimeSeconds = (animation.Duration + TimeSpan.FromSeconds(AnimationModule.Settings.AfterTime)).TotalSeconds;
        }
        else
        {
          currentTimeSeconds = AnimationModule.Settings.AtTime;
          ShiftKeyframes(currentTimeSeconds, duration);
        }
      }
      return currentTimeSeconds;
    }

    /// <summary>
    /// Shift the existing keyframes from the provided time by the provided duration.
    /// </summary>
    /// <param name="insertTime">The time at which all keyframes after should be shifted.</param>
    /// <param name="duration">The amount of time to shift each keyframe.</param>
    private void ShiftKeyframes(double insertTime, double duration)
    {
      var mapView = MapView.Active;
      if (mapView == null)
        return;

      var animation = mapView.Map.Animation;
      foreach (var track in animation.Tracks)
      {
        var keyframes = track.Keyframes.Where(k => k.TrackTime > TimeSpan.FromSeconds(insertTime)).OrderByDescending(k => k.TrackTime);
        foreach (var keyframe in keyframes)
        {
          keyframe.TrackTime = (keyframe.TrackTime + TimeSpan.FromSeconds(duration));
        }
      }
    }

  }
}
