namespace CurrencyConverter_Project;

public partial class ExchangeRatePage : ContentPage
{
    public List<CurrencyWithDecodedFlag> CurrenciesWithFlagsList { get; set; }
    public List<Item> itemList { get; set; }

    public ExchangeRatePage()
	{
		InitializeComponent();        
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        //ItemsListView.ItemsSource = fillUpListAsync();
        KeyboardHandler.ShowKeyboard(true);
    }

    private List<Item> fillUpListAsync()
    {
        string mainCurrency = CurrencyLabel.Text;
        mainCurrency = mainCurrency.Replace(" ⌵", "");

        decimal sourceRate = 0;
        List<Item> items= new List<Item>();

        ImageSource flag = null;

        if (App.CurrencyRates == null)
            return new List<Item>();

        foreach (var currencyRate in App.CurrencyRates)
        {
            bool matchedLabel = mainCurrency.Equals(currencyRate.Key);

            if (matchedLabel)
            {
                sourceRate = currencyRate.Value;
                break;
            }
        }

        foreach (var currencyRate in App.CurrencyRates)
        {
            decimal cost = Math.Round(sourceRate / currencyRate.Value, 2);
            decimal rate = Math.Round(currencyRate.Value / sourceRate, 2);
            string name = currencyRate.Key;

            var foundItem = App.CurrenciesWithFlags.FirstOrDefault(item => item.Code == name);
            if (foundItem != null)
            {
                flag = foundItem.Flag;
                Item item = new Item(flag, mainCurrency, name, cost, rate, foundItem.Name);
                items.Add(item);
            }
        }

        return items;
    }
    protected async void OnCurrencyLabelTapped(object sender, EventArgs e) {
        Label currencyLabel = sender as Label;

        var popupPage = new PopupPage();

        popupPage.PopupClosed += (s, selectedValue) =>
        {
            if (!CurrencyLabel.Text.Contains(selectedValue))
            {
                // Handle the returned value here
                currencyLabel.Text = selectedValue.ToString() + " ⌵";
            }
            ItemsListView.ItemsSource = fillUpListAsync();
        };

        await Navigation.PushModalAsync(popupPage);
    }
}

public class Item
{
    public ImageSource Flag { get; set; }
    public string CurrencyName { get; set; }
    public string MainCurrencyName { get; set; }
    public decimal CurrencyCost { get; set; }
    public decimal CurrencyRate { get; set; }

    public string CurrencyCountry { get; set; }

    public Item(ImageSource flagImage, string mainCurrencyName, string name, decimal cost, decimal rate, string currencyCountry)
    {
        Flag = flagImage;
        CurrencyName = name;
        MainCurrencyName = mainCurrencyName;
        CurrencyCost = cost;
        CurrencyRate = rate;
        CurrencyCountry = currencyCountry;
    }
}