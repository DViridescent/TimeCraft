using System.ComponentModel.DataAnnotations.Schema;

namespace Recorder.Database
{
    public class TimeBlock
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifiedTime { get; set; }
        public string ActivityPath { get; set; }

        [NotMapped]
        public TimeSpan Duration => EndTime - StartTime;
    }
}
