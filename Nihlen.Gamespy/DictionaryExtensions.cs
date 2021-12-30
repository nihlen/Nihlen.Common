namespace Nihlen.Gamespy;

public static class DictionaryExtensions
{
    public static string GetValue(this Dictionary<string, string> dict, string key)
    {
        return dict.ContainsKey(key) ? dict[key] : null;
    }
}
