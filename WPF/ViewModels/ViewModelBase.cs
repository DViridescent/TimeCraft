using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using WPF.Services;

namespace WPF.ViewModels;
internal abstract class ViewModelBase : ObservableObject
{
    protected readonly IServiceProvider _serviceProvider;
    protected Navigater Navigater => _serviceProvider.GetRequiredService<Navigater>();

    /// <summary>
    /// 基类构造函数，注入<see cref="IServiceProvider"/>以后提供服务
    /// </summary>
    /// <param name="serviceProvider"></param>
    protected ViewModelBase(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
}
