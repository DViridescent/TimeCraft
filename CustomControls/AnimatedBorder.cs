using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CustomControls;
internal class AnimatedBorder : Border
{
    static AnimatedBorder()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedBorder), new FrameworkPropertyMetadata(typeof(AnimatedBorder)));
    }
}
