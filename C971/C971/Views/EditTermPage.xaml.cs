using C971.Class;
using C971.Database;
namespace C971.Views;

[QueryProperty(nameof(TermId), "Id")]
public partial class EditTermPage : ContentPage
{
	
	public EditTermPage(DatabaseHandler dbhandler)
	{
		InitializeComponent();
		_dbhandler = dbhandler;
	}

	private Term? term;
	private DatabaseHandler _dbhandler;

    public string TermId
    {
        set
        {
            SetTerm(value);
        }
    }
	
    private async void SetTerm(string query)
    {
        // Problem 1: I used Task<Term>.Result to get back a Term, but according to
        // https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-return-types :
        // "The Result property is a blocking property.
        // If you try to access it before its task is finished, the thread that's currently active is blocked
        // until the task completes and the value is available.
        // n most cases, you should access the value by using Await or await instead of accessing
        // the property directly."
        var result = _dbhandler.GetTerm(int.Parse(query));
		if  (result != null) {
            term = await result;
            if (term != null)
            {
                entryTitle.Text = term.Title;
                dateStart.Date = term.StartDate;
                dateEnd.Date = term.EndDate;
            }
        }		
	}

    private async void btnSave_Clicked(object sender, EventArgs e)
    {
        if (titleValidator.IsNotValid)
        {
            await DisplayAlert("Error", "Title is required.", "OK");
            return;
        }

        if (dateStart.Date >= dateEnd.Date)
        {
            await DisplayAlert("Error", "Start date needs to be after the end date.", "OK");
            return;
        }

        if (term != null)
        {
            term.Title = entryTitle.Text;
            term.StartDate = dateStart.Date;
            term.EndDate = dateEnd.Date;

            await _dbhandler.UpdateTerm(term);
            await Shell.Current.GoToAsync("..");
        }        
    }

    private void btnCancel_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("..");
    }

    private async void btnDelete_Clicked(object sender, EventArgs e)
    {
        if (term != null)
        {
            string decision = await DisplayActionSheet("Do you want to delete this entire term?", "Cancel", "Delete");
            if (decision == "Delete")
            {
                var courseQuery = await _dbhandler.GetCoursesByTerm(term.Id);
                foreach (var course in courseQuery)
                {
                    var assessmentQuery = await _dbhandler.GetAssessmentsByCourse(course.Id);
                    foreach (var item in assessmentQuery)
                    {
                        await _dbhandler.DeleteAssessment(item);
                    }
                    await _dbhandler.DeleteCourse(course);
                }

                await _dbhandler.DeleteTerm(term);
                await Shell.Current.GoToAsync("..");
            }
        }
    }

    private void btnCourses_Clicked(object sender, EventArgs e)
    {
        if (term != null)
        {
            Shell.Current.GoToAsync($"{nameof(CoursePage)}?Id={term.Id}");
        }
    }
}