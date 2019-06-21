using System;
using System.Collections.Generic;
using System.Linq;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Core.Geometry;
using System.Windows.Media.Media3D;
using ArcGIS.Core.Data;
using ArcGIS.Core.CIM;

namespace CustomAnimation
{

  internal class CreateKeyframes : Button
  {
    private double _metersAbove = 1.0;
    private double _currentSpeed = 15.5; //Meters per second, ~ 35 miles per hour
    private bool _hasDropped = false;

    protected override void OnClick()
    {
      QueuedTask.Run(() =>
      {
        var mapView = MapView.Active;
        if (mapView == null)
          return;

        var polylines = GetRouteLines();
        var leftRail = polylines[0];
        var rightRail = polylines[1];

        double currentTimeSeconds = 0;
        MapPoint prevPoint = null;

        var cameraTrack = mapView.Map.Animation.Tracks.OfType<CameraTrack>().First();

        for (int i = 0; i < leftRail.Points.Count; i++)
        {
          var leftPoint = leftRail.Points[i];
          var rightPoint = rightRail.Points[i];
          var currentPoint = GetAveragePoint(leftPoint, rightPoint);
          List<Vector3D> vectors = null;

          if (i + 1 == leftRail.Points.Count)
          {
            vectors = GetVectors(currentPoint, prevPoint, leftPoint, rightPoint);
          }
          else
          {
            var nextPoint = GetAveragePoint(leftRail.Points[i + 1], rightRail.Points[i + 1]);
            vectors = GetVectors(nextPoint, currentPoint, leftPoint, rightPoint);
          }

          var offsetPoint = MapPointBuilder.CreateMapPoint(currentPoint.X + (_metersAbove * vectors[1].X),
            currentPoint.Y + (_metersAbove * vectors[1].Y),
            currentPoint.Z + (_metersAbove * vectors[1].Z),
            currentPoint.SpatialReference);

          var camera = new ArcGIS.Desktop.Mapping.Camera(offsetPoint.X, offsetPoint.Y, offsetPoint.Z, 0.0, 0.0, offsetPoint.SpatialReference, CameraViewpoint.LookFrom);

          #region Heading

          var radian = Math.Atan2(vectors[0].X, vectors[0].Y);
          var heading = radian * -180 / Math.PI;
          camera.Heading = heading;

          #endregion

          #region Pitch

          var hypotenuse = Math.Sqrt(Math.Pow(vectors[0].X, 2) + Math.Pow(vectors[0].Y, 2));
          radian = Math.Atan2(vectors[0].Z, hypotenuse);
          var pitch = (radian * 180 / Math.PI);
          camera.Pitch = pitch;

          #endregion

          #region Roll

          var d0 = new Vector3D(vectors[0].X, vectors[0].Y, 0);
          var u0 = new Vector3D(0, 0, 1);
          var r0 = Vector3D.CrossProduct(u0, d0);
          var roll = Vector3D.AngleBetween(r0, vectors[2]);
          if (vectors[2].Z < 0)
            roll *= -1;
          camera.Roll = roll;

          #endregion

          #region Speed

          if (i > 0)
          {
            var line = PolylineBuilder.CreatePolyline(new List<MapPoint>() { prevPoint, currentPoint }, currentPoint.SpatialReference);
            var length = GeometryEngine.Length3D(line) * currentPoint.SpatialReference.Unit.ConversionFactor;

            if (!_hasDropped && !(_hasDropped = (vectors[0].Z < 0)))
            {
              currentTimeSeconds += length / _currentSpeed;
            }
            else
            {
              _currentSpeed = Math.Sqrt(((Math.Pow(_currentSpeed, 2) / 2) + 9.8 * (prevPoint.Z - currentPoint.Z)) * 2);
              currentTimeSeconds += length / _currentSpeed;
            }
          }

          #endregion

          cameraTrack.CreateKeyframe(camera, TimeSpan.FromSeconds(currentTimeSeconds), AnimationTransition.FixedArc);
          prevPoint = currentPoint;
        }
      });
    }

    #region Private Methods

    private List<Polyline> GetRouteLines()
    {
      var rails = new List<Polyline>();

      var routeLayer = MapView.Active.Map.Layers.OfType<BasicFeatureLayer>().Where(l => l.ShapeType == ArcGIS.Core.CIM.esriGeometryType.esriGeometryPolyline).FirstOrDefault();

      var rows = routeLayer.Search();
      while (rows.MoveNext())
      {
        var row = rows.Current as Feature;
        rails.Add(row.GetShape() as Polyline);
      }

      return rails;
    }

    private MapPoint GetAveragePoint(MapPoint leftPoint, MapPoint rightPoint)
    {
      var avgX = (leftPoint.X + rightPoint.X) / 2;
      var avgY = (leftPoint.Y + rightPoint.Y) / 2;
      var avgZ = (leftPoint.Z + rightPoint.Z) / 2;
      var spatialRef = leftPoint.SpatialReference;
      return MapPointBuilder.CreateMapPoint(avgX, avgY, avgZ, spatialRef);
    }

    private List<Vector3D> GetVectors(MapPoint forward, MapPoint backward, MapPoint left, MapPoint right)
    {
      double dx = forward.X - backward.X;
      double dy = forward.Y - backward.Y;
      double dz = forward.Z - backward.Z;
      var d = new Vector3D(dx, dy, dz);
      d.Normalize();

      var rx = left.X - right.X;
      var ry = left.Y - right.Y;
      var rz = left.Z - right.Z;
      var r = new Vector3D(rx, ry, rz);
      r.Normalize();

      var u = Vector3D.CrossProduct(d, r);
      return new List<Vector3D>() { d, u, r };
    }

    #endregion
  }
}
