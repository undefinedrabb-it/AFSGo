using AFSGo.Web.Common;
using AFSGo.Web.Service.Interfaces;

namespace AFSGo.Web.Service.Translation;

public class TranslationFactory(ILogger<TranslationFactory> logger, IServiceProvider serviceProvider)
{
    public ITranslationService GetStreamService(string translator)
    {
        switch (translator)
        {
            case FunTranslators.Key:
                return (ITranslationService)serviceProvider.GetService(typeof(FunTranslationService))!;
            default:
                if (translator != DummyTranslators.Key)
                    logger.LogError($"Invalid Translator: {translator}, defaulting to DummyTranslatorService");

                return (ITranslationService)serviceProvider.GetService(typeof(DummyTranslatorService))!;
        }
    }
}