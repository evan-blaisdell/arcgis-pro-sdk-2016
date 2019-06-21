//Copyright 2015-2016 Esri

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
using System.Windows.Input;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using System.Threading.Tasks;
using ArcGIS.Core.Geometry;

namespace GeometryEditingSolution
{
    /// <summary>
    /// 
    /// </summary>
    internal class GeometryEditingModule : Module
    {
        private static GeometryEditingModule _this = null;

        /// <summary>
        /// Retrieve the singleton instance to this module here
        /// </summary>
        public static GeometryEditingModule Current
        {
            get
            {
                return _this ?? (_this = (GeometryEditingModule)FrameworkApplication.FindModule("GeometryEditingSolution_Module"));
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

    }

    /// <summary>
    /// Extension method to generate random coordinates within a given extent.
    /// </summary>
    public static class RandomExtension
    {
        /// <summary>
        /// Generate a random double number between the min and max values.
        /// </summary>
        /// <param name="random">Instance of a random class.</param>
        /// <param name="minValue">The min value for the potential range.</param>
        /// <param name="maxValue">The max value for the potential range.</param>
        /// <returns>Random number between min and max</returns>
        /// <remarks>The random result number will always be less than the max number.</remarks>
        public static double NextDouble(this Random random, double minValue, double maxValue)
        {
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }

        /// <summary>
        /// /Generate a random coordinate within the provided envelope.
        /// </summary>
        /// <param name="random">Instance of a random class.</param>
        /// <param name="withinThisExtent">Area of interest in which the random coordinate will be created.</param>
        /// <param name="is3D">Boolean indicator if the coordinate should be 2D (only x,y values) or 3D (containing x,y,z values).</param>
        /// <returns>A coordinate with random values within the extent.</returns>
        public static Coordinate NextCoordinate(this Random random, Envelope withinThisExtent, bool is3D)
        {
            Coordinate newCoordinate;

            if (is3D)
                newCoordinate = new Coordinate(random.NextDouble(withinThisExtent.XMin, withinThisExtent.XMax),
                    random.NextDouble(withinThisExtent.YMin, withinThisExtent.YMax), 0);
            else
                newCoordinate = new Coordinate(random.NextDouble(withinThisExtent.XMin, withinThisExtent.XMax),
                    random.NextDouble(withinThisExtent.YMin, withinThisExtent.YMax));

            return newCoordinate;
        }
    }

}
