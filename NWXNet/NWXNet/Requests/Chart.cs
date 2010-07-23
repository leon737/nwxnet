using System;
using System.Collections.Generic;

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

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Level { get; private set; }
        public DateTime Epoch { get; private set; }
        public LatLon LowerLeftCoordinates { get; private set; }
        public LatLon UpperRightCoordinates { get; private set; }

        private List<string> _features;
        public List<string> Features
        {
            get { return _features ?? (_features = new List<string>()); }
            private set { Features = value; }
        }

        public RequestTypes Type
        {
            get { return RequestTypes.Chart; }
        }

        public bool IsValid
        {
            get { return true; }
        }

        public string Id { get; private set; }

        public bool MapLayout { get; private set; }

        public bool TransparentBackground { get; private set; }

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