using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recorder.Database
{
    internal class DataContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<TimeBlock> TimeBlocks { get; set; }
    }
}
