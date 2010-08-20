using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWXNet.Requests
{
    public class SunRiseAndSet : IRequestData
    {
        internal LatLon Coordinates { get; private set; }
        internal DateTime Epoch { get; private set; }

        private SunRiseAndSet(LatLon coordinates, DateTime epoch)
        {
            Coordinates = coordinates;
            Epoch = epoch;
        }

        /// <summary>
        /// Create a Sun rise/set request for the specified coordinates and epoch.
        /// </summary>
        /// <param name="coords">The latitude/longitude, in decimal format (xx.x,xx.x).</param>
        /// <param name="epoch">The epoch to request for (this can simply be set to the date you wish to receive information for).</param>
        /// <returns></returns>
        public static SunRiseAndSet For(string coords, DateTime epoch)
        {
            return new SunRiseAndSet(new LatLon(coords), epoch);
        }

        /// <summary>
        /// Create a Sun rise/set request for the specified coordinates and epoch.
        /// </summary>
        /// <param name="coords">The latitude/longitude.</param>
        /// <param name="epoch">The epoch to request for (this can simply be set to the date you wish to receive information for).</param>
        /// <returns></returns>
        public static SunRiseAndSet For(LatLon coords, DateTime epoch)
        {
            return new SunRiseAndSet(coords, epoch);
        }

        #region Implementation of IRequestData

        RequestTypes IRequestData.Type
        {
            get { return RequestTypes.SunRiseAndSet; }
        }

        bool IRequestData.IsValid
        {
            get { return (Coordinates != null) && (Epoch != DateTime.MinValue); }
        }

        public string Id
        {
            get { return Coordinates.ToString("p"); }
        }

        #endregion
    }
}
