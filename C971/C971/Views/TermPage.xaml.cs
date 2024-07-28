using C971.Class;
using C971.Database;
using System.Collections.ObjectModel;

namespace C971.Views;

public partial class TermPage : ContentPage
{
	public TermPage(DatabaseHandler dbhandler)
	{
        InitializeComponent();
        _dbhandler = dbhandler;
    }

    private DatabaseHandler _dbhandler;

    protected override void OnAppearing()
    {
        base.OnAppearing();

        LoadTerms();
    }

    private async void LoadTerms()
	{
        var resultReturned = _dbhandler.GetTerms();
        var unpacked = await resultReturned;
        var terms = new ObservableCollection<Term>(unpacked);
        listTerms.ItemsSource = terms;
	}

    private void listTerms_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (listTerms.SelectedItem != null)
        {
            Shell.Current.GoToAsync($"{nameof(EditTermPage)}?Id={((Term)listTerms.SelectedItem).Id}");
        }
    }

    private void listTerms_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        listTerms.SelectedItem = null;
    }

    private void btnAdd_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync($"{nameof(AddTermPage)}");
    }
}