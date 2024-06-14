using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CustomControls;
public static class Animation
{
    /// <summary>
    /// 尺寸变化时播放动画
    /// </summary>
    public static readonly DependencyProperty SizeChangedProperty = DependencyProperty.RegisterAttached(
        "SizeChanged",
        typeof(bool),
        typeof(Animation),
        new PropertyMetadata(false, OnSizeChangedChanged));

    /// <summary>
    /// 祖先窗口贴靠屏幕边缘，动画同步
    /// </summary>
    public static readonly DependencyProperty AncestorWindowAttachedProperty = DependencyProperty.RegisterAttached(
        "AncestorWindowAttached",
        typeof(bool),
        typeof(Animation),
        new PropertyMetadata(false, OnAncestorWindowAttachedChanged));

    public static void SetSizeChanged(DependencyObject element, bool value) => element.SetValue(SizeChangedProperty, value);
    public static bool GetSizeChanged(DependencyObject element) => (bool)element.GetValue(SizeChangedProperty);
    public static void SetAncestorWindowAttached(DependencyObject element, bool value) => element.SetValue(AncestorWindowAttachedProperty, value);
    public static bool GetAncestorWindowAttached(DependencyObject element) => (bool)element.GetValue(AncestorWindowAttachedProperty);

    private static void OnSizeChangedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FrameworkElement element)
            return;

        if ((bool)e.NewValue)
        {
            var handler = new EventHandler((sender, e) => Element_LayoutUpdatedOnce(element));
            _handlerDict[element] = handler;
            element.LayoutTransform = new ScaleTransform(1, 1);
            element.LayoutUpdated += handler;
        }
        else
        {
            if (_handlerDict.TryGetValue(element, out var handler))
            {
                element.LayoutUpdated -= handler;
                _handlerDict.Remove(element);

                element.SizeChanged -= Element_SizeChanged;
            }
        }
    }
    private static void OnAncestorWindowAttachedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FrameworkElement element)
            return;

        if ((bool)e.NewValue)
        {
            // 需要找到d祖先的窗口
            _elementHasAttachedWindow.Add(element);
        }
        else
        {
            _elementHasAttachedWindow.Remove(element);
        }
    }

    private static readonly Dictionary<FrameworkElement, EventHandler> _handlerDict = [];
    private static readonly HashSet<FrameworkElement> _elementHasAttachedWindow = [];
    private static void Element_LayoutUpdatedOnce(FrameworkElement element)
    {
        // 在首次布局更新后移除LayoutUpdated事件处理器，以避免重复执行此逻辑
        if (_handlerDict.TryGetValue(element, out var handler))
        {
            element.LayoutUpdated -= handler;
            _handlerDict.Remove(element);
        }
        // 确保控件的尺寸不是0，然后注册SizeChanged事件
        element.SizeChanged += Element_SizeChanged;
    }

    private static void Element_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is not FrameworkElement element)
            return;

        if (e.WidthChanged && e.NewSize.Width != 0)
        {
            UpdateWidthSizeChangeAnimation(element, e.PreviousSize.Width, e.NewSize.Width);
        }
        if (e.HeightChanged && e.NewSize.Height != 0)
        {
            UpdateHeightSizeChangeAnimation(element, e.PreviousSize.Height, e.NewSize.Height);
        }
    }

    private static DoubleAnimation DoubleAnimation => new()
    {
        Duration = TimeSpan.FromSeconds(0.5),
        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut },
        FillBehavior = FillBehavior.Stop,
    };

    private static void UpdateWidthSizeChangeAnimation(FrameworkElement frameworkElement, double previousWidth, double newWidth)
    {
        var scaleTransform = ((ScaleTransform)frameworkElement.LayoutTransform);
        // 缩放比例，使新的宽度缩放后等于老的宽度
        scaleTransform.ScaleX = previousWidth / newWidth;
        var animation = DoubleAnimation;
        animation.To = 1;
        animation.Completed += (sender, e) =>
        {
            scaleTransform.ScaleX = 1;
        };

        // HACK: 神秘的解决方案 https://stackoverflow.com/questions/2131797/applying-animated-scaletransform-in-code-problem
        Storyboard.SetTargetProperty(animation, new PropertyPath("LayoutTransform.ScaleX"));
        Storyboard.SetTarget(animation, frameworkElement); // 设置动画的目标是 scaleTransform
        var storyboard = new Storyboard(); // 创建动画板
        storyboard.Children.Add(animation); // 把动画添加到动画板

        if (_elementHasAttachedWindow.Contains(frameworkElement))
        {
            var window = Window.GetWindow(frameworkElement);
            AddWindowSynchronizedHorizontalAnimation(window, newWidth - previousWidth, storyboard);
            storyboard.Begin();
        }
        else
        {
            storyboard.Begin();
        }
    }
    private static void UpdateHeightSizeChangeAnimation(FrameworkElement frameworkElement, double previousHeight, double newHeight)
    {
        var scaleTransform = ((ScaleTransform)frameworkElement.LayoutTransform);
        // 缩放比例，使新的高度缩放后等于老的高度
        scaleTransform.ScaleY = previousHeight / newHeight;
        var animation = DoubleAnimation;
        animation.To = 1;
        animation.Completed += (sender, e) =>
        {
            scaleTransform.ScaleY = 1;
        };

        Storyboard.SetTargetProperty(animation, new PropertyPath("LayoutTransform.ScaleY"));
        Storyboard.SetTarget(animation, frameworkElement); // 设置动画的目标是 scaleTransform
        var storyboard = new Storyboard(); // 创建动画板
        storyboard.Children.Add(animation); // 把动画添加到动画板

        if (_elementHasAttachedWindow.Contains(frameworkElement))
        {
            var window = Window.GetWindow(frameworkElement);
            AddWindowSynchronizedVerticalAnimation(window, newHeight - previousHeight, storyboard);
            storyboard.Begin();
        }
        else
        {
            storyboard.Begin();
        }
    }

    /// <summary>
    /// 给父窗口播放一段同步动画，保证继续贴右边
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private static void AddWindowSynchronizedHorizontalAnimation(Window window, double deltaWidth, Storyboard storyboard)
    {
        // 找到窗口所在的屏幕
        var screen = Screen.FromHandle(new WindowInteropHelper(window).Handle);

        // 找到窗口的工作区
        var workArea = screen.WorkingArea;

        // 判断窗口是否贴右边
        if (Math.Abs(window.Left + window.Width - workArea.Right) >= 1e-6)
            return;

        // 计算窗口的新位置
        var newLeft = window.Left - deltaWidth;

        // 创建动画
        var animation = DoubleAnimation;
        animation.From = window.Left;
        animation.To = newLeft;
        animation.Completed += (sender, e) =>
        {
            // 动画完成后，设置窗口的位置
            window.Left = newLeft;
        };

        Storyboard.SetTarget(animation, window);
        Storyboard.SetTargetProperty(animation, new PropertyPath(Window.LeftProperty));
        storyboard.Children.Add(animation);
    }
    /// <summary>
    /// 给父窗口播放一段同步动画，保证继续贴下边
    /// </summary>
    private static void AddWindowSynchronizedVerticalAnimation(Window window, double deltaHeight, Storyboard storyboard)
    {
        // 找到窗口所在的屏幕
        var screen = Screen.FromHandle(new WindowInteropHelper(window).Handle);

        // 找到窗口的工作区
        var workArea = screen.WorkingArea;

        // 判断窗口是否贴下边
        if (Math.Abs(window.Top + window.Height - workArea.Bottom) >= 1e-6)
            return;

        // 计算窗口的新位置
        var newTop = window.Top - deltaHeight;

        // 创建动画
        var animation = DoubleAnimation;
        animation.From = window.Top;
        animation.To = newTop;
        animation.Completed += (sender, e) =>
        {
            // 动画完成后，设置窗口的位置
            window.Top = newTop;
        };

        Storyboard.SetTarget(animation, window);
        Storyboard.SetTargetProperty(animation, new PropertyPath(Window.TopProperty));
        storyboard.Children.Add(animation);
    }
}
