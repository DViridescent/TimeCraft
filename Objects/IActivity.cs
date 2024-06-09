using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects;

public interface IActivity
{
    string Name { get; }
    string Description { get; }
    string Path { get; }
}

