using C971.Class;
using C971.Database;
using Plugin.LocalNotification;

namespace C971.Views;

[QueryProperty(nameof(OA), "OA")]
[QueryProperty(nameof(PA), "PA")]
[QueryProperty(nameof(CourseId), "CourseId")]
public partial class AddAssessmentPage : ContentPage
{
	public AddAssessmentPage(DatabaseHandler dbhandler)
	{
		InitializeComponent();
        _dbhandler = dbhandler;   
    }

    private DatabaseHandler _dbhandler;
    private List<string> listType = new List<string>();
    private bool OA_exist = false;
    private bool PA_exist = false;
    private int _courseId;
    public NotificationRequest startNotification = new();
    public NotificationRequest endNotification = new();
    private DateTime courseStart;
    private DateTime courseEnd;


    public string OA
    {
        set
        {
            OA_exist = bool.Parse(value);
        }
    }

    public string PA
    {
        set
        {
            PA_exist = bool.Parse(value);
        }

    }

    public string CourseId
    {
        set
        {
            _courseId = int.Parse(value);
            SetCourseInfo(_courseId);
        }
    }

    private async void SetCourseInfo(int courseId)
    {
        var query = _dbhandler.GetCourse(courseId);
        var course = await query;
        if (course != null)
        {
            courseStart = course.StartDate;
            courseEnd = course.EndDate;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (OA_exist != true)
        {
            listType.Add("Objective");
        }
        if (PA_exist != true)
        {
            listType.Add("Performance");
        }

        pickerType.ItemsSource = listType;
        pickerType.SelectedIndex = 0;
    }

    private async void btnSave_Clicked(object sender, EventArgs e)
    {   
        if (nameValidator.IsNotValid)
        {
            await DisplayAlert("Error", "Name is required.", "OK");
            return;
        }

        if (dateStart.Date < courseStart || dateEnd.Date > courseEnd)
        {
            await DisplayAlert("Error", "Assessment dates are outside the established course period.", "OK");
            return;
        }

        if (dateStart.Date >= dateEnd.Date)
        {
            await DisplayAlert("Error", "Start date needs to be after the end date.", "OK");
            return;
        }

        var assessment = new Assessment
        {
            Name = entryName.Text,
            Type = pickerType.SelectedItem.ToString(),
            StartDate = dateStart.Date,
            EndDate = dateEnd.Date,
            CourseId = _courseId,
            NotifyStart = checkboxNotifyStart.IsChecked,
            NotifyEnd = checkboxNotifyEnd.IsChecked,
        };

        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        {
            await LocalNotificationCenter.Current.RequestNotificationPermission();
        }
        CreateNotifications();
        CheckNotifications();

        await _dbhandler.CreateAssessment(assessment);
        await Shell.Current.GoToAsync("..");      
    }

    private void btnCancel_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("..");
    }

    private void CreateNotifications()
    {
        startNotification = new NotificationRequest
        {
            NotificationId = 173,
            Title = entryName.Text + "Start",
            Description = "Start date of the assessment, " + entryName.Text,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = dateStart.Date,
            },
        };

        endNotification = new NotificationRequest
        {
            NotificationId = 810,
            Title = entryName.Text + "End",
            Description = "End date of the assessment, " + entryName.Text,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = dateEnd.Date,
            },
        };
    }

    private void CheckNotifications()
    {
        if (checkboxNotifyStart.IsChecked)
        {
            LocalNotificationCenter.Current.Show(startNotification);
        }
        else
        {
            LocalNotificationCenter.Current.Cancel(startNotification.NotificationId);
        }

        if (checkboxNotifyEnd.IsChecked)
        {
            LocalNotificationCenter.Current.Show(endNotification);
        }
        else
        {
            LocalNotificationCenter.Current.Cancel(endNotification.NotificationId);
        }
    }
}