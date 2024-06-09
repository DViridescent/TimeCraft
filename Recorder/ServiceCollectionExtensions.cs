using Microsoft.Extensions.DependencyInjection;
using Recorder.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recorder;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRecorderService(this IServiceCollection services) => services
        .AddSingleton<IRecorder, RecorderService>();
}
