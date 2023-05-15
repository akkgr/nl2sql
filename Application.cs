using Azure.AI.OpenAI;

namespace TextToSql;

public record SqlRequest(string Tenant, string Prompt);
public record SqlResponse(string Sql);

public interface IProxy
{
    Task<SqlResponse> SendRequest(SqlRequest data, CancellationToken token);
}

public sealed class OpenAIProxy : IProxy
{
    private readonly OpenAIClient _client;
    private readonly string _model;

    public OpenAIProxy(IConfiguration config)
    {
        _client = new OpenAIClient(config["OpenAI:Key"]);
        _model = config["OpenAI:Model"] ?? string.Empty;
    }

    public async Task<SqlResponse> SendRequest(SqlRequest data, CancellationToken token)
    {
        var completionOptions = new CompletionsOptions
        {
            Prompts = { data.Prompt },
            MaxTokens = 60,
            Temperature = 0.3f,
            FrequencyPenalty = 0.0f,
            PresencePenalty = 0.0f,
            NucleusSamplingFactor = 1
        };

        Completions response = await _client.GetCompletionsAsync(_model, completionOptions, token);
        return new SqlResponse(response.Choices[0].Text);
    }
}