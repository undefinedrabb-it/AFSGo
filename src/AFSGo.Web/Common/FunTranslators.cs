namespace AFSGo.Web.Common;

public static class FunTranslators
{
    public const string DisplayName = "FunTranslator";
    public const string Key = "FunTranslator";
    private static readonly List<string> TranslatorList = ["leetspeak"];

    public static bool Contains(string translator) => TranslatorList.Contains(translator);

    public static List<string> Get() => TranslatorList;
}