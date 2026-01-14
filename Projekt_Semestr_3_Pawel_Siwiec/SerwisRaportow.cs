namespace LibraryManager;

public class SerwisRaportow
{
    private readonly Biblioteka _biblioteka;

    public SerwisRaportow(Biblioteka biblioteka)
    {
        _biblioteka = biblioteka;
    }

    public IEnumerable<Ksiazka> Top(int ile)
    {
        return _biblioteka.Ksiazki
            .OrderByDescending(k => k.LiczbaWypozyczen)
            .Take(ile);
    }
}
