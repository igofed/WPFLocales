namespace WPFLocales.Model
{
    public interface ILocaleItem
    {
        string Key { get; set; }
        string Value { get; set; }
        string Comment { get; set; }
    }
}