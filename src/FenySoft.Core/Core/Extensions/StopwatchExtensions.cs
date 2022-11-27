﻿using System.Diagnostics;

namespace FenySoft.Core.Extensions
{
    public static class StopwatchExtensions
    {
        public static double GetSpeed(this Stopwatch sw, long count)
        {
            return count / (sw.ElapsedMilliseconds / 1000.0);
        }
    }
}
