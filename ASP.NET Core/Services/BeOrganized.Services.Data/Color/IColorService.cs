namespace BeOrganized.Services.Data.Color
{
    using System.Collections.Generic;

    using BeOrganized.Data.Models;

    public interface IColorService
    {
        ICollection<Color> GetAllColors();
    }
}
