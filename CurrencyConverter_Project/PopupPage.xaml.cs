namespace CurrencyConverter_Project;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Core.Platform;

public partial class PopupPage : ContentPage
{
    public event EventHandler<string> PopupClosed;
    public List<CurrencyWithDecodedFlag> ListOfCurrencyItems { get; set; }

    public string selectedCurrencyCode = string.Empty;
    public PopupPage()
	{
        InitializeComponent();
        ListOfCurrencyItems = App.CurrenciesWithFlags;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        KeyboardHandler.ShowKeyboard(true);
    }
    private async void OnCurrencyItemTapped(object sender, EventArgs e)
    {
        if (sender is Grid grid)
        {
            // Iterate through children of HorizontalStackLayout
            foreach (var child in grid.Children)
            {
                // Check if the child is a Label
                if (child is Label label && label.BindingContext is CurrencyWithDecodedFlag viewModel)
                {
                    // Access the Code property from the ViewModel bound to the Label
                    selectedCurrencyCode = viewModel.Code;

                    break;
                }
            }
        }
        // Hide keyboard after menu closed
        searchEntry.HideKeyboardAsync(CancellationToken.None);

        // Fire the event with the selected value
        PopupClosed?.Invoke(this, selectedCurrencyCode);

        // Close the popup
        await Navigation.PopModalAsync();
    }

    private async void ClosePopup_Clicked(object sender, EventArgs e)
    {
        // Close the popup
        await Navigation.PopModalAsync();
    }

    public void Search(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue;

        if (string.IsNullOrWhiteSpace(searchText))
        {
            // Show all items if search text is empty
            ItemsListView.ItemsSource = ListOfCurrencyItems;
        }
        else
        {
            // Filter items based on the search text
            var filteredItems = ListOfCurrencyItems.Where(
                item => item.Code.ToLower().StartsWith(searchText.ToLower()) || 
                item.Name.ToLower().Contains(searchText.ToLower()) ||
                item.Country.ToLower().Contains(searchText.ToLower()) ||
                item.CountryCode.ToLower().StartsWith(searchText.ToLower())
                ).ToList();
            ItemsListView.ItemsSource = filteredItems;
        }
    }
}