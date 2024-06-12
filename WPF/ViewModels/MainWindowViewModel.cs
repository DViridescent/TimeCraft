using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Objects;
using Recorder;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using WPF.Interfaces;

namespace WPF;

internal partial class MainWindowViewModel : ObservableObject
{
    private readonly IRecorder _recorder;

    public MainWindowViewModel(IRecorder recorder)
    {
        _recorder = recorder;

        Blocks = [];
        InitializeBlocks(recorder.Activities);
    }

    private async void InitializeBlocks(IReadOnlyList<IActivity> activities)
    {
        foreach (var activity in activities)
        {
            var startTime = await _recorder.GetTotalTime(activity, DateTime.Today);
            Blocks.Add(new BlockViewModel(activity, startTime));
        }
    }

    [ObservableProperty]
    private BlockViewModel? _currentBlock;
    partial void OnCurrentBlockChanged(BlockViewModel? oldValue, BlockViewModel? newValue)
    {
        _recorder.StartActivity(newValue?.Activity);
        oldValue?.Stop();
        newValue?.Start();
    }

    public ObservableCollection<BlockViewModel> Blocks { get; private set; }

    [RelayCommand]
    private void Output()
    {
        // 将每个块的时间输出到%AppData%/TimeManager/Blocks.txt
        var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TimeManager");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        // 文件夹为月份
        folderPath = Path.Combine(folderPath, DateTime.Now.ToString("yyyy-MM"));
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // 文件名为日期加星期
        var fileName = DateTime.Now.ToString("dd dddd") + ".md";

        var path = Path.Combine(folderPath, fileName);
        var sb = new StringBuilder();
        sb.AppendLine($"# {DateTime.Now:D}");
        sb.AppendLine();
        TimeSpan total = TimeSpan.Zero;
        foreach (var block in Blocks)
        {
            total += block.Duration;
        }
        sb.AppendLine($"今天共计工作了{total.Hours}小时{total.Minutes}分钟，其中：");

        foreach (var block in Blocks)
        {
            var minutes = block.Duration.Minutes;
            if (block.Duration.Seconds >= 30)
            {
                minutes++;
            }
            sb.AppendLine($"- {block.Name}: {block.Duration.Hours}小时{minutes}分钟");
        }
        File.WriteAllText(path, sb.ToString());
        System.Diagnostics.Process.Start("explorer.exe", folderPath);
    }

    [RelayCommand]
    private void Close(ICloseable closable)
    {
        closable.Close();
    }

    [ObservableProperty]
    private bool _showBack;
    [RelayCommand]
    private void Switch()
    {
        ShowBack = !ShowBack;
    }
}
