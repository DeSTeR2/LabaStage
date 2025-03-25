using Microsoft.Identity.Client;

namespace HospitalMVC.HospitalInfrastructure.Services
{
    public class RefreshService : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly TimeSpan _delay = TimeSpan.FromSeconds(Utils.Constants.RefreshAppointmentStateInMinutes);

        public RefreshService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RefreshAppointmentsAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                await Task.Delay(_delay, stoppingToken);
            }
        }

        private async Task RefreshAppointmentsAsync()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseUrl"] ?? "https://localhost:44326/");
            var response = await client.PostAsync("/Appointments/UpdateAppointmentsState", null);
        }
    }
}
