using Microsoft.Extensions.Hosting;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Models.Entities;
using vmt_project.services.Contracts;
using vmt_project.services.Elastic;

namespace vmt_project.cronjob_search
{
    public class Worker : BackgroundService
    {
        private readonly int _sleepingTime;
        private readonly ICronjobService _cronjobService;
        private readonly ElasticClient _elasticClient;
        private readonly ICharacterElasticService _characterElasticService;
        public Worker(ICronjobService cronjobService, ElasticClient elasticClient,ICharacterElasticService characterElasticService)
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
            _characterElasticService = characterElasticService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"[{DateTime.UtcNow}]---START SEARCH CRONJOB---");
            var lastRunTime = DateTime.MinValue;
            while (true)
            {
                try
                {
                    var characters = await _cronjobService.ElasticSearchCharacter(lastRunTime);
                    if (characters.Count > 0)
                    {
                        var indexResponses = await _characterElasticService.BulkInsert(characters);
                        if (!indexResponses)
                        {
                            Console.WriteLine($"Error indexing character!");
                        }
                        foreach (var character in characters)
                        {
                            var indexResponse = await _characterElasticService.Insert(character);
                            if (!indexResponse)
                            {
                                Console.WriteLine($"Error indexing character!");
                            }
                            if (lastRunTime < character.ModifiedOn)
                            {
                                lastRunTime = character.ModifiedOn ?? DateTime.Now;
                            }
                        }
                        Console.WriteLine($"Success index " + characters.Count.ToString() + " characters!");
                    }
                    else
                    {
                        Console.WriteLine($"No character to index!");
                    }

                    Thread.Sleep(_sleepingTime * 1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
