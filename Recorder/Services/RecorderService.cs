using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Objects;
using Recorder.Database;
using System.Diagnostics;
using System.Timers;
using Activity = Objects.Activity;
using Timer = System.Timers.Timer;

namespace Recorder.Services
{
    internal class RecorderService : IRecorder
    {
        private readonly List<IActivity> _activities =
        [
            Activity.Coding,
            Activity.CodeReview,
            Activity.Assistance,
            Activity.Meeting,
            Activity.Eating,
            Activity.Slacking
        ];
        private static string DbPath
        {
            get
            {
                var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TimeManager");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                };
                return Path.Combine(folderPath, "TimeManager.db");
            }
        }

        private readonly DataContext _dbContext;
        private Timer _timer = null!;
        private IActivity? _currentActivity = Activity.Alive;

        public RecorderService()
        {
            var connectionString = new SqliteConnectionStringBuilder() { DataSource = DbPath, }.ConnectionString;
            var options = new DbContextOptionsBuilder()
                .UseSqlite(connectionString)
                .Options;
            _dbContext = new DataContext(options);
            _dbContext.Database.EnsureCreated(); // 未来可能使用迁移

            SetupTimer();
        }

        public IReadOnlyList<IActivity> Activities => _activities.AsReadOnly();

        public void StartActivity(IActivity? activity)
        {
            if (activity is not null && !_activities.Contains(activity))
            {
                throw new ArgumentException("Activity not found.", nameof(activity));
            }

            _currentActivity = activity;
        }

        public async Task<TimeSpan> GetTotalTime(IActivity activity, DateTime startTime)
        {
            // 首先找到在那个时刻之前开始的最后一个项目
            var firstBlock = await _dbContext.TimeBlocks
                .OrderByDescending(i => i.Id)
                .Where(i => i.StartTime < startTime)
                .FirstOrDefaultAsync();

            if (firstBlock is null && _dbContext.TimeBlocks.Any())
            {
                // 如果找不到，但数据库中有项目，那么startTime太早了，我们需要找到最早的项目
                firstBlock = await _dbContext.TimeBlocks
                    .OrderBy(i => i.Id)
                    .FirstOrDefaultAsync();
            }

            if (firstBlock != null)
            {
                if (firstBlock.EndTime < startTime)
                {
                    // 如果找到的项目的结束时间比startTime早，那么这个项目不是我们要找的
                    // 我们需要找到这个项目之后的项目
                    firstBlock = await _dbContext.FindAsync<TimeBlock>(firstBlock.Id + 1);
                    // 如果那之后就无了，说明那就是最后一个了，那就返回0
                    if (firstBlock is null)
                    {
                        return TimeSpan.Zero;
                    }
                }

                // 然后查询从这个项目到最后的所有项目
                var itemsFromPivotalToEnd = await _dbContext.TimeBlocks
                    .Where(i => i.Id >= firstBlock.Id)
                    .Where(i => i.ActivityPath == activity.Path)
                    .OrderBy(i => i.Id) // 按Id升序排列以维持原有顺序
                    .ToListAsync(); // 将结果集转换为List

                TimeSpan timeSpan = TimeSpan.Zero;
                foreach (var block in itemsFromPivotalToEnd)
                {
                    if (block.StartTime < startTime)
                    {
                        timeSpan += block.EndTime - startTime;
                    }
                    else
                    {
                        timeSpan += block.Duration;
                    }
                }
                return timeSpan;
            }
            else
            {
                return TimeSpan.Zero;
            }
        }

        private void SetupTimer()
        {
            // 当前时间
            DateTime now = DateTime.Now;
            // 下一个整分钟的时间 - 当前时间 = 需要等待的时间
            int millisecondsUntilNextMinute = 60000 - now.Second * 1000 - now.Millisecond;
            // 创建并设置定时器
            _timer = new Timer(millisecondsUntilNextMinute);
            _timer.Elapsed += TimerInitialize;
            _timer.AutoReset = false; // 确保只触发一次，之后我们将重新设置它
            _timer.Start();
        }

        private void TimerInitialize(object? sender, ElapsedEventArgs e)
        {
            // 定时器触发的事件
            Console.WriteLine("A new minute has started!");

            // 重设定时器，以便它每分钟触发一次
            _timer.Interval = 60000; // 每分钟触发
            _timer.AutoReset = true; // 自动重置
            _timer.Elapsed -= TimerInitialize; // 移除旧的事件处理程序
            _timer.Elapsed += TimerElapsed; // 添加新的事件处理程序
            _timer.Start();
        }
        private async void TimerElapsed(object? sender, ElapsedEventArgs e)
        {
            var now = DateTime.Now;
            var currentActivityPath = _currentActivity?.Path ?? Activity.Alive.Path; // 如果为空那就只知道活着了

            var lastBlock = await _dbContext.TimeBlocks
                .OrderByDescending(d => d.Id)
                .Take(1)
                .FirstOrDefaultAsync();

            bool needNewBlock = false;
            if (lastBlock is null)
            {
                // 如果没有上一个块，则需要创建一个新块
                needNewBlock = true;
            }
            else
            {
                // 有上一个块
                if ((now - lastBlock.ModifiedTime).TotalMinutes <= 2)
                {
                    // 上一个块很近，可以更新
                    lastBlock!.EndTime = now;
                    lastBlock.ModifiedTime = now;
                    Debug.Print($"[{lastBlock.ModifiedTime:T}]更新了旧块：{lastBlock.ActivityPath}");

                    if (lastBlock.ActivityPath != currentActivityPath)
                    {
                        // 如果上一个块和当前活动不一致，说明活动切换了，需要先更新活动，再创建一个新块
                        needNewBlock = true;
                    }
                }
                else
                {
                    // 上一个块很远，说明中间没有记录
                    // 那么首先要处理上一个块，它可能有问题
                    if (lastBlock.StartTime == lastBlock.EndTime)
                    {
                        _dbContext.Remove(lastBlock);
                    }

                    // 然后创建一个新块
                    needNewBlock = true;
                }
            }

            if (needNewBlock)
            {
                var newBlock = new TimeBlock()
                {
                    StartTime = now,
                    EndTime = now,
                    ModifiedTime = now,
                    ActivityPath = currentActivityPath,
                };
                _dbContext.TimeBlocks.Add(newBlock);
                Debug.Print($"[{newBlock.ModifiedTime:T}]创建了新块：{newBlock.ActivityPath}");
            }

            await _dbContext.SaveChangesAsync();
        }

        //private void Stop()
        //{
        //    _timer?.Stop();
        //    _timer?.Dispose();
        //}
    }
}
