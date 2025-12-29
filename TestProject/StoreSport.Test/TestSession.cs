using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace SportsStore.Tests;

public class TestSession : ISession
{
    private readonly Dictionary<string, byte[]> sessionData = new();

    public string Id => "test-session-id";
    public bool IsAvailable => true;
    public IEnumerable<string> Keys => this.sessionData.Keys;

    public void Clear() => this.sessionData.Clear();

    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public void Remove(string key) => this.sessionData.Remove(key);

    public void Set(string key, byte[] value) => this.sessionData[key] = value;

    public bool TryGetValue(string key, out byte[] value) => this.sessionData.TryGetValue(key, out value!);

    // Extension methods для работы со строками
    public void SetString(string key, string value)
    {
        this.Set(key, System.Text.Encoding.UTF8.GetBytes(value));
    }

    public string? GetString(string key)
    {
        if (!this.TryGetValue(key, out var value))
        {
            return null;
        }

        return System.Text.Encoding.UTF8.GetString(value);
    }

    // Методы для тестирования
    public void SetJson<T>(string key, T value)
    {
        var json = JsonConvert.SerializeObject(value);
        this.SetString(key, json);
    }

    public T? GetJson<T>(string key)
    {
        var json = this.GetString(key);

        if (string.IsNullOrEmpty(json))
        {
            return default;
        }

        return JsonConvert.DeserializeObject<T>(json);
    }

    // Реализация extension methods для совместимости с SessionExtensions
    public void SetJson(string key, object value)
    {
        this.SetJson<object>(key, value);
    }
}
