﻿using System;

namespace NWXNet
{
    public static class Available
    {
        public static Epochs Epochs
        {
            get { return new Epochs(); }
        }

        public static Levels Levels
        {
            get { return new Levels(); }
        }

        public static GeoMagModels GeoMagModels
        {
            get { return new GeoMagModels();}
        }
    }
}