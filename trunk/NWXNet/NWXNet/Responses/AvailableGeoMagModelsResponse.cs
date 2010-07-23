using System;
using System.Collections.Generic;

namespace NWXNet
{
    public class AvailableGeoMagModelsResponse : IResponseData
    {
        public readonly List<GeoMagModel> Models = new List<GeoMagModel>();

        public GeoMagModel this[int id]
        {
            get { return Models[id]; }
        }

        public GeoMagModel this[string name]
        {
            get { return Models.Find(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)); }
        }
    }

    public class GeoMagModel
    {
        /// <summary>
        /// Name of the model.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Base year of the grid data.
        /// </summary>
        public int BaseGridYear { get; set; }

        /// <summary>
        /// First year for which this model is valid.
        /// </summary>
        public int FirstValidYear { get; set; }

        /// <summary>
        /// Last year for which this model is valid.
        /// </summary>
        public int LastValidYear { get; set; }

        /// <summary>
        /// Resolution of the latitude axis (in sexagesimal degrees).
        /// </summary>
        public double LatitudeResolution { get; set; }

        /// <summary>
        /// Resolution of the longitude axis (in sexagesimal degrees).
        /// </summary>
        public double LongitudeResolution { get; set; }
    }
}