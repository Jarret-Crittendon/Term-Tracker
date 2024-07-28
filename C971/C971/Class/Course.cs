using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C971.Class
{
    [SQLite.Table("Course")]
    public class Course
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int TermId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public string InstructorPhone { get; set; } = string.Empty;
        public string InstructorEmail { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public bool NotifyStart { get; set; }
        public bool NotifyEnd { get; set; }
        public int OAId { get; set; }
        public int PAId { get; set; } 
        public string Status { get; set; } = string.Empty;
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
