namespace CurrencyConverter_Project
{
    public class CurrencyWithCodedFlag
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Country { get; set; }
        public string? CountryCode { get; set; }
        public string? Flag { get; set; }
    }
    public class CurrencyWithDecodedFlag
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public ImageSource Flag { get; set; }

        public CurrencyWithDecodedFlag(string code, string name, string country, string countryCode, ImageSource flagImage)
        {
            Code = code;
            Name = name;
            Country = country;
            CountryCode = countryCode;
            Flag = flagImage;
        }
    }
}
