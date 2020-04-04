namespace OrganizeMe.Services.Data.Color
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using OrganizeMe.Data.Models;

    public interface IColorService
    {
        Task<ICollection<Color>> GetAllColorsAsync();
    }
}
