using System.Text.Json;
using AFSGo.Web.Common;
using AFSGo.Web.Data;
using AFSGo.Web.Dto;
using AFSGo.Web.Service.Interfaces;
using AFSGo.Web.Service.Settings;
using Microsoft.Extensions.Options;

namespace AFSGo.Web.Service.Translation;

public record TranslateSuccess(int Total);

public record Contents(string Translation, string Text, string Translated);

public record TranslateResponse(TranslateSuccess Success, Contents Contents);

public class FunTranslationService(
    ILogger<FunTranslationService> logger,
    IOptions<FunTranslatorSettings> settings,
    ApplicationDbContext context,
    IHttpClientFactory factory
) : ITranslationService

{
    private readonly FunTranslatorSettings _settings = settings.Value;

    public async Task<TranslationDto?> TranslateAsync(TranslationDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        if (FunTranslators.Key != dto.Translator)
        {
            logger.LogError($"Invalid Translator: {dto.Translator}");
            return null;
        }

        if (!FunTranslators.Contains(dto.Language))
        {
            logger.LogError($"Not supported language: {dto.Language}");
            return null;
        }

        var httpClient = factory.CreateClient();
        httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        httpClient.DefaultRequestHeaders.Add("X-Funtranslations-Api-Secret", _settings.ApiKey);
        try
        {
            // Make the API call
            logger.LogInformation($"API Call: Translating '{dto.Text}'");

            var reqBody = new Dictionary<string, string> { { "text", dto.Text } };
            var response = await httpClient.PostAsync(
                $"/translate/{dto.Language}.json",
                new FormUrlEncodedContent(reqBody),
                cancellationToken
            );

            // Check if the call was successful
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"API Call Failed: {response.ReasonPhrase}");
                return null;
            }

            // Read the response content
            var result = await response.Content.ReadAsStringAsync(cancellationToken);
            var r = JsonSerializer.Deserialize<TranslateResponse>(result,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }          );

            // Log the API call and result
            logger.LogInformation($"API Response: {result}");
            await context.TranslationViewModel.AddAsync(dto.ToEntity(result), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return new TranslationDto(dto.Translator, dto.Language, r!.Contents.Translated);
        }
        catch (Exception ex)
        {
            logger.LogError($"Http Call Exception: {ex.Message}");
            return null;
        }
    }
}