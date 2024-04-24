using Azure;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
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
            try
            {
                var config = new ProducerConfig { BootstrapServers = "localhost:9092" };

                using var producer = new ProducerBuilder<Null, string>(config).Build();
                try
                {
                    var response = await producer.ProduceAsync("weather",
                    new Message<Null, string>
                    {
                        Value = "message"
                    });
                    Console.WriteLine(response.Value);
                    return Ok(response.Value);

                }
                catch (ProduceException<Null, string> ex)
                {
                    return Ok(ex.Message);
                }
                return Ok();

            }
            catch (Exception ex)
            {
                return Success(ex.StackTrace);
            }
            finally
            {
            }
        }
    }
}
