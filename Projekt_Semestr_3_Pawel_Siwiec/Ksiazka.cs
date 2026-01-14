namespace LibraryManager;

public class Ksiazka : PozycjaBiblioteczna
{
    public string Autor { get; set; } = "";
    public int RokWydania { get; set; }
    public bool Wypozyczona { get; set; }
    public int LiczbaWypozyczen { get; set; }

    public override string ToString()
    {
        return $"{Id}. {Tytul} — {Autor} ({RokWydania})" +
               (Wypozyczona ? " [WYPOŻYCZONA]" : "");
    }
}
