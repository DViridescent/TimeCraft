using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomControls;
public class SelectableItemsControl : Selector
{
    static SelectableItemsControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectableItemsControl), new FrameworkPropertyMetadata(typeof(SelectableItemsControl)));
    }
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is ContentPresenter;
    }
    protected override DependencyObject GetContainerForItemOverride()
    {
        var container = new ListBoxItem();
        container.Style = FindResource("ListBoxItemStyle") as Style;
        return container;
    }
}