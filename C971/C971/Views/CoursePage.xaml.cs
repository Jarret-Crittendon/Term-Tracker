using C971.Class;
using C971.Database;
using System.Collections.ObjectModel;

namespace C971.Views;


[QueryProperty(nameof(TermId), "Id")]
public partial class CoursePage : ContentPage
{
	public CoursePage(DatabaseHandler dbhandler)
	{
		InitializeComponent();
        _dbhandler = dbhandler;
	}

    private DatabaseHandler _dbhandler;
    public string TermId
    {
        set
        {
            _termId =  int.Parse(value);
        }
    }

    private int _termId;
    private int courseCount;
    private const int MAX_COURSE = 6;

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadCourses();
    }

    private async void LoadCourses()
    {
        var resultReturned = _dbhandler.GetCoursesByTerm(_termId);
        var unpacked = await resultReturned;
        courseCount = unpacked.Count();
        var courses = new ObservableCollection<Course>(unpacked);
        listCourses.ItemsSource = courses;
    }

    private void listCourses_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (listCourses.SelectedItem != null)
        {
            Shell.Current.GoToAsync($"{nameof(EditCoursePage)}?Id={((Course)listCourses.SelectedItem).Id}");
        }
    }

    private void listCourses_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        listCourses.SelectedItem = null;
    }

    private void btnAdd_Clicked(object sender, EventArgs e)
    {
        if (courseCount >= MAX_COURSE)
        {
            DisplayAlert("Error", "A maximum of six courses are allowed per term.", "OK");
            return;
        }
        Shell.Current.GoToAsync($"{nameof(AddCoursePage)}?Id={_termId}");
    }

    private void btnReturn_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("..");
    }
}