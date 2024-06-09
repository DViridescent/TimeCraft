using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomControls;
public interface IBlockViewModel
{
    public string Name { get; }
    public TimeSpan Duration { get; }
}
