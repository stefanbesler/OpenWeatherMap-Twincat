using System.Runtime.InteropServices;
using TwinCAT.Ads.Server;
using TwinCAT.Ads;
using System.Text;

namespace OpenWeatherMapTwincat
{
    /*
     * Extend the TcAdsServer class to implement ADS Server
     */
    public class OpenWeatherMapService : AdsServer
    {
        public OpenWeatherMapService(ushort port, string portName, ILogger logger) : base(port, portName, logger)
        {

        }

        ~OpenWeatherMapService()
        {
        }


        protected override void OnConnected()
        {
        }

        protected override async Task<ResultReadWriteBytes> OnReadWriteAsync(AmsAddress sender, uint invokeId, uint indexGroup, uint indexOffset, int readLength, ReadOnlyMemory<byte> requestData, CancellationToken cancel)
        {
            ResultReadWriteBytes result = ResultReadWriteBytes.CreateError(AdsErrorCode.DeviceServiceNotSupported);

            // use index group (and offset) to distinguish between the servicesof this server
            if (indexGroup + indexOffset == 0)
            {
                IntPtr requestPtr = Marshal.AllocHGlobal(requestData.Length);
                byte[] arr = new byte[requestData.Length];
                requestData.CopyTo(arr);

                int responseSize = Marshal.SizeOf<Api.OpenWeatherMapResponse>();
                byte[] responseData = new byte[responseSize];
                IntPtr responsePtr = Marshal.AllocHGlobal(responseSize);

                var request = new Api.OpenWeatherMapRequest();
                Marshal.Copy(arr, 0, requestPtr, Marshal.SizeOf(request));
                request = Marshal.PtrToStructure<Api.OpenWeatherMapRequest>(requestPtr);

                if(request == null)
                {
                    return ResultReadWriteBytes.CreateError((AdsErrorCode)(0xFFFF));
                }

                try
                {
                    var appId = Encoding.ASCII.GetString(request.Token.AsSpan(0, request.TokenLength).ToArray());
                    var zipCode = Encoding.ASCII.GetString(request.ZipCode.AsSpan(0, request.ZipCodeLength).ToArray());
                    var countryCode = Encoding.ASCII.GetString(request.CountryCode.AsSpan(0, request.CountryCodeLength).ToArray());
                    var client = new OpenWeatherMap.Cache.OpenWeatherMapCache(appId, 9_500);

                    var currentWeather = await client.GetReadingsAsync(new OpenWeatherMap.Cache.Models.ZipCode(zipCode, countryCode));
                    var cityName = currentWeather.CityName.AsSpan(0, 256).ToArray();
                    var condition = currentWeather.Weather.FirstOrDefault()?.Description.AsSpan(0, 256).ToArray() ?? "Unknown".AsSpan().ToArray();
                    var exception = currentWeather.Exception.Message.AsSpan(0, 256).ToArray();

                    var response = new Api.OpenWeatherMapResponse
                    {
                        CityId = currentWeather.CityId,
                        CityNameLength = cityName.Length,
                        CityName = cityName,
                        CloudinessInPercent = currentWeather.Cloudiness.Value,
                        Error = currentWeather.IsSuccessful ? (ushort)1 : (ushort)0,
                        FeelsLikeInCelsius = currentWeather.FeelsLike.DegreesCelsius,
                        FetchedAtUtc = currentWeather.FetchedTime.ToUniversalTime().Ticks,
                        MeasuredAtUtc = currentWeather.MeasuredTime.ToUniversalTime().Ticks,
                        SunriseUtc = currentWeather.Sunrise.Ticks,
                        SunsetUtc = currentWeather.Sunset.Ticks,
                        GroundLevelPressureInBar = currentWeather.GroundLevelPressure.HasValue ? currentWeather.GroundLevelPressure.Value.Bars : 0,
                        SeaLevelPressureInBar = currentWeather.SeaLevelPressure.HasValue ? currentWeather.SeaLevelPressure.Value.Bars : 0,
                        PressureInBar = currentWeather.Pressure.Bars,
                        MaximumTemperatureInCelsius = currentWeather.MaximumTemperature.DegreesCelsius,
                        MinimumTemperatureInCelsius = currentWeather.MinimumTemperature.DegreesCelsius,
                        TemperatureInCelsius = currentWeather.Temperature.DegreesCelsius,
                        RainfallLastHourInMillimeter = currentWeather.RainfallLastHour.HasValue ? currentWeather.RainfallLastHour.Value.Millimeters : 0,
                        RainfallLastThreeHoursInMillimeter = currentWeather.RainfallLastThreeHours.HasValue ? currentWeather.RainfallLastThreeHours.Value.Millimeters : 0,
                        RelativeHumidityInPercent = currentWeather.Humidity.Percent,
                        WindSpeedInMeterPerSecond = currentWeather.WindSpeed.MillimetersPerSecond,
                        WindGustInMeterPerSecond = currentWeather.WindGust.HasValue ? currentWeather.WindGust.Value.MillimetersPerSecond : 0,
                        SnowfallLastHourInMillimeter = currentWeather.SnowfallLastHour.HasValue ? currentWeather.SnowfallLastHour.Value.Millimeters : 0,
                        SnowfallLastThreeHoursInMillimeter = currentWeather.SnowfallLastThreeHours.HasValue ? currentWeather.SnowfallLastThreeHours.Value.Millimeters : 0,
                        WindDirectionInDegree = currentWeather.WindDirection.Degrees,
                        UtcOffsetInMilliseconds = currentWeather.TimeZoneOffset.Milliseconds,
                        Condition = condition,
                        ConditionLength = condition.Length,
                        Exception = exception,
                        ExceptionLength = exception.Length
                    };


                    Marshal.StructureToPtr(responseData, responsePtr, false);
                    Marshal.Copy(responsePtr, responseData, 0, responseSize);
                    result = ResultReadWriteBytes.CreateSuccess(responseData);
                }
                catch (Exception ex)
                {
                    result = ResultReadWriteBytes.CreateError((AdsErrorCode)(0xFFFF));
                }
                finally
                {
                    Marshal.FreeHGlobal(requestPtr);
                    Marshal.FreeHGlobal(responsePtr);
                }
   
            }

            return result;
        }
    }
}
