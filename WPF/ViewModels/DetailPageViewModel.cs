using CommunityToolkit.Mvvm.Input;

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
