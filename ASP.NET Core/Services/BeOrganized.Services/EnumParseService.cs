namespace BeOrganized.Services
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    public class EnumParseService : IEnumParseService
    {
        public string GetEnumDescription(string name, Type typeOfEnum)
        {
            FieldInfo specificField = typeOfEnum.GetField(name);

            if (specificField != null)
            {
                DescriptionAttribute attr =
                       Attribute.GetCustomAttribute(
                           specificField,
                           typeof(DescriptionAttribute)) as DescriptionAttribute;

                if (attr != null)
                {
                    return attr.Description;
                }
            }

            return null;
        }

        public bool IsEnumValid<TEnum>(string description)
        {
            return this.FindEnumNameByDescription<TEnum>(description) == null ? false : true;
        }

        public TEnum Parse<TEnum>(string description)
        {
            return (TEnum)Enum.Parse(
                             typeof(TEnum),
                             this.FindEnumNameByDescription<TEnum>(description));
        }

        private string FindEnumNameByDescription<TEnum>(string description)
        {
            var specificFields = typeof(TEnum).GetFields().Where(x => x.CustomAttributes.Any());
            if (specificFields != null)
            {
                foreach (FieldInfo field in specificFields)
                {
                    DescriptionAttribute attr =
                      Attribute.GetCustomAttribute(
                          field,
                          typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (description == attr.Description)
                    {
                        return field.Name;
                    }
                }
            }

            return null;
        }
    }
}
