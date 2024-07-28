using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C971.Class
{
    [SQLite.Table("Term")]
    public class Term
    {
        [PrimaryKey,  AutoIncrement]
        public int Id {  get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate {  get; set; }
        public DateTime EndDate { get; set; }
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
