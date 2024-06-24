using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PiHire.BAL.Common.Types;
using PiHire.BAL.IRepositories;
using PiHire.BAL.ViewModels;
using Serilog;

namespace PiHire.WS
{
    public class cronWorker : BackgroundService
    {
        public static DateTime currentTime { get { return BAL.Repositories.BaseRepository.CurrentTime; /*TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, UAETimeZone);*/ } }
        private readonly ILogger<cronWorker> _logger;
        private BAL.Common.Extensions.AppSettings appSettings;
        private IConfiguration configuration;

        private List<Timer> eventTimers = new List<Timer>();
        private Dictionary<int, int> runningEvents = new Dictionary<int, int>();
        public static List<ScheduleServiceViewModel> schedules = new List<ScheduleServiceViewModel>();
        string connectString;

        public cronWorker(ILogger<cronWorker> logger, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _logger = logger;
            this.configuration = configuration;
            connectString = configuration["ConnectionStrings:dbConnection"];

            var SettingProperties = configuration.GetSection("AppSettings");
            var appSettingsProperties = SettingProperties.Get<BAL.Common.Extensions.AppSettingsProperties>();
            var SmtpEmailConfig = configuration.GetSection("SmtpEmailConfig");
            var appSmtpEmailConfig = SmtpEmailConfig.Get<BAL.Common.Extensions.SmtpEmailConfig>();
            this.appSettings = new BAL.Common.Extensions.AppSettings(appSettingsProperties, appSmtpEmailConfig);
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");
            return base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Background Service is executing.");
            if (Environment.UserInteractive)
            {
            }
            else
            {

            }
            #region InfoBipStatus
            {
                var dueTime = TimeSpan.Zero;
                var period = TimeSpan.FromMinutes(15);
                ScheduleTrigger trigger = new ScheduleTrigger(0, getWsService, _logger, this, isInfoBipStatus: true);
                eventTimers.Add(new Timer(trigger.Callback, null, dueTime, period));
                runningEvents.Add(0, eventTimers.Count - 1);
                _logger.LogInformation("newSchedule. scheduleId:" + 0);
            }
            #endregion
            #region TmpAllCandidatesUpdate
            {
                var dueTime = TimeSpan.Zero;
                var period = TimeSpan.FromMinutes(2);
                ScheduleTrigger trigger = new ScheduleTrigger(0, getWsService, _logger, this, isTmpAllCandidatesUpdate: true);
                eventTimers.Add(new Timer(trigger.Callback, null, dueTime, period));
                runningEvents.Add(-1, eventTimers.Count - 1);
                _logger.LogInformation("newSchedule. scheduleId:" + -1);
            }
            #endregion
            #region RecruiterJobAssignmentsCarryForward
            {
                var dueTime = TimeSpan.Zero;
                var period = TimeSpan.FromHours(1);
                ScheduleTrigger trigger = new ScheduleTrigger(0, getWsService, _logger, this, isRecruiterJobAssignmentsCarryForward: true);
                eventTimers.Add(new Timer(trigger.Callback, null, dueTime, period));
                runningEvents.Add(-2, eventTimers.Count - 1);
                _logger.LogInformation("newSchedule. scheduleId:" + -2);
            }
            #endregion
            _logger.LogInformation("IsCancellationRequested:" + stoppingToken.IsCancellationRequested);
            while (!stoppingToken.IsCancellationRequested)
            {
                updateScheduleList().Wait();
                await Task.Delay(taskListUpdate_millSec);
            }
        }
        int taskListUpdate_millSec = 1000 * 60 * 5;
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");
            foreach (var _timer in eventTimers)
                _timer?.Change(Timeout.Infinite, 0);

            return base.StopAsync(cancellationToken);
        }
        public override void Dispose()
        {
            foreach (var _timer in eventTimers)
                _timer?.Dispose();
            base.Dispose();
        }


