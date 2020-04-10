namespace BeOrganized.Services
{
    using System;

    public interface IEnumParseService
    {
        string GetEnumDescription(string name, Type typeOfEnum);

        bool IsEnumValid<TEnum>(string description);

        TEnum Parse<TEnum>(string description);
    }
}
