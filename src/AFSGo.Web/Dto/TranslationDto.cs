using AFSGo.Web.Models;

namespace AFSGo.Web.Dto;

public record TranslationDto(
    string Translator,
    string Language,
    string Text
)
{
    public static TranslationDto FromModel(TranslationRequestModel model) =>
        new(model.Translator, model.Language, model.InputText);

    public TranslationResponseModel ToModel() =>
        new(Text);

    public TranslationViewModel ToEntity(string translatedText) =>
        new(Translator, Language, Text, translatedText);
}