using System.Runtime.InteropServices;

namespace OpenWeatherMapTwincat.Api
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct OpenWeatherMapResponse
    {
        public int ConditionLength;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public char[] Condition;

        public double TemperatureInCelsius;
        public double FeelsLikeInCelsius;
        public double PressureInBar;
        public double RelativeHumidityInPercent;
        public double MinimumTemperatureInCelsius;
        public double MaximumTemperatureInCelsius;
        public double SeaLevelPressureInBar;
        public double GroundLevelPressureInBar;
        public double WindSpeedInMeterPerSecond;
        public double WindDirectionInDegree;
        public double WindGustInMeterPerSecond;
        public double CloudinessInPercent;
        public double RainfallLastHourInMillimeter;
        public double RainfallLastThreeHoursInMillimeter;
        public double SnowfallLastHourInMillimeter;
        public double SnowfallLastThreeHoursInMillimeter;
        public long SunriseUtc;
        public long SunsetUtc;

        public long UtcOffsetInMilliseconds;
        public int CityId;

        public int CityNameLength;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public char[] CityName;

        public long FetchedAtUtc;
        public long MeasuredAtUtc;

        public int ExceptionLength;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public char[] Exception;

        public ushort Error;
    }
}
