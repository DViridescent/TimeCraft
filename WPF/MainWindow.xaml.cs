using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using WPF.Interfaces;
using WPF.Views;
using Point = System.Windows.Point;
using Window = System.Windows.Window;

namespace WPF
{
    public partial class MainWindow : Window, ICloseable, IContentHolder
    {
        private const int SnapDistance = 40; // 吸附距离
        private Point initialMousePosition;
        private Point initialWindowPosition;
        private bool isDragging = false;

        public MainWindow()
        {
            InitializeComponent();

            // 移动窗口
            this.MouseLeftButtonDown += (s, e) =>
            {
                initialMousePosition = this.PointToScreen(e.GetPosition(this));
                initialWindowPosition = new Point(this.Left, this.Top);
                isDragging = true;
                this.CaptureMouse(); // 开始捕获鼠标
            };

            MouseMove += MouseMoveCallback;

            this.MouseLeftButtonUp += (s, e) =>
            {
                isDragging = false;
                this.ReleaseMouseCapture(); // 停止捕获鼠标
            };

            // 从最小化恢复时，显示前面板
            // TODO: 优化
            this.StateChanged += (s, e) =>
            {
                if (WindowState == WindowState.Normal)
                {
                    var homePage = App.ServiceProvider.GetRequiredService<HomePage>();
                    homePage.ToFrontPanel(this, new RoutedEventArgs());
                }
            };
        }

        public FrameworkElement ChangeableContent
        {
            set => _mainControl.Content = value;
        }

        private void MouseMoveCallback(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!isDragging) return;

            var currentMousePosition = this.PointToScreen(e.GetPosition(this));

            // Rect workArea = SystemParameters.WorkArea;

            var deltaX = currentMousePosition.X - initialMousePosition.X;
            var deltaY = currentMousePosition.Y - initialMousePosition.Y;

            var windowX = initialWindowPosition.X + deltaX;
            var windowY = initialWindowPosition.Y + deltaY;

            // 使用鼠标来确定屏幕
            Screen screen = Screen.FromPoint(new System.Drawing.Point((int)currentMousePosition.X, (int)currentMousePosition.Y));
            Rect workArea = new(screen.WorkingArea.X, screen.WorkingArea.Y, screen.WorkingArea.Width, screen.WorkingArea.Height);


            if (windowX < workArea.Left + SnapDistance)
            {
                windowX = workArea.Left;
            }
            else if (windowX + ActualWidth > workArea.Right - SnapDistance)
            {
                windowX = workArea.Right - ActualWidth;
            }
            if (windowY < workArea.Top + SnapDistance)
            {
                windowY = workArea.Top;
            }
            else if (windowY + ActualHeight > workArea.Bottom - SnapDistance)
            {
                windowY = workArea.Bottom - ActualHeight;
            }

            this.Left = windowX;
            this.Top = windowY;
        }

        public void Minimize()
        {
            WindowState = WindowState.Minimized;
        }
    }
}