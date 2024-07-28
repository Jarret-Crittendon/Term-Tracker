using C971.Class;
using C971.Database;
using System.Collections.ObjectModel;

namespace C971.Views;

[QueryProperty(nameof(CourseId), "Id")]
public partial class AssessmentPage : ContentPage
{
	public AssessmentPage(DatabaseHandler dbhandler)
	{
		InitializeComponent();
        _dbhandler = dbhandler;
	}

    private DatabaseHandler _dbhandler;
    private int _courseId;
    private bool OA_Set;
    private bool PA_Set;



    public string CourseId
    {
        set
        {
            _courseId = int.Parse(value);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        OA_Set = false;
        PA_Set = false;
        LoadAssessments();
    }

    private async void LoadAssessments()
    {
        var results = _dbhandler.GetAssessmentsByCourse(_courseId);
        var unpacked = await results;
        var assessments = new ObservableCollection<Assessment>(unpacked);
        listAssessments.ItemsSource = assessments;
        if (assessments.Any())
        {
            foreach( var assessment in assessments)
            {
                if (assessment.Type.Equals("Objective")) {
                    OA_Set = true;
                } else if (assessment.Type.Equals("Performance"))
                {
                    PA_Set = true;
                }
            }
        }
    }




    private void listAssessments_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (listAssessments.SelectedItem != null)
        {
            Shell.Current.GoToAsync($"{nameof(EditAssessmentPage)}?Id={((Assessment)listAssessments.SelectedItem).Id}&OA={OA_Set}&PA={PA_Set}");
        }
    }

    private void listAssessments_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        listAssessments.SelectedItem = null;
    }

    private void btnAdd_Clicked(object sender, EventArgs e)
    {
        // If one OA and one PA are present, raise a display box preventing movement to the add screen
        // if there is one assessment, but not the other, prevent adding another of the same type

        if (OA_Set == true && PA_Set == true)
        {
            DisplayAlert("Error", "No more assessments can be added for this course.", "OK");
            return;
        }

        Shell.Current.GoToAsync($"{nameof(AddAssessmentPage)}?CourseId={_courseId}&OA={OA_Set}&PA={PA_Set}");

    }

    private void btnReturn_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("..");
    }
}