        private async Task updateScheduleList()
        {
            try
            {
                using (IWsServiceRepository wsSrvc = getWsService())
                {
                    var newSchedules = await wsSrvc.GetScheduleListSync();
                    _logger.LogInformation("newSchedules:" + Newtonsoft.Json.JsonConvert.SerializeObject(newSchedules));
                    if (schedules == null)
                        schedules = newSchedules;
                    else
                    {
                        var existingSc = schedules.Select(da => da.Id).ToList();
                        foreach (var newSc in newSchedules.Where(da =>
                                    (da.Frequency == CustomSchedulerFrequency.Daily) ||
                                    (da.Frequency == CustomSchedulerFrequency.DateAndTime &&
                                        (
                                            (da.ScheduleDate > currentTime && da.ScheduleDate < currentTime.AddMilliseconds(taskListUpdate_millSec + (1000 * 60)))
                                            || (da.AnySubPending && da.ScheduleDate < currentTime)
                                            || (da.IsMissed && da.ScheduleDate < currentTime)
                                        )
                                    )))
                        {
                            if (schedules.FirstOrDefault(da => da.Id == newSc.Id) == null)
                            {
                                if (newSchedule(newSc))
                                {
                                    schedules.Add(newSc);
                                    _logger.LogInformation("newSchedule. scheduleId:" + newSc.Id);
                                }
                            }
                            else
                                existingSc.Remove(newSc.Id);
                        }
                        foreach (var scheduleId in existingSc)
                        {
                            removeSchedule(scheduleId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "updateScheduleList error");
            }
        }
        private bool newSchedule(ScheduleServiceViewModel schedule)
        {
            try
            {
                var dueTime = TimeSpan.Zero;

                var period = TimeSpan.FromSeconds(5);
                switch (schedule.Frequency)
                {
                    case CustomSchedulerFrequency.Daily:
                        {
                            period = TimeSpan.FromDays(1);
                            if (schedule.ScheduleDate.Ticks > currentTime.Ticks)
                            {
                                dueTime = new TimeSpan(schedule.ScheduleDate.Ticks - currentTime.Ticks);
                            }
                            else
                            {
                                var dt = currentTime.Date;
                                dt.AddHours(schedule.ScheduleDate.Hour);
                                dt.AddMinutes(schedule.ScheduleDate.Minute);
                                dt.AddSeconds(schedule.ScheduleDate.Second);
                                while (dt.Ticks < currentTime.Ticks)
                                {
                                    dt = dt.AddDays(1);
                                }
                                dueTime = new TimeSpan(dt.Ticks - currentTime.Ticks);
                            }
                        }
                        break;
                    case CustomSchedulerFrequency.DateAndTime:
                        {
                            period = TimeSpan.FromDays(1);
                            if (schedule.ScheduleDate.Ticks > currentTime.Ticks)
                            {
                                dueTime = new TimeSpan(schedule.ScheduleDate.Ticks - currentTime.Ticks);
                            }
                            else if ((schedule.AnySubPending && currentTime.Ticks > schedule.ScheduleDate.Ticks)
                                || (schedule.IsMissed && currentTime.Ticks > schedule.ScheduleDate.Ticks))
                            {
                                dueTime = new TimeSpan(0, 1, 0);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        break;
                }
                switch (schedule.Event)
                {
                    case CustomSchedulerEventTypes.NJ:
                        period = TimeSpan.FromHours(3);
                        break;
                    case CustomSchedulerEventTypes.BD:
                        break;
                    case CustomSchedulerEventTypes.SD:
                        break;
                    case CustomSchedulerEventTypes.EH:
                        break;
                    default:
                        break;
                }

                ScheduleTrigger trigger = new ScheduleTrigger(schedule.Id, getWsService, _logger, this);
                eventTimers.Add(new Timer(trigger.Callback, null, dueTime, period));
                runningEvents.Add(schedule.Id, eventTimers.Count - 1);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "newSchedule creation failed. schedule:" + Newtonsoft.Json.JsonConvert.SerializeObject(schedule));
                return false;
            }
        }
        public bool removeSchedule(int scheduleId)
        {
            try
            {
                if (runningEvents.ContainsKey(scheduleId))
                {
                    var currentIndx = runningEvents[scheduleId];
                    var rearrangeSet = runningEvents.Where(da => da.Value > currentIndx).Select(da => da.Key).ToList();
                    foreach (var Key in rearrangeSet)
                    {
                        runningEvents[Key] = runningEvents[Key] - 1;
                    }

                    var tmTrigger = eventTimers[currentIndx];
                    tmTrigger.Change(Timeout.Infinite, 0);
                    tmTrigger.Dispose();
                    eventTimers.Remove(eventTimers[currentIndx]);
                    runningEvents.Remove(scheduleId);
                }
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "removeSchedule failed. scheduleId:" + scheduleId);
                return false;
            }
        }
        private IWsServiceRepository getWsService()
        {
            var db = BAL.Repositories.BaseRepository.getDatabase(connectString);

            var _logFile = new LoggerConfiguration()
                               .ReadFrom.Configuration(configuration)
                               .Enrich.FromLogContext()
                               //.WriteTo.Console()
                               .CreateLogger();

            var logFile = new Serilog.Extensions.Logging.SerilogLoggerProvider(_logFile, true);

            return BAL.Repositories.WsServiceRepository.Instance(db, logFile, appSettings);
        }
    }


    public class ScheduleTrigger
    {
        private int scheduleId;
        private ILogger<cronWorker> logger;
        private Func<IWsServiceRepository> getWsService;
        private cronWorker cronWorker;
        private bool isInfoBipStatus;
        private bool isTmpAllCandidatesUpdate;
        private bool isRecruiterJobAssignmentsCarryForward;

        public ScheduleTrigger(int scheduleId, Func<IWsServiceRepository> getWsService, ILogger<cronWorker> logger, cronWorker cronWorker, bool isInfoBipStatus = false, bool isTmpAllCandidatesUpdate = false, bool isRecruiterJobAssignmentsCarryForward = false)
        {
            this.scheduleId = scheduleId;
            this.getWsService = getWsService;
            this.logger = logger;
            this.cronWorker = cronWorker;
            this.isInfoBipStatus = isInfoBipStatus;
            this.isTmpAllCandidatesUpdate = isTmpAllCandidatesUpdate;
            this.isRecruiterJobAssignmentsCarryForward= isRecruiterJobAssignmentsCarryForward;
        }

        public void Callback(object state)
        {
            if (isInfoBipStatus)
            {
                using (var wsSrvc = getWsService.Invoke())
                {
                    logger.LogDebug("Timed Background Service InfoBipStatus is started.");
                    wsSrvc.UpdateInfoBipStatus().Wait();
                    logger.LogDebug("Timed Background Service InfoBipStatus is completed.");
                }
            }
            else if (isTmpAllCandidatesUpdate)
            {
                using (var wsSrvc = getWsService.Invoke())
                {
                    logger.LogDebug("Timed Background Service TmpAllCandidatesUpdate is started.");
                    wsSrvc.TmpAllCandidatesUpdate().Wait();
                    logger.LogDebug("Timed Background Service TmpAllCandidatesUpdate is completed.");
                }
            }
            else if (isRecruiterJobAssignmentsCarryForward)
            {
                using (var wsSrvc = getWsService.Invoke())
                {
                    logger.LogDebug("Timed Background Service RecruiterJobAssignmentsCarryForward is started.");
                    wsSrvc.RecruiterJobAssignmentsCarryForwardAsync().Wait();
                    logger.LogDebug("Timed Background Service RecruiterJobAssignmentsCarryForward is completed.");
                }
            }
            else
            {
                var dat = cronWorker.schedules.FirstOrDefault(da => da.Id == this.scheduleId);
                using (var wsSrvc = getWsService.Invoke())
                {
                    logger.LogDebug("Timed Background Service {0} is started.", dat.Id);
                    switch (dat.Event)
                    {
                        case CustomSchedulerEventTypes.BD:
                            wsSrvc.BirthdayAsync(this.scheduleId).Wait();
                            break;
                        case CustomSchedulerEventTypes.EH:
                            wsSrvc.EventHappendAsync(this.scheduleId).Wait();
                            break;
                        case CustomSchedulerEventTypes.NJ:
                            wsSrvc.NewJobPublishedAsync(this.scheduleId).Wait();
                            break;
                        case CustomSchedulerEventTypes.SD:
                            wsSrvc.SpecialDayAsync(this.scheduleId).Wait();
                            break;
                        default:
                            break;
                    }
                    logger.LogDebug("Timed Background Service {0} is completed.", dat.Id);
                    switch (dat.Frequency)
                    {
                        case CustomSchedulerFrequency.Daily:
                            break;
                        case CustomSchedulerFrequency.DateAndTime:
                            logger.LogDebug("Stopping the Background Service {0}.", dat.Id);
                            logger.LogDebug("Stopping the Background Service {0} status:{1}", dat.Id, this.cronWorker.removeSchedule(dat.Id));
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
