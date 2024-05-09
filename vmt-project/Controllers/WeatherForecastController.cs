using Azure;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using NetCore.Infrastructure.Api.Controller;
using Newtonsoft.Json;
using vmt_project.models.Request.User;

namespace vmt_project.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : BaseController
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpGet]
        [Route("test")]
        public async Task<IActionResult> Test()
        {
            var hubUrl = "https://localhost:7130/message";

            // Khởi tạo một kết nối Hub SignalR
            var connection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .Build();

            try
            {
                // Bắt đầu kết nối
                await connection.StartAsync();
                Console.WriteLine("Connected to Hub.");

                // Gửi một tin nhắn tới Hub
                await connection.SendAsync("SendMessage", "User", "Hello");
                Console.WriteLine("Message sent to Hub.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            await connection.StopAsync();
            return Ok();
        }
    }
}
