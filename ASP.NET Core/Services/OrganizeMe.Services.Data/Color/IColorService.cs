namespace OrganizeMe.Services.Data.Color
{
    using System.Collections.Generic;

    using OrganizeMe.Data.Models;

    public interface IColorService
    {
        ICollection<Color> GetAllColors();
    }
}
