namespace FenySoft.Core
{
    public static class TEnvironment
    {
        public static readonly bool RunningOnMono = Type.GetType("Mono.Runtime") != null;
    }
}
