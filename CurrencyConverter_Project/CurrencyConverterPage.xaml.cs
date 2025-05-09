using Microsoft.Maui.Layouts;
using System.Buffers;

namespace CurrencyConverter_Project;

public partial class CurrencyConverterPage : ContentPage
{
    public Dictionary<string, Label> labelNames = new Dictionary<string, Label>();
    public Dictionary<Entry, string> entryNames = new Dictionary<Entry, string>();

    public Dictionary<string, decimal> selectedExchangeRates = new Dictionary<string, decimal>();

    //tracing last clicked entry for switching between currencies
    Entry? lastClickedEntry;
    Entry? focusedEntry;
    bool onCurrencyLabelTapped = false;


    public CurrencyConverterPage()
	{
        InitializeComponent();

        labelNames.Add("firstLabel", CurrencyLabel1);
        labelNames.Add("secondLabel", CurrencyLabel2);

        entryNames.Add(EntryCurrency1, "firstEntry");
        entryNames.Add(EntryCurrency2, "secondEntry");
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        //disable system keyboard from popping out
        KeyboardHandler.ShowKeyboard(false);
    }
    private void OnEntryFocused(object sender, FocusEventArgs e)
    {
        focusedEntry = sender as Entry;
        lastClickedEntry = sender as Entry;
        BoxView border1, border2;

        // Change the color of the bottom border on focus
        if (EntryCurrency1.IsFocused)
        {
            border1 = BottomBorder1;
            border2 = BottomBorder2;
        }
        else
        {
            border1 = BottomBorder2;
            border2 = BottomBorder1;
        }

        border1.Color = Colors.Cyan;
        border2.Color = Colors.White;

        selectTwoCurrencies();
    }

    private void OnEntryUnfocused(object sender, FocusEventArgs e)
    {
        focusedEntry = null;
        if (!EntryCurrency1.IsFocused && !EntryCurrency2.IsFocused)
        {
            BottomBorder1.Color = Colors.White;
            BottomBorder2.Color = Colors.White;
        }
    }
    private void OnNumericButtonClicked(object sender, EventArgs e)
	{
        Button? button = sender as Button;
        if (button == null) return;

        if (focusedEntry != null)
        focusedEntry.Text += button.Text;
    }
    private void OnEraseButtonClicked(object sender, EventArgs e)
    {
        string currentText = "";
        // Get the current text from the Entry
        if (focusedEntry != null)
            currentText = focusedEntry.Text;

        // Check if the text is not null or empty
        if (!string.IsNullOrEmpty(currentText))
        {
            // Remove the last character
            focusedEntry.Text = currentText.Substring(0, currentText.Length - 1);
        }
    }

    private async void OnCurrencyLabelTapped(object sender, EventArgs e)
    {
        Label currencyLabel = sender as Label;

        string currencyCode1;
        string selectedLabel = labelNames.FirstOrDefault(x => x.Value == currencyLabel).Key;
        string currencyCode2 = selectedLabel.Contains("first") ? labelNames["secondLabel"].Text : labelNames["firstLabel"].Text;
        Entry target = selectedLabel.Contains("first") ? EntryCurrency2 : EntryCurrency1;
        decimal amount = 0;

        try
        {
            amount = selectedLabel.Contains("first") ? decimal.Parse(EntryCurrency1.Text) : decimal.Parse(EntryCurrency2.Text);
        }
        catch { }

        // Logic to show a popup for currency selection
        var popupPage = new PopupPage();

        // Subscribe to the PopupClosed event to handle the returned value
        popupPage.PopupClosed += (s, selectedValue) =>
        {
            if(!CurrencyLabel1.Text.Contains(selectedValue) && !CurrencyLabel2.Text.Contains(selectedValue))
            {
                currencyCode1 = selectedValue.ToString() + " ⌵";
                // Handle the returned value here
                currencyLabel.Text = selectedValue.ToString() + " ⌵";

                selectTwoCurrencies();

                if (amount != 0 && currencyCode1 != null)
                {
                    onCurrencyLabelTapped = true;
                    target.Text = convertValue(currencyCode1, currencyCode2, amount);
                    onCurrencyLabelTapped = false;
                }
            }
        };

        // Display the popup as a modal
        await Navigation.PushModalAsync(popupPage); 
    }

    private string convertValue(string currencyCode1, string currencyCode2, decimal amount) {
        currencyCode1 = currencyCode1.Replace(" ⌵", "");
        currencyCode2 = currencyCode2.Replace(" ⌵", "");

        decimal sourceValue = selectedExchangeRates[currencyCode1];
        decimal destValue = selectedExchangeRates[currencyCode2];
        decimal ratio = destValue / sourceValue;
        decimal result = ratio * amount;

        return Math.Round(result, 2).ToString();
    }

