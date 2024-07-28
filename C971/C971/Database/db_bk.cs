using C971.Class;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C971.Database
{
    public static class DatabaseHandler
    {
        private static SQLiteAsyncConnection _db;
        private const string DBNAME = "term.db";

        public static async Task Init()
        {
            _db = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DBNAME));
            if (_db == null)
            {
                System.Environment.Exit(1);
            }

            await _db.CreateTableAsync<Term>();
            await _db.CreateTableAsync<Course>();
            await _db.CreateTableAsync<Assessment>();
            var term1 = new Term
            {
                Title = "Spring Term",
                StartDate = new DateTime(2022, 01, 04),
                EndDate = new DateTime(2022, 05, 01)
            };
            var term2 = new Term
            {
                Title = "Autumn Term",
                StartDate = new DateTime(2022, 09, 04),
                EndDate = new DateTime(2022, 12, 16)
            };

            await CreateTerm(term1);
            await CreateTerm(term2);
        }

        public static async Task<List<Term>> GetTerms()
        {
            await Init();

            return await _db.Table<Term>().ToListAsync();
        }

        public static async Task<Term> GetTerm(int id)
        {
            await Init();

            var query = _db.Table<Term>().Where(t => t.Id.Equals(id));
            return await query.FirstAsync();
        }

        public static async Task CreateTerm(Term term)
        {
            await Init();
            await _db.InsertAsync(term);

        }

        public static async Task UpdateTerm(Term term)
        {
            await Init();
            await _db.UpdateAsync(term);
        }

        public static async Task DeleteTerm(Term term)
        {
            await Init();
            await _db.DeleteAsync(term);
        }

        /// Courses

        public static async Task<List<Course>?> GetCoursesByTerm(int termId)
        {
            await Init();
            return await _db.Table<Course>().Where(x => x.TermId == termId).ToListAsync();
        }

        public static async Task CreateCourse(Course course)
        {
            await Init();
            await _db.InsertAsync(course);
        }

        public static async Task UpdateCourse(Course course)
        {
            await Init();
            await _db.UpdateAsync(course);
        }

        public static async Task DeleteCourse(Course course)
        {
            await Init();
            await _db.DeleteAsync(course);
        }

        /// Assessment

        public static async Task CreateAssessment(Assessment assessment)
        {
            await Init();
            await _db.InsertAsync(assessment);
        }

        public static async Task UpdateAssessment(Assessment assessment)
        {
            await Init();
            await _db.UpdateAsync(assessment);
        }

        public static async Task DeleteAssessment(Assessment assessment)
        {
            await Init();
            await _db.DeleteAsync(assessment);
        }

        public static async Task<List<Assessment>?> GetAssessmentsByCourse(int courseId)
        {
            await Init();
            return await _db.Table<Assessment>().Where(x => x.CourseId == courseId).ToListAsync();
        }
    }
}
