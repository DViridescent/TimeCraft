﻿using CommunityToolkit.Mvvm.ComponentModel;
using MahApps.Metro.IconPacks;
using Objects;
using System.Windows.Threading;
using WPF.Helpers;

namespace WPF.ViewModels
{
    public partial class BlockViewModel : ObservableObject, IDisposable
    {
        private readonly DispatcherTimer _timer;
        private DateTime _lastStartTime;
        private TimeSpan _lastDuration;

        public IActivity Activity { get; }

        public BlockViewModel(IActivity activity, TimeSpan startTime)
        {
            Activity = activity;
            _name = activity.Name;
            _kind = IconHelper.GetIconKind(activity);
            _timer = new()
            {
                Interval = TimeSpan.FromSeconds(1),
            };
            _timer.Tick += Tick;

            _duration = startTime;
            _lastDuration = startTime;
        }

        public void Start()
        {
            _lastStartTime = DateTime.Now;
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
            _lastDuration = Duration;
        }

        [ObservableProperty]
        private TimeSpan _duration = TimeSpan.Zero;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private PackIconLucideKind _kind;

        private void Tick(object? sender, EventArgs e)
        {
            Duration = DateTime.Now - _lastStartTime + _lastDuration;
        }

        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _timer.Tick -= Tick;
                    _timer.Stop();
                }

                disposedValue = true;
            }
        }
        void IDisposable.Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
