using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnionSurvey.Data.Models;


namespace UnionSurvey.HostedServices
{
    public class ScheduledJobService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ScheduledJobService> _logger;

        public ScheduledJobService(IServiceScopeFactory serviceScopeFactory, ILogger<ScheduledJobService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<SurveyUnionContext>();

                        // Get the current time of day
                        //var now = DateTime.Now.TimeOfDay;
                        DateTime currentDate = DateTime.UtcNow;
                        TimeOnly nowTime = TimeOnly.FromDateTime(currentDate);
                        DateOnly nowDate = DateOnly.FromDateTime(currentDate);
                        string description = "";


                        // Find jobs scheduled to run at or before the current time, but not yet executed
                        var jobsToRun = dbContext.ScheduledJobs
                                                //.Where(job => job.LastExecutedDate < nowDate) //job.SchedualTime <= now &&
                                                .ToList();

                        foreach (var job in jobsToRun)
                        {
                            if (job.JobName == "SurveyCommisionCalculate")
                            {
                                var commissions = dbContext.TransactionInternals
                                                          .Where(i => //i.IsFirstSurveyCommision == false &&
                                                                       i.IsCommisionTransfered == false
                                                                      && i.TransactionDate == nowDate)//.AddDays(-1))
                                                          .ToList();

                                if (commissions.Count() > 0)
                                {

                                    var comUsers = commissions.Select(i => i.ToUserName).ToArray();
                                    var dbUsersFinancial = dbContext.UserFinancials
                                                                    .Where(i => comUsers.Contains(i.UserName))
                                                                    .ToList();


                                    foreach (var user in dbUsersFinancial)
                                    {
                                        foreach (var com in commissions.Where(i => i.ToUserName == user.UserName))
                                        {
                                            //Get last two surveys
                                            var surveyTnx = dbContext.Transactions
                                                                    .Where(i => i.Description == "Survey"
                                                                            && i.TransactionStatus == "COMPLETED"
                                                                            && i.TransAccountName == user.UserName
                                                                            && i.TransactionDate <= com.TransactionDate)
                                                                    .OrderByDescending(i => i.TransactionDate)
                                                                    .FirstOrDefault();

                                            bool isCompletedLastTwoSurveys = IsCompletedLastTwoSurveys(user, com, surveyTnx);
                                            description += user.UserName;
                                            if (isCompletedLastTwoSurveys)
                                            {
                                                description += $":{com.Amount}";

                                                user.TeamComission += com.Amount;
                                                com.IsCommisionTransfered = true;
                                            }
                                            description += "_";
                                        }
                                    }

                                    dbContext.UserFinancials.UpdateRange(dbUsersFinancial);
                                    dbContext.TransactionInternals.UpdateRange(commissions);
                                }
                            }

                            ScheduledJobLog log = new ScheduledJobLog
                            {
                                ScheduledJobId = job.Id,
                                ScheduledJobDate = DateTime.UtcNow,
                                Status = "COMPLETED",
                                Description = description
                            };

                            // Mark the job as executed
                            job.LastExecutedDate = nowDate;

                            dbContext.ScheduledJobLogs.Add(log);    
                            dbContext.ScheduledJobs.Update(job);
                        }

                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while executing scheduled jobs.");
                }

                // Wait for a minute before checking again
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private Task RunJobOfSurveyCommissionAsync(ScheduledJob job)
        {
            // Implement the logic for executing the job
            _logger.LogInformation($"Executing job: {job.JobName} at {DateTime.UtcNow}");
            return Task.CompletedTask; // Replace with actual job execution logic
        }

        private static bool IsCompletedLastTwoSurveys(UserFinancial user, TransactionInternal com, Transaction? surveyTnx)
        {
            //check today survey, commission and last suvey should be done.
            if (user.LastSurveyDate == null || com == null || surveyTnx == null) return false;


            DateOnly todaySurveyDate = DateOnly.FromDateTime(user.LastSurveyDate.Value);
            DateOnly todayDate = DateOnly.FromDateTime(DateTime.UtcNow);


            //DateOnly commisionDate = lastTwoSurveys[0].TransactionDate;
            //DateOnly secondLastCommisionDate = lastTwoSurveys[1].TransactionDate;
            //DateOnly comDate = com.TransactionDate;
            //DateOnly secondLastComDate = com.TransactionDate.AddDays(-1);

            // Return true only if the last two surveys completed from commission.
            return todaySurveyDate == todayDate && com.TransactionDate == surveyTnx.TransactionDate;
        }
    }
}
