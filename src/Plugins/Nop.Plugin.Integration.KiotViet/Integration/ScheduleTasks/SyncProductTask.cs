using Nop.Core.Domain.Logging;
using Nop.Plugin.Integration.KiotViet.Integration.KiotViet;
using Nop.Services.Logging;
using Nop.Services.Tasks;

namespace Nop.Plugin.Integration.KiotViet.Integration.ScheduleTasks
{
    public class SyncProductTask : IScheduleTask
    {
        private KiotVietApiConsumer _apiConsumer;
        private ILogger _logger;
        public SyncProductTask(ILogger logger)
        {
            _logger = logger;
            _apiConsumer = new KiotVietApiConsumer();
        }

        public void Execute()
        {
            _logger.InsertLog(LogLevel.Information, _apiConsumer.GetToken());
        }
    }
}
