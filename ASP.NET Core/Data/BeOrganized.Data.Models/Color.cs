namespace BeOrganized.Data.Models
{
    using BeOrganized.Data.Common.Models;

    public class Color : BaseModel<int>
    {
        public string Name { get; set; }

        public string Hex { get; set; }
    }
}
