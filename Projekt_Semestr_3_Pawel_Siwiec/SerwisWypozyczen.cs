namespace LibraryManager;

public class SerwisWypozyczen
{
    private readonly Biblioteka _biblioteka;

    public SerwisWypozyczen(Biblioteka biblioteka)
    {
        _biblioteka = biblioteka;
    }

    public bool Wypozycz(int ksiazkaId, int czytelnikId)
    {
        var k = _biblioteka.Ksiazki.FirstOrDefault(x => x.Id == ksiazkaId);
        var c = _biblioteka.Czytelnicy.FirstOrDefault(x => x.Id == czytelnikId);

        if (k == null || c == null || k.Wypozyczona)
            return false;

        k.Wypozyczona = true;
        k.LiczbaWypozyczen++;
        c.WypozyczoneKsiazki.Add(k.Id);
        return true;
    }

    public bool Zwroc(int ksiazkaId, int czytelnikId)
    {
        var k = _biblioteka.Ksiazki.FirstOrDefault(x => x.Id == ksiazkaId);
        var c = _biblioteka.Czytelnicy.FirstOrDefault(x => x.Id == czytelnikId);

        if (k == null || c == null)
            return false;

        k.Wypozyczona = false;
        c.WypozyczoneKsiazki.Remove(k.Id);
        return true;
    }
}
