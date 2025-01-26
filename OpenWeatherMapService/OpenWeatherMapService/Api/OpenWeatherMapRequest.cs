using System.Runtime.InteropServices;

namespace OpenWeatherMapTwincat.Api
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    internal class OpenWeatherMapRequest
    {
        public ushort TokenLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 81)]
        public byte[] Token;

        public ushort ZipCodeLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] ZipCode;

        public ushort CountryCodeLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] CountryCode;
    }
}
