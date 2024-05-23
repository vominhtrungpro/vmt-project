using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Models.Entities;
using vmt_project.services.Contracts;

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
        Console.WriteLine($"[{DateTime.UtcNow}]---START CRONJOB---");
        while (true)
        {
            try
            {
                var userCount = _cronjobService.CountUser();
                Console.WriteLine($"NOW is [{DateTime.UtcNow}], num of users is {userCount}");
                Thread.Sleep(_sleepingTime * 10000);
            }
            catch (Exception)
            {
            }
        }
    }
}
