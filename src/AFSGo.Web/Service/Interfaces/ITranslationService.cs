using AFSGo.Web.Dto;

namespace AFSGo.Web.Service.Interfaces;

public interface ITranslationService
{
    Task<TranslationDto?> TranslateAsync(TranslationDto dto, CancellationToken cancellationToken = default);
}