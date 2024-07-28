using C971.Class;
using C971.Database;
using Plugin.LocalNotification;

namespace C971.Views;

[QueryProperty(nameof(PageTermId), "Id")]
public partial class AddCoursePage : ContentPage
{
    
	public AddCoursePage(DatabaseHandler dbhandler)
	{
		InitializeComponent();
        _dbhandler = dbhandler;
        listStatus.Add("Completed");
        listStatus.Add("In Progress");
        listStatus.Add("Withdrawn");
        pickerStatus.ItemsSource = listStatus;
        pickerStatus.SelectedIndex = 0;
    }

    
    private int _termId;
    private DateTime _termStart;
    private DateTime _termEnd;
    public string PageTermId
    {
        set
        {
            _termId = int.Parse(value);
            SetTermInfo(_termId);
        }
    }
    
    private async void SetTermInfo(int termId)
    {
        var query = _dbhandler.GetTerm(termId);
        var term = await query;
        if (term != null)
        {
            _termStart = term.StartDate;
            _termEnd = term.EndDate;
        }
    }


    private DatabaseHandler _dbhandler;
    private List<string> listStatus = new();
    public NotificationRequest startNotification = new();
    public NotificationRequest endNotification = new();


    private void CreateNotifications()
    {
        startNotification = new NotificationRequest
        {
            NotificationId = 375,
            Title = entryName.Text + "Start",
            Description = "Start date of the course, " + entryName.Text,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = dateStart.Date,
            },
        };

        endNotification = new NotificationRequest
        {
            NotificationId = 780,
            Title = entryName.Text + "End",
            Description = "End date of the course, " + entryName.Text,
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

    private async void btnSave_Clicked(object sender, EventArgs e)
    {
        if (nameValidator.IsNotValid)
        {
            await DisplayAlert("Error", "Name is required.", "OK");
            return;
        }

        if (dateStart.Date < _termStart || dateEnd.Date > _termEnd)
        {
            await DisplayAlert("Error", "Course dates are outside the established term period.", "OK");
            return;
        }

        if (dateStart.Date >= dateEnd.Date)
        {
            await DisplayAlert("Error", "Start date needs to be after the end date.", "OK");
            return;
        }

        var course = new Course
        {
            Name = entryName.Text,
            TermId = _termId,
            StartDate = dateStart.Date,
            EndDate = dateEnd.Date,
            InstructorEmail = entryInstructorEmail.Text,
            InstructorName = entryInstructorName.Text,
            InstructorPhone = entryInstructorPhone.Text,
            Notes = entryNote.Text,
            NotifyStart = checkboxNotifyStart.IsChecked,
            NotifyEnd = checkboxNotifyEnd.IsChecked,
            Status = pickerStatus.SelectedItem.ToString(),
        };

        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        {
            await LocalNotificationCenter.Current.RequestNotificationPermission();
        }
        CreateNotifications();
        CheckNotifications();

        await _dbhandler.CreateCourse(course);
        await Shell.Current.GoToAsync("..");
    }

    private void btnCancel_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("..");
    }   
}