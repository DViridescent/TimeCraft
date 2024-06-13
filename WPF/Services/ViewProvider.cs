using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPF.Services;

public class ViewProvider(IServiceProvider serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public FrameworkElement GetViewByViewModel(Type viewModelType)
    {
        // 创建新的的View实例

        // 这里假设View和ViewModel具有相同的名称规则，只是命名空间不同
        string viewTypeName = viewModelType.FullName?.Replace("ViewModels", "Views").Replace("ViewModel", "") ?? string.Empty;

        // 获取当前 ViewModel 类型所在的程序集
        var assembly = viewModelType.Assembly;

        // 在这个程序集中查找 View 类型
        var viewType = assembly.GetType(viewTypeName);

        if (viewType != null)
        {
            // 使用依赖注入创建 View 实例
            var viewInstance = _serviceProvider.GetRequiredService(viewType);

            if (viewInstance is FrameworkElement view)
            {
                return view;
            }
            else throw new Exception($"视图对象不是FrameworkElement，而是{viewInstance.GetType().FullName}");
        }
        else throw new Exception($"未找到视图对象[{viewTypeName}]");
    }

    public FrameworkElement GetViewByViewModel<TViewModel>() => GetViewByViewModel(typeof(TViewModel));
}
