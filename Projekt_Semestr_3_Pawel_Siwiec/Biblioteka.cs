namespace LibraryManager;

public class Biblioteka
{
    public List<Ksiazka> Ksiazki { get; } = new();
    public List<Czytelnik> Czytelnicy { get; } = new();

    private int _nextKsiazkaId = 1;
    private int _nextCzytelnikId = 1;

    public void DodajKsiazke(Ksiazka k)
    {
        k.Id = _nextKsiazkaId++;
        Ksiazki.Add(k);
    }

    public void DodajCzytelnika(Czytelnik c)
    {
        c.Id = _nextCzytelnikId++;
        Czytelnicy.Add(c);
    }

    public void OdbudujLiczniki()
    {
        if (Ksiazki.Any())
            _nextKsiazkaId = Ksiazki.Max(k => k.Id) + 1;

        if (Czytelnicy.Any())
            _nextCzytelnikId = Czytelnicy.Max(c => c.Id) + 1;
    }
}
