using C971.Class;
using C971.Database;
using Plugin.LocalNotification;

namespace C971.Views;

[QueryProperty(nameof(CourseId), "Id")]
public partial class EditCoursePage : ContentPage
{
	public EditCoursePage(DatabaseHandler dbhandler)
	{
		InitializeComponent();
        _dbhandler = dbhandler;
        listStatus.Add("Completed");
        listStatus.Add("In Progress");
        listStatus.Add("Withdrawn");
        pickerStatus.ItemsSource = listStatus;
        pickerStatus.SelectedIndex = 0;
    }

    private DatabaseHandler _dbhandler;
    private Course? course;
    private List<string> listStatus = new List<string>();
    public NotificationRequest startNotification = new();
    public NotificationRequest endNotification = new();
    private DateTime _termStart;
    private DateTime _termEnd;

    private void CreateNotifications()
    {
        startNotification = new NotificationRequest
        {
            NotificationId = 556,
            Title = entryName.Text + "Start",
            Description = "Start date of the course, " + entryName.Text,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = dateStart.Date,
            },
        };

        endNotification = new NotificationRequest
        {
            NotificationId = 121,
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


    public string CourseId
    {
        set
        {
            SetCourse(value);
        }
    }

    

    private async void SetCourse(string courseId)
    {
        var result = _dbhandler.GetCourse(int.Parse(courseId));
        if (result != null)
        {
            int index;
            course = await result;
            if (course != null)
            {
                entryName.Text = course.Name;
                dateStart.Date = course.StartDate;
                dateEnd.Date = course.EndDate;
                index = listStatus.FindIndex(a => a.Equals(course.Status));
                if (index < 0)
                {
                    pickerStatus.SelectedIndex = 2;
                }
                else
                {
                    pickerStatus.SelectedIndex = index;
                }
                entryInstructorName.Text = course.InstructorName;
                entryInstructorPhone.Text = course.InstructorPhone;
                entryInstructorEmail.Text = course.InstructorEmail;
                entryNote.Text = course.Notes;
                checkboxNotifyStart.IsChecked = course.NotifyStart;
                checkboxNotifyEnd.IsChecked = course.NotifyEnd;

                var query = _dbhandler.GetTerm(course.TermId);
                var term = await query;
                if (term != null)
                {
                    _termStart = term.StartDate;
                    _termEnd = term.EndDate;
                }
            }
        }
    }

    private void btnAssessments_Clicked(object sender, EventArgs e)
    {
        if (course != null)
        {
            Shell.Current.GoToAsync($"{nameof(AssessmentPage)}?Id={course.Id}");
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

        if (course != null)
        {
            course.Name = entryName.Text;
            course.StartDate = dateStart.Date;
            course.EndDate = dateEnd.Date;
            course.InstructorEmail = entryInstructorEmail.Text;
            course.InstructorName = entryInstructorName.Text;
            course.InstructorPhone = entryInstructorPhone.Text;
            course.Notes = entryNote.Text;
            course.NotifyStart = checkboxNotifyStart.IsChecked;
            course.NotifyEnd = checkboxNotifyEnd.IsChecked;
            course.Status = pickerStatus.SelectedItem.ToString();

            if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }
            CreateNotifications();
            CheckNotifications();

            await _dbhandler.UpdateCourse(course);
            await Shell.Current.GoToAsync("..");
        }
    }

    private async void btnDelete_Clicked(object sender, EventArgs e)
    {
        if (course != null)
        {
            string decision = await DisplayActionSheet("Do you want to delete this course?", "Cancel", "Delete");
            if (decision == "Delete")
            {
                var query = await _dbhandler.GetAssessmentsByCourse(course.Id);
                foreach (var item in query)
                {
                    await _dbhandler.DeleteAssessment(item);
                }
                await _dbhandler.DeleteCourse(course);
                await Shell.Current.GoToAsync("..");
            }
        }
    }

    private void btnCancel_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("..");
    }

    private async void btnShare_Clicked(object sender, EventArgs e)
    {
        string text = entryNote.Text;
        /* if (course != null)
        {
            text = course.Notes;
        } */

        if (text == null || text == string.Empty)
        {
            await DisplayAlert("Error", "Note field must contain text for it to be shared.", "OK");
            return;
        }
     
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Text = text,
            Title = "Share Notes"
        });
    }
}