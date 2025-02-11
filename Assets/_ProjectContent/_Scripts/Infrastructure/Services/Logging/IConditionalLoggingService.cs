using System.Diagnostics;
using System.Linq;

namespace Infrastructure.Services.Logging
{
    // Fake interface for consistency in injection methods and for Conditional attribute
    public abstract class IConditionalLoggingService
    {
#if DEV
        private readonly LogTag[] _tagsToExclude = {};
        private readonly LogTag[] _warningTagsToExclude = {};
        private readonly LogTag[] _errorTagsToExclude = {LogTag.UnityServices};
#endif

        [Conditional("DEV")]
        public void Log(string text, LogTag tag = LogTag.Default)
        {
            if (_tagsToExclude.Contains(tag)) return;
            InternalLog(text, tag);
        }

        [Conditional("DEV")]
        public void LogWarning(string text, LogTag tag = LogTag.Default)
        {
            if (_warningTagsToExclude.Contains(tag)) return;
            InternalLogWarning(text, tag);
        }

        [Conditional("DEV")]
        public void LogError(string text, LogTag tag = LogTag.Default)
        {
            if (_errorTagsToExclude.Contains(tag)) return;
            InternalLogError(text, tag);
        }

        protected abstract void InternalLog(string text, LogTag tag);

        protected abstract void InternalLogWarning(string text, LogTag tag);

        protected abstract void InternalLogError(string text, LogTag tag);
    }
}