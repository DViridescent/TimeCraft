using MahApps.Metro.IconPacks;
using Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF.Helpers;
internal static class IconHelper
{
    public static PackIconLucideKind GetIconKind(IActivity activity)
    {
        if (activity == Activity.Coding) return PackIconLucideKind.Code;
        else if (activity == Activity.CodeReview) return PackIconLucideKind.GitPullRequestArrow;
        else if (activity == Activity.Assistance) return PackIconLucideKind.BugOff;
        else if (activity == Activity.Meeting) return PackIconLucideKind.MessageSquareWarning;
        else if (activity == Activity.Eating) return PackIconLucideKind.Soup;
        else if (activity == Activity.Slacking) return PackIconLucideKind.Fish;
        else return PackIconLucideKind.Ban;
    }
}
