using System;

namespace NWXNet
{
    /// <summary>
    /// Helper class to provide commands to set features for chart requests.
    /// </summary>
    public static class ChartFeatures
    {
        /// <summary>
        /// Draw coastlines and political boundaries.
        /// </summary>
        public static string CountriesAndCoastlines
        {
            get { return "setmpdraw on"; }
        }

        /// <summary>
        /// Draw a latitude/longitude grid.  This has no effect if using map layout.
        /// </summary>
        public static string LatLonGrid
        {
            get { return "setgrid on"; }
        }

        /// <summary>
        /// Draw wind barbs representing wind direction and strength.
        /// </summary>
        public static string Wind
        {
            get { return "wind"; }
        }

        /// <summary>
        /// Color map with precipitation type (rain, freezing rain, hail, snow).
        /// </summary>
        public static string PrecipitationType
        {
            get { return "preciptype"; }
        }

        /// <summary>
        /// Show QFF on image (atmospheric pressure reduced at mean sea level).
        /// </summary>
        public static string QFF
        {
            get { return "prmslmsl"; }
        }

        /// <summary>
        /// Show freezing level on image (0°C isotherm).
        /// </summary>
        public static string FreezingLevel
        {
            get { return "hgt0c"; }
        }

        /// <summary>
        /// Show geopotential height on image (in feet).
        /// </summary>
        /// <param name="color">If set to <c>true</c>, contours will be colored individually.</param>
        /// <returns></returns>
        public static string GeopotentialHeight(bool color = false)
        {
            return color ? "hgtprs rainbow" : "hgtprs";
        }

        /// <summary>
        /// Show surface temperature chart on image.
        /// </summary>
        /// <param name="contour">if set to <c>true</c> a contour chart will be drawn rather than a shaded chart.</param>
        /// <param name="interval">The contour interval.</param>
        /// <returns></returns>
        public static string SurfaceTemperature(bool contour = false, int interval = 0)
        {
            return contour ? "tmpsfc c " + interval : "tmpsfc";
        }

        /// <summary>
        /// Show surface temperature grid on image.
        /// </summary>
        public static string SurfaceTemperatureGrid
        {
            get { return "tmpsfc_g"; }
        }

        /// <summary>
        /// Show temperature at the chart's pressure level on image.
        /// </summary>
        public static string LevelTemperatureGrid
        {
            get { return "tmpprs_g"; }
        }

        /// <summary>
        /// Show the relative humidity at the chart's pressure level on image.
        /// </summary>
        /// <param name="contour">if set to <c>true</c> a contour chart will be drawn rather than a shaded chart.</param>
        /// <returns></returns>
        public static string LevelRelativeHumidity(bool contour = false)
        {
            return contour ? "rhprs c" : "rhprs";
        }

        /// <summary>
        /// Show convective action on the image.
        /// </summary>
        public static string ConvectiveAction
        {
            get { return "capesfc"; }
        }

        /// <summary>
        /// Show accumulated precipitation from the last 3 hours on the image.
        /// </summary>
        public static string PrecipitationLast3Hours
        {
            get { return "apcpsfc"; }
        }

        /// <summary>
        /// Show the cloud cover for the selected cloud level on the image.
        /// </summary>
        /// <param name="cl">The cloud level to use (low, mid, or high).</param>
        /// <param name="contour">If set to <c>true</c> a contour chart will be drawn rather than a shaded chart.</param>
        /// <returns></returns>
        public static string CloudCover(CloudLevel cl, bool contour = false)
        {
            return (contour ? "tccc " : "tcc ") + (char) cl.GetHashCode();
        }

        /// <summary>
        /// Sets the specified text as the chart's title.  This has no effect if you are not using map layout.
        /// </summary>
        /// <param name="text">The text to display as the title.</param>
        /// <returns></returns>
        public static string Title(string text)
        {
            return "title " + text;
        }

        /// <summary>
        /// Show the top altitude of clouds for the selected cloud level on the image.
        /// </summary>
        /// <param name="cl">The cloud level to use (low, mid, or high).</param>
        /// <returns></returns>
        public static string CloudTopAltitude(CloudLevel cl)
        {
            return "presclt " + (char)cl.GetHashCode();
        }

        /// <summary>
        /// Show the Deformation Vertical Shear Index (DVSI) between the chart's level and the one below it on the image.
        /// </summary>
        public static string DVSI
        {
            get { return "dvsi"; }
        }

        /// <summary>
        /// Show the cloud density at the selected level on the image.
        /// </summary>
        /// <param name="level">The pressure level in millibars to display cloud density for.</param>
        /// <param name="onlyAboveFreezing">if set to <c>true</c> will only show cloud density if above the freezing level.</param>
        /// <returns></returns>
        public static string CloudDensity(int level, bool onlyAboveFreezing = false)
        {
            return "incloud " + level + (onlyAboveFreezing ? "1" : null);
        }
    }

    public enum CloudLevel
    {
        Low = 'l',
        Mid = 'm',
        High = 'h'
    }
}