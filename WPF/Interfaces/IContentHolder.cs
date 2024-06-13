using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPF.Interfaces;

/// <summary>
/// View层实现的内容容器，一般通过对<see cref="ContentControl"/>赋值实现
/// </summary>
public interface IContentHolder
{
    /// <summary>
    /// 用于承载可变内容，通过赋值改变内容
    /// </summary>
    FrameworkElement ChangeableContent { set; }
}
