namespace LibraryManager;

public class SerwisKsiazek
{
    private readonly Biblioteka _biblioteka;

    public SerwisKsiazek(Biblioteka biblioteka)
    {
        _biblioteka = biblioteka;
    }

    public void Dodaj(Ksiazka k)
    {
        _biblioteka.DodajKsiazke(k);
    }

    public IEnumerable<Ksiazka> Szukaj(string tekst)
    {
        tekst = (tekst ?? "").ToLower();

        return _biblioteka.Ksiazki
            .Where(k =>
                k.Tytul.ToLower().Contains(tekst) ||
                k.Autor.ToLower().Contains(tekst));
    }
}
