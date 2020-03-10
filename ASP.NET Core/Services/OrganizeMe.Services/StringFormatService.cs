namespace OrganizeMe.Services
{
    public class StringFormatService : IStringFormatService
    {
        public string RemoveWhitespaces(string text)
        {
            return text.Replace(" ", string.Empty);
        }
    }
}
