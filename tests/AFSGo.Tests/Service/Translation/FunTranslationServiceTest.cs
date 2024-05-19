using AFSGo.Web.Service.Translation;
using System.Net;
using AFSGo.Tests.Setup;
using AFSGo.Web.Data;
using AFSGo.Web.Dto;
using AFSGo.Web.Service.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;

namespace AFSGo.Tests.Service.Translation;

public class FunTranslationServiceTest
{
    private readonly Mock<ILogger<FunTranslationService>> _loggerMock;
    private readonly Mock<IOptions<FunTranslatorSettings>> _settingsMock;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;

    private readonly FunTranslatorSettings _settings;

    public FunTranslationServiceTest()
    {
        _loggerMock = new Mock<ILogger<FunTranslationService>>();
        _settingsMock = new Mock<IOptions<FunTranslatorSettings>>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();

        _settings = new FunTranslatorSettings { BaseUrl = "http://localhost:5000", ApiKey = "SECRET_KEY" };
        _settingsMock.Setup(s => s.Value).Returns(_settings);
    }

    [Fact]
    public async Task TranslateAsync_ValidRequest_ReturnsTranslation()
    {
        // Arrange
        var context = TestDatabase.Setup();
        var dto = new TranslationDto("FunTranslator", "leetspeak", "Hello there");

        var httpClient = new MockHttpMessageHandler();
        httpClient
            .When(HttpMethod.Post, $"{_settings.BaseUrl}/translate/{dto.Language}.json")
            .WithFormData("text", dto.Text)
            .WithHeaders("X-Funtranslations-Api-Secret", _settings.ApiKey)
            .Respond(HttpStatusCode.OK, System.Net.Mime.MediaTypeNames.Application.Json,
                "{ \"contents\": { \"translated\": \"Translated text\" } }");

        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient.ToHttpClient());


        var service = new FunTranslationService(
            _loggerMock.Object,
            _settingsMock.Object,
            context,
            _httpClientFactoryMock.Object
        );

        // Act
        var result = await service.TranslateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Translated text", result.Text);
    }

    [Fact]
    public async Task TranslateAsync_InvalidTranslator_ReturnsNull()
    {
        // Arrange
        var dto = new TranslationDto("invalid", "invalid", "Hello there");

        var context = TestDatabase.Setup();

        var service = new FunTranslationService(
            _loggerMock.Object,
            _settingsMock.Object,
            context,
            _httpClientFactoryMock.Object
        );

        // Act
        var result = await service.TranslateAsync(dto);

        // Assert
        Assert.Null(result);
        _loggerMock.Verify(
            l =>
                l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains($"Invalid Translator: {dto.Translator}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task TranslateAsync_InvalidLanguage_ReturnsNull()
    {
        // Arrange
        var dto = new TranslationDto("FunTranslator", "invalid", "Hello there");

        var context = TestDatabase.Setup();

        var service = new FunTranslationService(
            _loggerMock.Object,
            _settingsMock.Object,
            context,
            _httpClientFactoryMock.Object
        );

        // Act
        var result = await service.TranslateAsync(dto);

        // Assert
        Assert.Null(result);
        _loggerMock.Verify(
            l =>
                l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains($"Not supported language: {dto.Language}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
            Times.Once
        );
    }


    [Fact]
    public async Task TranslateAsync_ApiCallFails_ReturnsNull()
    {
        // Arrange
        var context = TestDatabase.Setup();
        var dto = new TranslationDto("FunTranslator", "leetspeak", "Hello there");

        var httpClient = new MockHttpMessageHandler();
        httpClient
            .When(HttpMethod.Post, $"{_settings.BaseUrl}/translate/{dto.Language}.json")
            .WithFormData("text", dto.Text)
            .WithHeaders("X-Funtranslations-Api-Secret", _settings.ApiKey)
            .Respond(HttpStatusCode.Unauthorized, System.Net.Mime.MediaTypeNames.Application.Json,
                "{\n  \"error\": {\n    \"code\": 401,\n    \"message\": \"Invalid API key.\"\n  }\n}\n");

        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient.ToHttpClient());


        var service = new FunTranslationService(
            _loggerMock.Object,
            _settingsMock.Object,
            context,
            _httpClientFactoryMock.Object
        );

        // Act
        var result = await service.TranslateAsync(dto);

        // Assert
        Assert.Null(result);
        _loggerMock.Verify(
            l =>
                l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("API Call Failed")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task TranslateAsync_HttpCallThrowError_ReturnsNull()
    {
        // Arrange
        var context = TestDatabase.Setup();
        var dto = new TranslationDto("FunTranslator", "leetspeak", "Hello there");

        var httpClient = new MockHttpMessageHandler();
        httpClient
            .When(HttpMethod.Post, $"{_settings.BaseUrl}/translate/{dto.Language}.json")
            .WithFormData("text", dto.Text)
            .WithHeaders("X-Funtranslations-Api-Secret", _settings.ApiKey)
            .Throw(new Exception("Error"));

        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient.ToHttpClient());


        var service = new FunTranslationService(
            _loggerMock.Object,
            _settingsMock.Object,
            context,
            _httpClientFactoryMock.Object
        );

        // Act
        var result = await service.TranslateAsync(dto);

        // Assert
        Assert.Null(result);
        _loggerMock.Verify(
            l =>
                l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Http Call Exception:")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task TranslateAsync_NullDto_ThrowsArgumentNullException()
    {
        // Arrange
        var context = TestDatabase.Setup();
        var service = new FunTranslationService(
            _loggerMock.Object,
            _settingsMock.Object,
            context,
            _httpClientFactoryMock.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => service.TranslateAsync(null!));
    }
}