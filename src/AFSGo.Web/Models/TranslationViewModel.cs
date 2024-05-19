using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AFSGo.Web.Models;

public class TranslationViewModel(
    string translator,
    string language,
    string text,
    string translatedText
)
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "varchar(255)")]
    public string Translator { get; set; } = translator;

    [Column(TypeName = "varchar(255)")]
    public string Language { get; set; } = language;

    [Column(TypeName = "varchar(255)")]
    public string Text { get; set; } = text;

    [Column(TypeName = "varchar(255)")]
    public string TranslatedText { get; set; } = translatedText;
}