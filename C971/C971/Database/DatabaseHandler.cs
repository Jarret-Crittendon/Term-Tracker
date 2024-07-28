using C971.Class;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C971.Database
{
    public class DatabaseHandler
    {
        private SQLiteAsyncConnection _db;
        private const string DBNAME = "term.db";

        public DatabaseHandler()
        {
            _db = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DBNAME));
            if (_db == null)
            {
                System.Environment.Exit(1);
            }
            
            //DeleteTables();

            CreateTables();
            
            EvalEntry();

            Init();    
        }

        private async void DeleteTables()
        {
            await _db.DeleteAllAsync<Term>();
            await _db.DeleteAllAsync<Course>();
            await _db.DeleteAllAsync<Assessment>();
        }

        private async void CreateTables()
        {
            await _db.CreateTableAsync<Term>();
            await _db.CreateTableAsync<Course>();
            await _db.CreateTableAsync<Assessment>();
        }

        public void Init()
        {

        }

        public async Task<List<Term>> GetTerms()
        {  
            return await _db.Table<Term>().ToListAsync();  
        }

        public async Task<Term> GetTerm(int id)
        {
            var query = _db.Table<Term>().Where(t => t.Id.Equals(id));
            return await query.FirstAsync();          
        }

        public async Task CreateTerm(Term term)
        {
            await _db.InsertAsync(term);          
        }

        public async Task UpdateTerm(Term term)
        {
            await _db.UpdateAsync(term);     
        }
         
        public async Task DeleteTerm(Term term)
        {;
            await _db.DeleteAsync(term);            
        }

        /// Courses

        public async Task<List<Course>> GetCoursesByTerm(int termId)
        {
            return await _db.Table<Course>().Where(x => x.TermId == termId).ToListAsync();
        }

        public async Task<Course> GetCourse(int courseId)
        {
            var query = _db.Table<Course>().Where(t =>  t.Id.Equals(courseId));
            return await query.FirstAsync();
        }

        public async Task CreateCourse(Course course)
        {
            await _db.InsertAsync(course);
        }

        public async Task UpdateCourse(Course course)
        {
            await _db.UpdateAsync(course);
        }

        public async Task DeleteCourse(Course course)
        {;
            await _db.DeleteAsync(course);
        }

        /// Assessment

        public async Task CreateAssessment(Assessment assessment)
        {
            await _db.InsertAsync(assessment);
        }

        public async Task UpdateAssessment(Assessment assessment)
        {
            await _db.UpdateAsync(assessment);
        }

        public async Task DeleteAssessment(Assessment assessment)
        {
            await _db.DeleteAsync(assessment);
        }

        public async Task<Assessment> GetAssessment(int id)
        {
            var query = _db.Table<Assessment>().Where(t => t.Id.Equals(id));
            return await query.FirstAsync();
        }

        public async Task<List<Assessment>> GetAssessmentsByCourse(int courseId)
        {
            return await _db.Table<Assessment>().Where(x => x.CourseId == courseId).ToListAsync();
        }

        private async void EvalEntry()
        {
            var term = new Term {
                Title = "Test Term",
                StartDate = new DateTime(2024, 9, 06),
                EndDate = new DateTime(2024, 12, 16)
            };
            
            
            await _db.InsertAsync(term);
            var queryTerm = GetTerms();
            var terms = await queryTerm;

            var _termId = terms[0].Id;

            var course = new Course
            {
                Name = "Baking",
                TermId = _termId,
                StartDate = new DateTime(2024, 9, 10),
                EndDate = new DateTime(2024, 12, 09),
                InstructorName = "Anika Patel",
                InstructorPhone = "555-123-4567",
                InstructorEmail = "anika.patel@strimeuniversity.edu",
                Notes = "Includes bread making.",
                NotifyStart = false,
                NotifyEnd = false,
                Status = "In Progress"
            };

            //var _courseId = course.Id;
            await _db.InsertAsync(course);
            var queryCourse = GetCoursesByTerm(_termId);
            var courses = await queryCourse;
            var _courseId = courses[0].Id;

            var oa1 = new Assessment
            {
                Name = "Objective",
                CourseId = _courseId,
                Type = "Objective",
                StartDate = new DateTime(2024, 10, 19),
                EndDate = new DateTime(2024, 10, 20),
                NotifyStart = false,
                NotifyEnd = false
            };

            var pa1 = new Assessment
            {
                Name = "Performance",
                CourseId = _courseId,
                Type = "Performance",
                StartDate = new DateTime(2024, 12, 06),
                EndDate = new DateTime(2024, 12, 08),
                NotifyStart = true,
                NotifyEnd = false
            };

            await _db.InsertAsync(oa1);
            await _db.InsertAsync(pa1);
        }
    }
}
