using LinkHub.Data;
using Microsoft.AspNetCore.Mvc;

namespace LinkHub.Controllers
{
    public class LogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var logs = _context.Logs
            .OrderByDescending(log => log.Timestamp)
            .ToList();

            return View(logs);
        }
    }
}
