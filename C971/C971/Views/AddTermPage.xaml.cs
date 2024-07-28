using C971.Class;
using C971.Database;

namespace C971.Views;

public partial class AddTermPage : ContentPage
{
	public AddTermPage(DatabaseHandler dbhandler)
	{
		InitializeComponent();
        _dbhandler = dbhandler;
	}

    private DatabaseHandler _dbhandler;

    private void btnCancel_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("..");
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

        var term = new Term
        {
            Title = entryTitle.Text,
            StartDate = dateStart.Date,
            EndDate = dateEnd.Date,
        };
        await _dbhandler.CreateTerm(term);
        await Shell.Current.GoToAsync("..");
    }
}