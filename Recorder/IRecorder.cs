using Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recorder
{
    public interface IRecorder
    {
        IReadOnlyList<IActivity> Activities { get; }
        void StartActivity(IActivity? activity);
        Task<TimeSpan> GetTotalTime(IActivity activity, DateTime startTime);
    }
}
