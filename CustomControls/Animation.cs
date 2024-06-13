using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Windows.Controls;

namespace CustomControls;
public static class Animation
{
    public static readonly DependencyProperty SizeChangedProperty = DependencyProperty.RegisterAttached(
        "SizeChanged",
        typeof(bool),
        typeof(Animation),
        new PropertyMetadata(false, OnSizeChangedChanged));

    public static void SetSizeChanged(DependencyObject element, bool value) => element.SetValue(SizeChangedProperty, value);
    public static bool GetSizeChanged(DependencyObject element) => (bool)element.GetValue(SizeChangedProperty);

    private static void OnSizeChangedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FrameworkElement element)
            return;

        if ((bool)e.NewValue)
        {
            element.LayoutTransform = new ScaleTransform(1, 1);
            element.LayoutUpdated += Element_LayoutUpdatedOnce;
        }
        else
        {
            element.LayoutUpdated -= Element_LayoutUpdatedOnce;
            element.SizeChanged -= Element_SizeChanged;
        }
    }

    private static void Element_LayoutUpdatedOnce(object? sender, EventArgs e)
    {
        // BUG
        if (sender is not FrameworkElement element)
            return;

        // 在首次布局更新后移除LayoutUpdated事件处理器，以避免重复执行此逻辑
        element.LayoutUpdated -= Element_LayoutUpdatedOnce;
        // 确保控件的尺寸不是0，然后注册SizeChanged事件
        element.SizeChanged += Element_SizeChanged;
    }

    private static void Element_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement element)
        {
            element.SizeChanged += Element_SizeChanged;
        }
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

    private static DoubleAnimation ScaleAnimation => new()
    {
        To = 1,
        Duration = TimeSpan.FromSeconds(0.5),
        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut },
        FillBehavior = FillBehavior.Stop
    };

    private static void UpdateWidthSizeChangeAnimation(FrameworkElement frameworkElement, double previousWidth, double newWidth)
    {
        Debug.Print($"previousWidth: {previousWidth}, newWidth: {newWidth}");
        var scaleTransform = ((ScaleTransform)frameworkElement.LayoutTransform);
        // 缩放比例，使新的宽度缩放后等于老的宽度
        scaleTransform.ScaleX = previousWidth / newWidth;
        var animation = ScaleAnimation;
        animation.Completed += (sender, e) =>
        {
            scaleTransform.ScaleX = 1;
        };
        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animation, HandoffBehavior.SnapshotAndReplace);
    }

    private static void UpdateHeightSizeChangeAnimation(FrameworkElement frameworkElement, double previousHeight, double newHeight)
    {
        var scaleTransform = ((ScaleTransform)frameworkElement.LayoutTransform);
        // 缩放比例，使新的高度缩放后等于老的高度
        scaleTransform.ScaleY = previousHeight / newHeight;
        var animation = ScaleAnimation;
        animation.Completed += (sender, e) =>
        {
            scaleTransform.ScaleY = 1;
        };
        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animation, HandoffBehavior.SnapshotAndReplace);
    }
}
