namespace LibraryManager;

public class Czytelnik : Osoba
{
    public string Email { get; set; } = "";
    public string Telefon { get; set; } = "";
    public List<int> WypozyczoneKsiazki { get; } = new();

    public override string ToString()
    {
        return $"{Id}. {Imie} {Nazwisko} — {Email} (wypożyczone: {WypozyczoneKsiazki.Count})";
    }
}
