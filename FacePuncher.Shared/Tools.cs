using System;
using System.Collections.Generic;

using FacePuncher.Geometry;

namespace FacePuncher
{
    public static class Tools
    {
        public static int Clamp(this int val, int min, int max)
        {
            return val < min ? min : val > max ? max : val;
        }
    }
}
