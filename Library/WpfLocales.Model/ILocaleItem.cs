namespace WPFLocales.Model
{
    public interface ILocaleItem
    {
        string Key { get; }
        string Value { get; }
        string Comment { get; }
    }
}