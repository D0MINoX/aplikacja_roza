using System.Text.Json;

namespace MauiApp1.Services;

internal sealed class RosaryPartProgress
{
    public int Day { get; set; } = 1;
    public Dictionary<int, HashSet<int>> Completed { get; set; } = new();
    public bool IsComplete(int day, int mystery) =>
        Completed.TryGetValue(day, out var mysteries) && mysteries.Contains(mystery);
}

internal sealed class RosaryProgressStore(string path)
{
    private static readonly SemaphoreSlim Gate = new(1, 1);

    private async Task<Dictionary<string, RosaryPartProgress>> ReadAsync()
    {
        if (!File.Exists(path))
            return new();
        var data = JsonSerializer.Deserialize<Dictionary<string, RosaryPartProgress>>(
            await File.ReadAllTextAsync(path)) ?? throw new InvalidDataException("Nieprawidłowy zapis postępu.");
        if (data.Values.Any(p => p == null || p.Day < 1 || p.Day > 31 || p.Completed == null ||
            p.Completed.Any(d => d.Key < 1 || d.Key > 31 || d.Value == null || d.Value.Any(i => i < 0 || i > 4))))
            throw new InvalidDataException("Nieprawidłowy zapis postępu.");
        return data;
    }

    internal async Task<RosaryPartProgress> LoadAsync(string part)
    {
        await Gate.WaitAsync();
        try
        {
            var data = await ReadAsync();
            return data.GetValueOrDefault(part) ?? new RosaryPartProgress();
        }
        finally { Gate.Release(); }
    }

    internal async Task<RosaryPartProgress> CompleteAsync(string part, int day, int mystery)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(part);
        if (day < 1 || day > 31 || mystery < 0 || mystery > 4)
            throw new ArgumentOutOfRangeException(nameof(day));
        await Gate.WaitAsync();
        try
        {
            var data = await ReadAsync();
            var progress = data.GetValueOrDefault(part) ?? new RosaryPartProgress();
            if (!progress.Completed.TryGetValue(day, out var completed))
                progress.Completed[day] = completed = new();
            completed.Add(mystery);
            while (progress.Day < 31 && progress.Completed.TryGetValue(progress.Day, out var done) && done.Count == 5)
                progress.Day++;
            data[part] = progress;
            // Replace only after the complete new document has been written.
            string temporary = path + ".tmp";
            await File.WriteAllTextAsync(temporary, JsonSerializer.Serialize(data));
            File.Move(temporary, path, overwrite: true);
            return progress;
        }
        finally { Gate.Release(); }
    }
}
