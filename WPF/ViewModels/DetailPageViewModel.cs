using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF.ViewModels;
internal partial class DetailPageViewModel : ViewModelBase
{
    public DetailPageViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [RelayCommand]
    private void Back()
    {
        Navigater.Navigate<HomePageViewModel>();
    }
}
