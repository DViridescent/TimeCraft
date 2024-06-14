using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace WPF.Views;
/// <summary>
/// HomePage.xaml 的交互逻辑
/// </summary>
public partial class HomePage : UserControl
{
    public HomePage()
    {
        InitializeComponent();

        // 这样可以让代码一定在Loaded完全结束后执行
        Dispatcher.BeginInvoke(new Action(() =>
        {
            ((TranslateTransform)BackPanel.RenderTransform).Y = MainGrid.ActualHeight;
        }), DispatcherPriority.Loaded);
    }

    private void ToBackPanel(object sender, RoutedEventArgs e)
    {
        FrontPanelLeaveAnimation.To = -MainGrid.ActualHeight;

        VisualStateManager.GoToElementState(MainGrid, "ShowBackPanel", true);
    }

    private void ToFrontPanel(object sender, RoutedEventArgs e)
    {
        BackPanelLeaveAnimation.To = MainGrid.ActualHeight;

        VisualStateManager.GoToElementState(MainGrid, "ShowFrontPanel", true);
    }
}
