namespace AFSGo.Web.Common;

public static class DummyTranslators
{
    public const string DisplayName = "DummyTranslator";
    public const string Key = "DummyTranslator";

    private static readonly List<string> TranslatorList = ["Dummy"];

    public static bool Contains(string translator) => TranslatorList.Contains(translator);

    public static List<string> Get() => TranslatorList;
}