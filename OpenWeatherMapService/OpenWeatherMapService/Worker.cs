namespace OpenWeatherMapTwincat
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                var server = new OpenWeatherMapService(25734, "Twincat OpenWeatherMap Server", _logger);
                await server.ConnectServerAndWaitAsync(stoppingToken);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
