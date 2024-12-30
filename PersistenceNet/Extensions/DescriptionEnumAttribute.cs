namespace PersistenceNet.Extensions
{
    public class DescriptionEnumAttribute(string description)
        : Attribute
    {
        public string Description { get; set; } = description;
    }

    public static class EnumExtension
    {
        public static string? GetDescription(this Enum value)
        {
            try
            {
                var type = value.GetType();
                var info = type.GetField(value.ToString());

                if (info != null)
                {
                    var atributos = info.GetCustomAttributes(typeof(DescriptionEnumAttribute), false) as DescriptionEnumAttribute[];
                    return atributos!.Length == 0 ? null : atributos[0].Description;
                }
                else { return string.Empty; }
            }
            catch
            {
                return $"desconhecido ({value})";
            }
        }
    }
}