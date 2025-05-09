
using System.Text.Json;
namespace CurrencyConverter_Project
{
    public partial class App : Application
    {
        public static Dictionary<string, decimal>? CurrencyRates { get; set; }
        public static List<CurrencyWithDecodedFlag>? CurrenciesWithFlags { get; set; }
       
        public App()
        {
            InitializeComponent();
            GetDecodedCurrencyWithFlagsListAsync();
            GetExchangeRatesFromFileAsync();

            MainPage = new MainPage();
        }
        protected override async void OnStart()
        {
            base.OnStart();

            // Calling the fetch function when the app starts
            _ = FetchDataAsync();
        }

        private async Task FetchDataAsync()
        {
            try
            {
                // Fetching data from URL
                const string URL = "https://open.er-api.com/v6/latest/USD";

                // Creating an HttpClient instance
                using HttpClient client = new HttpClient();

                // Making a GET request to the URL
                HttpResponseMessage response = await client.GetAsync(URL);

                // Ensuring the request was successful
                response.EnsureSuccessStatusCode();

                // Reading the response content as a string
                string fetchedData = await response.Content.ReadAsStringAsync();

                // Saving data to file
                await SaveDataToFileAsync(fetchedData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
            }
        }

        private async Task SaveDataToFileAsync(string data)
        {
            try
            {
                // Defining the file path
                const string FILENAME = "currency-rate.json";
                string filePath = Path.Combine(FileSystem.AppDataDirectory, FILENAME);

                // Writing data to the file asynchronously
                await File.WriteAllTextAsync(filePath, data);
            }
            catch (Exception ex)
            {
                // Handle any exceptions related to file I/O
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
        }

        private async void GetExchangeRatesFromFileAsync()
        {
            try
            {
                const string FILENAME = "currency-rate.json";
                string filePath = Path.Combine(FileSystem.AppDataDirectory, FILENAME);

                if (File.Exists(filePath))
                {
                    string jsonData = await File.ReadAllTextAsync(filePath);
                    var jsonDocument = JsonDocument.Parse(jsonData);
                    var jsonRates = jsonDocument.RootElement.GetProperty("rates");

                    // Deserialize the "rates" JSON object directly into a Dictionary<string, decimal>
                    var dictionaryOfRates = JsonSerializer.Deserialize<Dictionary<string, decimal>>(jsonRates.GetRawText());

                    CurrencyRates = dictionaryOfRates;
                }
                else
                {
                    Console.WriteLine("File does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading data: {ex.Message}");
            }
        }
        
        private async Task<List<CurrencyWithCodedFlag>> LoadBundledFile()
        {
            try
            {
                using var stream = await FileSystem.Current.OpenAppPackageFileAsync("Resources/Raw/currencies-with-flags.json");
                using var reader = new StreamReader(stream);
                string jsonContent = await reader.ReadToEndAsync();

                // Deserialize the JSON content
                List<CurrencyWithCodedFlag> currencyList = JsonSerializer.Deserialize<List<CurrencyWithCodedFlag>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                return currencyList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                return new List<CurrencyWithCodedFlag>();
            }
        }

        private async void GetDecodedCurrencyWithFlagsListAsync() 
        {
            List<CurrencyWithCodedFlag> currenciesWithBase64Flags = await LoadBundledFile();
            List<CurrencyWithDecodedFlag> currencyList = new List<CurrencyWithDecodedFlag>();

            if (currenciesWithBase64Flags != null)
            {

                foreach (CurrencyWithCodedFlag currency in currenciesWithBase64Flags)
                {
                    string base64ImageWithPrefix = currency.Flag;
                    string base64Image = base64ImageWithPrefix.Replace("data:image/png;base64,", "");

                    // Decode the Base64 string to a byte array
                    byte[] imageBytes = Convert.FromBase64String(base64Image);

                    // Convert byte array to ImageSource
                    ImageSource imageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));

                    CurrencyWithDecodedFlag newCurrency = new CurrencyWithDecodedFlag(currency.Code, currency.Name, currency.Country, currency.CountryCode, imageSource);
                    currencyList.Add(newCurrency);
                }
            }

            CurrenciesWithFlags = currencyList;
        }
    }
}
