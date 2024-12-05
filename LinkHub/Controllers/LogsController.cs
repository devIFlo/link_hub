using LinkHub.Data;
using LinkHub.Services;
using LinkHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace LinkHub.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LogsController : Controller
    {
        private readonly LogService _logService;

        public LogsController(LogService logService)
        {
            _logService = logService;
        }

        public IActionResult Index(DateTime? startDate, DateTime? endDate, int logLevel)
        {
            var logs = _logService.GetLogs();

            if (startDate.HasValue)
            {
                logs = logs.Where(log => log.Timestamp.HasValue && log.Timestamp.Value.Date >= startDate.Value.Date).ToList();
            }

            if (endDate.HasValue)
            {
                logs = logs.Where(log => log.Timestamp.HasValue && log.Timestamp.Value.Date <= endDate.Value.Date).ToList();
            }

            if (logLevel != 0)
            {
                logs = logs.Where(log => log.Level == logLevel).ToList();
            }

            var logViewModel = new LogViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                LogLevel = logLevel,
                LogEntries = logs
            };

            return View(logViewModel);
        }
    }
}
