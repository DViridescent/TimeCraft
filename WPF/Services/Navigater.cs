using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using WPF.Interfaces;

namespace WPF.Services;

/// <summary>
/// 负责页面切换，需要依赖注入传入内容容器<see cref="IContentHolder"/>
/// </summary>
public class Navigater
{
    private readonly IServiceProvider _rootServiceProvider;
    private ViewProvider ViewProvider => _rootServiceProvider.GetRequiredService<ViewProvider>();

    public Navigater(IServiceProvider serviceProvider)
    {
        _rootServiceProvider = serviceProvider;
    }

    /// <summary>
    /// 切换到某个页面
    /// </summary>
    /// <remarks>
    /// 如果调用此方法时没有传入参数，那么会通过<see cref="IServiceProvider"/>获取对应的ViewModel，否则会使用参数构造一个新的ViewModel
    /// <para>此时该ViewModel的生命周期类似Transient</para>
    /// </remarks>
    /// <typeparam name="TViewModel">要切换的ViewModel的类型，需要有对应的View</typeparam>
    /// <param name="parameters">这个ViewModel构造函数中需要的（未注册的）额外参数</param>
    public void Navigate<TViewModel>(params object[] parameters) where TViewModel : ObservableObject => Navigate<TViewModel>(null, parameters);
    /// <summary>
    /// 针对多内容容器的时候的Navigate方法
    /// </summary>
    /// <remarks>
    /// 传入注册内容容器时的Key（<see cref="ServiceCollectionServiceExtensions.AddKeyedSingleton{TService}(IServiceCollection, object?)"/>）来指定要切换的内容容器
    /// </remarks>
    /// <typeparam name="TViewModel">要切换的ViewModel的类型，需要有对应的View</typeparam>
    /// <param name="contentHolderKey">内容容器的Key</param>
    /// <param name="parameters">这个ViewModel构造函数中需要的（未注册的）额外参数</param>
    public void NavigateContentHolder<TViewModel>(object contentHolderKey, params object[] parameters) where TViewModel : ObservableObject
        => Navigate<TViewModel>(contentHolderKey, parameters);

    /// <summary>
    /// 切换到某个页面，直接使用传入的ViewModel对象
    /// </summary>
    /// <typeparam name="TViewModel">要切换的ViewModel的类型，需要有对应的View</typeparam>
    /// <param name="viewModel">需要使用的ViewModel对象</param>
    public void Navigate<TViewModel>(TViewModel viewModel) where TViewModel : ObservableObject => Navigate(null, viewModel);
    /// <summary>
    /// 针对多内容容器的时候的Navigate方法，直接使用传入的ViewModel对象
    /// </summary>
    /// <typeparam name="TViewModel">要切换的ViewModel的类型，需要有对应的View</typeparam>
    /// <param name="contentHolderKey">内容容器的Key</param>
    /// <param name="viewModel">需要使用的ViewModel对象</param>
    public void NavigateContentHolder<TViewModel>(object contentHolderKey, TViewModel viewModel) where TViewModel : ObservableObject
        => Navigate(contentHolderKey, viewModel);

    private IContentHolder GetContentHolder(object? key) => key == null
            ? _rootServiceProvider.GetRequiredService<IContentHolder>()
            : _rootServiceProvider.GetRequiredKeyedService<IContentHolder>(key);

    private void Navigate<TViewModel>(object? contentHolderKey, params object[] parameters) where TViewModel : ObservableObject
    {
        // 如果没有参数，那么通过_serviceProvider获取ViewModel，否则new一个新的
        var viewModel = parameters.Length == 0
            ? _rootServiceProvider.GetRequiredService<TViewModel>()
            : ActivatorUtilities.CreateInstance<TViewModel>(_rootServiceProvider, parameters);

        Navigate(contentHolderKey, viewModel);
    }
    private void Navigate<TViewModel>(object? contentHolderKey, TViewModel viewModel) where TViewModel : ObservableObject
    {
        var view = ViewProvider.GetViewByViewModel<TViewModel>();
        view.DataContext = viewModel;

        GetContentHolder(contentHolderKey).ChangeableContent = view;
    }
}
