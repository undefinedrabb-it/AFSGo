namespace AFSGo.Web.Models;

public record TranslationRequestModel(
    string Translator,
    string Language,
    string InputText
);