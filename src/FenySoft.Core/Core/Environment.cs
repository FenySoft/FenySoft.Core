﻿namespace FenySoft.Core
{
    public static class Environment
    {
        public static readonly bool RunningOnMono = Type.GetType("Mono.Runtime") != null;
    }
}