    private void OnEntryTextChanged1(object sender, TextChangedEventArgs e)
    {
        if (onCurrencyLabelTapped) return;

        Entry entry = sender as Entry;
        if(string.IsNullOrWhiteSpace(e.NewTextValue))
        {
            EntryCurrency2.Text = "";
        }
        else
        {
            try
            {
                decimal amount = decimal.Parse(entry.Text);

                string currencyCode1 = labelNames["firstLabel"].Text;
                string currencyCode2 = labelNames["secondLabel"].Text;

                string result = convertValue(currencyCode1, currencyCode2, amount);

                if (lastClickedEntry == EntryCurrency1)
                    EntryCurrency2.Text = result;
            }
            catch (Exception ex){
                DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
    private void OnEntryTextChanged2(object sender, TextChangedEventArgs e)
    {
        if (onCurrencyLabelTapped) return;

        Entry entry = sender as Entry;
        if (string.IsNullOrWhiteSpace(e.NewTextValue))
        {
            EntryCurrency1.Text = "";
        }
        else
        {
            try
            {
                decimal amount = decimal.Parse(entry.Text);

                string currencyCode1 = labelNames["secondLabel"].Text;
                string currencyCode2 = labelNames["firstLabel"].Text;

                string result = convertValue(currencyCode1, currencyCode2, amount);

                if (lastClickedEntry == EntryCurrency2)
                    EntryCurrency1.Text = result;
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
    private void calculateValueAfterChecked()
    {
        if (!string.IsNullOrWhiteSpace(EntryCurrency1.Text) && !string.IsNullOrWhiteSpace(EntryCurrency2.Text))
        {
            Entry targetEntry;
            string? currencyCode1;
            string? currencyCode2;

            if (entryNames[lastClickedEntry].Contains("first"))
            {
                currencyCode1 = labelNames["firstLabel"].Text;
                currencyCode2 = labelNames["secondLabel"].Text;
                targetEntry = EntryCurrency2;
            }
            else
            {
                currencyCode1 = labelNames["secondLabel"].Text;
                currencyCode2 = labelNames["firstLabel"].Text;
                targetEntry = EntryCurrency1;
            }

            decimal amount = decimal.Parse(lastClickedEntry.Text);

            targetEntry.Text = convertValue(currencyCode1, currencyCode2, amount);
        }
    }

    private async void OnArrowLabelTapped(Object sender, EventArgs e)
    {
        await AnimateChildrenFadeOut(CurrencyValuesFlex);

        if (CurrencyValuesFlex.Direction == FlexDirection.Column)
        {
            CurrencyValuesFlex.Direction = FlexDirection.ColumnReverse;

            //changing entry indicators to being able to bind a label with entry
            labelNames["firstLabel"] = CurrencyLabel2;
            labelNames["secondLabel"] = CurrencyLabel1;
        }
        else
        {
            CurrencyValuesFlex.Direction = FlexDirection.Column;

            labelNames["firstLabel"] = CurrencyLabel1;
            labelNames["secondLabel"] = CurrencyLabel2;
        }

        await AnimateChildrenFadeIn(CurrencyValuesFlex);

        calculateValueAfterChecked();
    }

    private async Task AnimateChildrenFadeOut(FlexLayout flexLayout)
    {
        var tasks = flexLayout.Children
            .OfType<View>()
            .Where(view => view != ArrowLabel)
            // Fade out over 250ms
            .Select(view => view.FadeTo(0, 250))
            .ToArray();
        // Wait for all animations to complete
        await Task.WhenAll(tasks);
    }

    private async Task AnimateChildrenFadeIn(FlexLayout flexLayout)
    {
        var tasks = flexLayout.Children
            .OfType<View>()
            .Where(view => view != ArrowLabel)
            .Select(view =>
            {
                // Start with 0 opacity
                view.Opacity = 0;
                // Fade in over 250ms
                return view.FadeTo(1, 250); 
            })
            .ToArray();
        // Wait for all animations to complete
        await Task.WhenAll(tasks);
    }

    private void selectTwoCurrencies()
    {
        if (App.CurrencyRates == null)
            return;
        foreach (var currencyRate in App.CurrencyRates) {
            bool matchedFirstLabel = labelNames["firstLabel"].Text.Contains(currencyRate.Key);
            bool matchedSecondLabel = labelNames["secondLabel"].Text.Contains(currencyRate.Key);

            if (matchedFirstLabel || matchedSecondLabel)
            {
                if (!selectedExchangeRates.ContainsKey(currencyRate.Key))
                    selectedExchangeRates.Add(currencyRate.Key, currencyRate.Value);
            }
        }
    }
}