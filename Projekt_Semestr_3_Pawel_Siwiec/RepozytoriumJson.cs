using System.Text.Json;

namespace LibraryManager;

public class RepozytoriumJson :
    ILoader<List<Ksiazka>>,
    ISaver<List<Ksiazka>>
{
    private readonly string _sciezka;

    public RepozytoriumJson(string sciezka)
    {
        _sciezka = sciezka;
    }

    public async Task<List<Ksiazka>> LoadAsync()
    {
        if (!File.Exists(_sciezka))
            return new List<Ksiazka>();

        using var s = File.OpenRead(_sciezka);
        return await JsonSerializer.DeserializeAsync<List<Ksiazka>>(s)
               ?? new List<Ksiazka>();
    }

    public async Task SaveAsync(List<Ksiazka> data)
    {
        using var s = File.Create(_sciezka);
        await JsonSerializer.SerializeAsync(
            s, data,
            new JsonSerializerOptions { WriteIndented = true });
    }
}
