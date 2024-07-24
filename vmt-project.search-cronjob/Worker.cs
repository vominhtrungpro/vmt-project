using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.services.Contracts;

namespace vmt_project.search_cronjob
{
    public class Worker : BackgroundService
    {
        private readonly int _sleepingTime;
        private readonly ICronjobService _cronjobService;
        public Worker(ICronjobService cronjobService)
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
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"[{DateTime.UtcNow}]---START SEARCH CRONJOB---");
            while (true)
            {
                try
                {
                    Console.WriteLine($"NOW is [{DateTime.UtcNow}]");
                    Thread.Sleep(_sleepingTime * 10000);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
