using Microsoft.AspNetCore.Mvc;

namespace AFSGo.Api.Controllers;

// {
//     "success": {
//         "total": 1
//     },
//     "contents": {
//         "translation": "ermahgerd",
//         "text": "Oh my god! This is giving me goosebumps!",
//         "translated": "<translated text>"
//     }
// }
public record TranslateSuccess(int Total);

public record Contents(string Translation, string Text, string Translated);

public record TranslateResponse(TranslateSuccess Success, Contents Contents);

public record TranslateRequest(string Text);

[ApiController]
[Route("[controller]")]
public class TranslateController(ILogger<TranslateController> logger) : ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    private readonly ILogger<TranslateController> _logger = logger;

    [HttpPost("{translator}.json")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<TranslateResponse> Get([FromRoute] string translator, [FromForm] TranslateRequest req)
    {
        var translatedText = Summaries[Random.Shared.Next(Summaries.Length)];
        var response = new TranslateResponse(
            new TranslateSuccess(1),
            new Contents(translator, req.Text, translatedText)
        );

        return Task.FromResult(response);
    }
}