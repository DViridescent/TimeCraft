using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF
{
    internal partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private Block? _currentBlock;

        public MainWindowViewModel()
        {
            Blocks =
            [
                new Block("写代码", PackIconLucideKind.Code),
                new Block("开会", PackIconLucideKind.MessageSquareWarning),
                new Block("审查", PackIconLucideKind.GitPullRequestArrow),
                new Block("帮忙", PackIconLucideKind.BugOff),
                new Block("吃饭", PackIconLucideKind.GitPullRequestArrow),
            ];
        }

        public ObservableCollection<Block> Blocks { get; private set; }

        [RelayCommand]
        public void Swap(Block targetBlock)
        {
            CurrentBlock = targetBlock;
        }
    }

    public partial class Block(string name, PackIconLucideKind kind) : ObservableObject
    {
        [ObservableProperty]
        private string _name = name;

        [ObservableProperty]
        private PackIconLucideKind _kind = kind;
    }
}
