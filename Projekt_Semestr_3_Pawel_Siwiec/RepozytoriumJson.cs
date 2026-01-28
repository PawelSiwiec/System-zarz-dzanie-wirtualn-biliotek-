using System.Text.Json;

namespace LibraryManager;

public class RepozytoriumJson<T> : ILoader<List<T>>, ISaver<List<T>>
{
    private readonly string _plik;

    public RepozytoriumJson(string plik)
    {
        _plik = plik;
    }

    public async Task<List<T>> LoadAsync()
    {
        if (!File.Exists(_plik))
            return new List<T>();

        using var stream = File.OpenRead(_plik);
        return await JsonSerializer.DeserializeAsync<List<T>>(stream)
               ?? new List<T>();
    }

    public async Task SaveAsync(List<T> data)
    {
        using var stream = File.Create(_plik);
        await JsonSerializer.SerializeAsync(
            stream,
            data,
            new JsonSerializerOptions { WriteIndented = true }
        );
    }
}
