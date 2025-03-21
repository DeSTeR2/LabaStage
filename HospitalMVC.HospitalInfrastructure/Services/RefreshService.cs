using Microsoft.Identity.Client;

namespace HospitalMVC.HospitalInfrastructure.Services
{
    public class RefreshService : BackgroundService
    {
        private readonly ILogger<RefreshService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TimeSpan _delay = new TimeSpan(0, 0, Utils.Constants.RefreshAppointmentStateInMinutes);

        public RefreshService(ILogger<RefreshService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
            await Task.Run(async () =>
            {
                _logger.LogInformation("Refresh started");
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        await RefreshAppointmentsAsync();
                        _logger.LogInformation("Appointments refreshed successfully.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error refreshing appointments.");
                    }

                    await Task.Delay(_delay, stoppingToken);
                }

                _logger.LogInformation("Appointment Refresh Service stopped.");
            });
        }

        private async Task RefreshAppointmentsAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync("/Appointments/UpdateAppointmentsState", null);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Appointments refreshed via server-side call.");
            }
            else
            {
                _logger.LogWarning("Failed to refresh appointments. Status: {StatusCode}", response.StatusCode);
            }
        }
    }
}
