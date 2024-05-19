using AFSGo.Tests.Setup;
using AFSGo.Web.Common;
using AFSGo.Web.Dto;
using AFSGo.Web.Service.Translation;
using JetBrains.Annotations;

namespace AFSGo.Tests.Service.Translation;

[TestSubject(typeof(DummyTranslatorService))]
public class DummyTranslatorServiceTest
{
    [Fact]
    public async Task TranslateAsync_ShouldReturnSameText()
    {
        var context = TestDatabase.Setup();

        var dto = new TranslationDto(
            DummyTranslators.Key,
            DummyTranslators.Get().First(),
            "Hello, World!"
        );
        var service = new DummyTranslatorService(context);

        // Act
        var result = await service.TranslateAsync(dto);

        // Assert
        Assert.Equal(dto, result);
    }

    [Fact]
    public async Task TranslateAsync_ShouldThrowNullException()
    {
        var context = TestDatabase.Setup();
        var service = new DummyTranslatorService(context);

        await Assert.ThrowsAsync<ArgumentNullException>(() => service.TranslateAsync(null!));
    }
}