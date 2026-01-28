namespace LibraryManager;

public class SerwisKsiazek
{
    private readonly Biblioteka _biblioteka;

    public SerwisKsiazek(Biblioteka biblioteka)
    {
        _biblioteka = biblioteka;
    }

    public void Dodaj(Ksiazka ksiazka)
    {
        ksiazka.Id = _biblioteka.Ksiazki.Any()
            ? _biblioteka.Ksiazki.Max(k => k.Id) + 1
            : 1;

        _biblioteka.Ksiazki.Add(ksiazka);
    }
}
