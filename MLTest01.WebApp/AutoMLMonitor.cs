using Microsoft.ML.AutoML;

namespace MLTest01.WebApp
{
    public class AutoMLMonitor : IMonitor
    {
        private readonly List<TrialResult> _completedTrials;
        private readonly SweepablePipeline _pipeline;
        private readonly ILogger<AutoMLMonitor> logger;

        public AutoMLMonitor(SweepablePipeline pipeline, ILogger<AutoMLMonitor> logger)
        {
            _completedTrials = new List<TrialResult>();
            _pipeline = pipeline;
            this.logger = logger;
        }

        public IEnumerable<TrialResult> GetCompletedTrials() => _completedTrials;

        public void ReportBestTrial(TrialResult result)
        {
            return;
        }

        public void ReportCompletedTrial(TrialResult result)
        {
            var trialId = result.TrialSettings.TrialId;
            var timeToTrain = result.DurationInMilliseconds;
            var pipeline = _pipeline.ToString(result.TrialSettings.Parameter);
            logger.LogDebug($"Trial {trialId} finished training in {timeToTrain}ms with pipeline {pipeline}");
            _completedTrials.Add(result);
        }

        public void ReportFailTrial(TrialSettings settings, Exception exception = null)
        {
            if (exception.Message.Contains("Operation was canceled."))
            {
                logger.LogDebug($"{settings.TrialId} cancelled. Time budget exceeded.");
            }
            logger.LogDebug($"{settings.TrialId} failed with exception {exception.Message}");
        }

        public void ReportRunningTrial(TrialSettings setting)
        {
            return;
        }
    }
}
