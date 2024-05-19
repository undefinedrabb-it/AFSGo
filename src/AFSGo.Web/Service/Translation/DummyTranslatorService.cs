using AFSGo.Web.Data;
using AFSGo.Web.Dto;
using AFSGo.Web.Service.Interfaces;

namespace AFSGo.Web.Service.Translation;

public class DummyTranslatorService(ApplicationDbContext context) : ITranslationService
{
    public async Task<TranslationDto?> TranslateAsync(TranslationDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        await context.TranslationViewModel.AddAsync(dto.ToEntity(dto.Text), cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return dto;
    }
}