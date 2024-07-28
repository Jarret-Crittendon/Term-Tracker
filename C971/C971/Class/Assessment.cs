using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C971.Class
{
    [SQLite.Table("Assessment")]
    public class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime StartDate {  get; set; }
        public DateTime EndDate { get; set; }
        public int CourseId { get; set; }
        public bool NotifyStart { get; set; }
        public bool NotifyEnd { get; set; }
        public string DetailDate
        {
            get
            {
                var start = DateOnly.FromDateTime(StartDate);
                var end = DateOnly.FromDateTime(EndDate);
                return start + " - " + end;
            }
        }

    }
}
