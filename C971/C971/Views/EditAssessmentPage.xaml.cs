using C971.Class;
using C971.Database;
using Plugin.LocalNotification;
using System;

namespace C971.Views;

[QueryProperty(nameof(OA), "OA")]
[QueryProperty(nameof(PA), "PA")]
[QueryProperty(nameof(AssessmentId), "Id")]
public partial class EditAssessmentPage : ContentPage
{
	public EditAssessmentPage(DatabaseHandler dbhandler)
	{
		InitializeComponent();
        _dbhandler = dbhandler;
        listType.Add("Objective");
        listType.Add("Performance");
    }

    private DatabaseHandler _dbhandler;
    private Assessment? assessment;
    private List<string> listType = new List<string>();
    private bool OA_exist = false;
    private bool PA_exist = false;
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

    public string AssessmentId
    {
        set
        {
            SetAssessment(int.Parse(value));
        }
    }

    private async void SetAssessment(int id)
    {
        int index = 0;
        var result = _dbhandler.GetAssessment(id);
        if (result != null)
        {
            assessment = await result;
            entryName.Text = assessment.Name;
            index = listType.FindIndex(a => a.Equals(assessment.Type));
            if (index < 0)
            {
                index = 0;
            }
            dateStart.Date = assessment.StartDate.Date;
            dateEnd.Date = assessment.EndDate.Date;
            checkboxNotifyStart.IsChecked = assessment.NotifyStart;
            checkboxNotifyEnd.IsChecked = assessment.NotifyEnd;
        }

        if (assessment != null)
        {
            if (assessment.Type.Equals("Objective"))
            {
                //listType.Add(assessment.Type);
                if (PA_exist == true)
                {
                    listType.Remove("Performance");
                }
            }
            else if (assessment.Type.Equals("Performance"))
            {
                //listType.Add(assessment.Type);
                if (OA_exist == true)
                {
                    listType.Remove("Objective");
                }
            }

            var query = _dbhandler.GetCourse(assessment.CourseId);
            var course = await query;
            if (course != null)
            {
                courseStart = course.StartDate;
                courseEnd = course.EndDate;
            }
        }

        pickerType.ItemsSource = listType;
        pickerType.SelectedIndex = index;

    }

    private void btnCancel_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("..");
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

        if (assessment != null)
        {
            assessment.Name = entryName.Text;
            assessment.Type = pickerType.SelectedItem.ToString();
            assessment.StartDate = dateStart.Date;
            assessment.EndDate = dateEnd.Date;
            assessment.NotifyStart = checkboxNotifyStart.IsChecked;
            assessment.NotifyEnd = checkboxNotifyEnd.IsChecked;

            if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }
            CreateNotifications();
            CheckNotifications();

            await _dbhandler.UpdateAssessment(assessment);
            await Shell.Current.GoToAsync("..");
        }
    }

    private async void btnDelete_Clicked(object sender, EventArgs e)
    {
        if (assessment != null)
        {
            string decision = await DisplayActionSheet("Do you want to delete this entry?", "Cancel", "Delete");
            if (decision == "Delete")
            {
                await _dbhandler.DeleteAssessment(assessment);
                await Shell.Current.GoToAsync("..");
                return;
            }
        }
    }

    private void CreateNotifications()
    {
        startNotification = new NotificationRequest
        {
            NotificationId = 464,
            Title = entryName.Text + "Start",
            Description = "Start date of the assessment, " + entryName.Text,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = dateStart.Date,
            },
        };

        endNotification = new NotificationRequest
        {
            NotificationId = 917,
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
        } else
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