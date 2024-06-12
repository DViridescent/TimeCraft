using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace CustomControls;
public class AnimatedBorder : Border
{
    public AnimatedBorder()
    {
        LayoutTransform = new ScaleTransform(1, 1);
        Loaded += AnimatedBorder_Loaded;
    }

    private void AnimatedBorder_Loaded(object sender, RoutedEventArgs e)
    {
        SizeChanged += AnimatedBorder_SizeChanged;
    }

    private void AnimatedBorder_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.WidthChanged)
        {
            UpdateWidthSizeChangeAnimation(e.PreviousSize.Width, e.NewSize.Width);
        }
        if (e.HeightChanged)
        {
            UpdateHeightSizeChangeAnimation(e.PreviousSize.Height, e.NewSize.Height);
        }
    }

    private static DoubleAnimation ScaleAnimation => new()
    {
        To = 1,
        Duration = TimeSpan.FromSeconds(0.5),
        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut },
        FillBehavior = FillBehavior.Stop
    };

    private void UpdateWidthSizeChangeAnimation(double previousWidth, double newWidth)
    {
        Debug.Print($"previousWidth: {previousWidth}, newWidth: {newWidth}");
        var scaleTransform = ((ScaleTransform)LayoutTransform);
        // 缩放比例，使新的宽度缩放后等于老的宽度
        scaleTransform.ScaleX = previousWidth / newWidth;
        var animation = ScaleAnimation;
        animation.Completed += (sender, e) =>
        {
            scaleTransform.ScaleX = 1;
        };
        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animation, HandoffBehavior.SnapshotAndReplace);
    }

    private void UpdateHeightSizeChangeAnimation(double previousHeight, double newHeight)
    {
        var scaleTransform = ((ScaleTransform)LayoutTransform);
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
