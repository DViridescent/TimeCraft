using Microsoft.Extensions.DependencyInjection;
using Recorder;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using WPF.Interfaces;
using WPF.Services;
using WPF.ViewModels;
using WPF.Views;
using Application = System.Windows.Application;

namespace WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; }

    private NotifyIcon notifyIcon = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);

        ServiceProvider = serviceCollection.BuildServiceProvider();

        // 从资源加载图标
        using (Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/WPF;component/Assets/favicon.ico")).Stream)
        {
            notifyIcon = new NotifyIcon
            {
                Icon = new Icon(iconStream),
                Visible = false // 初始时不可见
            };
        }
        notifyIcon.DoubleClick += (sender, args) =>
        {
            MainWindow.Show();
            MainWindow.WindowState = WindowState.Normal;
            notifyIcon.Visible = false;
        };
        MainWindow = ServiceProvider.GetRequiredService<MainWindow>();

        // MainWindow.DataContext = ServiceProvider.GetRequiredService<MainWindowViewModel>();
        ServiceProvider.GetRequiredService<Navigater>().Navigate<HomePageViewModel>();
        MainWindow.Show();
        // 处理主窗口最小化逻辑
        MainWindow.StateChanged += MainWindow_StateChanged;
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // 使用扩展方法注册你的服务
        services.AddRecorderService()
            .AddSingleton<ViewProvider>()
            .AddSingleton<Navigater>()
            .AddSingleton<MainWindow>()
            .AddSingleton<IContentHolder>(sp => sp.GetRequiredService<MainWindow>())

            .AddSingleton<HomePageViewModel>()
            .AddSingleton<DetailPageViewModel>()
            .AddSingleton<HomePage>()
            .AddSingleton<DetailPage>();
        ;
    }

    private void MainWindow_StateChanged(object? sender, EventArgs e)
    {
        if (MainWindow.WindowState == WindowState.Minimized)
        {
            MainWindow.Hide();
            notifyIcon.Visible = true;
        }
    }
}
