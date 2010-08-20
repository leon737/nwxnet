using System;
using System.Collections.Generic;
using System.Linq;

namespace NWXNet
{
    public class Chart : IRequestData
    {
        private Chart(int width, int height, int level, DateTime epoch, LatLon lowerLeftCoordinates, LatLon upperRightCoordinates)
        {
            Width = width;
            Height = height;
            Level = level;
            Epoch = epoch;
            LowerLeftCoordinates = lowerLeftCoordinates;
            UpperRightCoordinates = upperRightCoordinates;
        }

        internal int Width { get; private set; }
        internal int Height { get; private set; }
        internal int Level { get; private set; }
        internal DateTime Epoch { get; private set; }
        internal LatLon LowerLeftCoordinates { get; private set; }
        internal LatLon UpperRightCoordinates { get; private set; }

        private List<string> _features;
        internal List<string> Features
        {
            get { return _features ?? (_features = new List<string>()); }
            private set { _features = value; }
        }

        RequestTypes IRequestData.Type
        {
            get { return RequestTypes.Chart; }
        }

        bool IRequestData.IsValid
        {
            get
            {
                return (Width != 0) && (Height != 0) && (Level != 0) && (LowerLeftCoordinates != null) &&
                       (UpperRightCoordinates != null) && (Epoch != DateTime.MinValue) && Features.Any();
            }
        }

        public string Id { get; private set; }

        internal bool MapLayout { get; private set; }

        internal bool TransparentBackground { get; private set; }

        public static Chart For(LatLon lowerLeftCoordinates, LatLon upperRightCoordinates, int level, DateTime epoch, int width, int height)
        {
            return new Chart(width, height, level, epoch, lowerLeftCoordinates, upperRightCoordinates);
        }

        public static Chart For(string lowerLeftCoordinates, string upperRightCoordinates, int level, DateTime epoch, int width, int height)
        {
            return new Chart(width, height, level, epoch, new LatLon(lowerLeftCoordinates), new LatLon(upperRightCoordinates));
        }

        public Chart WithId(string id)
        {
            Id = id;
            return this;
        }

        public Chart UsingMapLayout
        {
            get
            {
                MapLayout = true;
                return this;
            }
        }

        // TODO: Why does this cause the server to hangup?
        public Chart WithTransparentBackground
        {
            get 
            { 
                TransparentBackground = true;
                return this;
            }
        }

        public Chart WithFeatures(params string[] features)
        {
            Features.AddRange(features);
            return this;
        }
    }
}