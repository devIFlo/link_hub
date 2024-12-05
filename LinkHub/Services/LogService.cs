using LinkHub.Data;
using LinkHub.Models;

namespace LinkHub.Services
{
    public class LogService
    {
        private readonly ApplicationDbContext _context;

        public LogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<LogEntry> GetLogs()
        {
            return _context.Logs.ToList();
        }
    }
}