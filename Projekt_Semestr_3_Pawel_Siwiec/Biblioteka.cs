namespace LibraryManager;

public class Biblioteka
{
    private int _nextBookId = 1;
    private int _nextReaderId = 1;

    public List<Ksiazka> Ksiazki { get; } = new();
    public List<Czytelnik> Czytelnicy { get; } = new();

    public void DodajKsiazke(Ksiazka k)
    {
        k.Id = _nextBookId++;
        Ksiazki.Add(k);
    }

    public void DodajCzytelnika(Czytelnik c)
    {
        c.Id = _nextReaderId++;
        Czytelnicy.Add(c);
    }
}
