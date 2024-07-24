using Microsoft.Extensions.Hosting;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Models.Entities;
using vmt_project.services.Contracts;

namespace vmt_project.cronjob_search
{
    public class Worker : BackgroundService
    {
        private readonly int _sleepingTime;
        private readonly ICronjobService _cronjobService;
        private readonly ElasticClient _elasticClient;
        public Worker(ICronjobService cronjobService, ElasticClient elasticClient)
        {
            _cronjobService = cronjobService;
            var sleepingTime = Environment.GetEnvironmentVariable("CronJobSettings:SleepingTime");
            if (String.IsNullOrEmpty(sleepingTime))
            {
                _sleepingTime = 5;
            }
            else
            {
                _sleepingTime = int.Parse(sleepingTime);
            }
            _elasticClient = elasticClient;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"[{DateTime.UtcNow}]---START SEARCH CRONJOB---");
            //var settings = new ConnectionSettings(new Uri("https://c1e26596e2114447af4c0f0224e4a5de.eastus2.azure.elastic-cloud.com/"))
            //    .DefaultIndex("character-index")
            //    .BasicAuthentication("elastic", "LmKZuwze4VkJotLKRTtqbo2l");

            //var client = new ElasticClient(settings);
            var lastRunTime = DateTime.MinValue;
            while (true)
            {
                try
                {
                    var characters = await _cronjobService.ElasticSearchCharacter(lastRunTime);
                    foreach (var character in characters) 
                    {
                        var indexResponse = await _elasticClient.IndexDocumentAsync(character);
                        if (!indexResponse.IsValid)
                        {
                            Console.WriteLine($"Error indexing user {character.Id}: {indexResponse.ServerError}");
                        }
                        if (lastRunTime < character.ModifiedOn)
                        {
                            lastRunTime = character.ModifiedOn ?? DateTime.Now;
                        }
                    }
                    Console.WriteLine($"Success index "+characters.Count.ToString()+" characters!");
                    Thread.Sleep(_sleepingTime * 1000);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
