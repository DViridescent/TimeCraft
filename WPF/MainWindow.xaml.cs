using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using WPF.Interfaces;
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

            SizeChanged += Window_SizeChanged;

            // 从最小化恢复时，显示前面板
            //this.StateChanged += (s, e) =>
            //{
            //    if (WindowState == WindowState.Normal)
            //    {
            //        ToFrontPanel(this, new RoutedEventArgs());
            //    }
            //};
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

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // 使用WindowInteropHelper来获取当前窗口的句柄
            var windowInteropHelper = new WindowInteropHelper(this);
            var currentScreen = Screen.FromHandle(windowInteropHelper.Handle);

            // BUG 还是有问题，好几个。

            // 获取当前屏幕的工作区域
            var workingArea = currentScreen.WorkingArea;

            // 如果窗口的右侧超出了屏幕的右侧
            if (Left + ActualWidth > workingArea.Right)
            {
                Left = workingArea.Right - ActualWidth; // 向左移动窗口
            }

            // 如果窗口的底部超出了屏幕的底部
            if (Top + ActualHeight > workingArea.Bottom)
            {
                Top = workingArea.Bottom - ActualHeight; // 向上移动窗口
            }
        }

        public void Minimize()
        {
            WindowState = WindowState.Minimized;
        }
    }
}