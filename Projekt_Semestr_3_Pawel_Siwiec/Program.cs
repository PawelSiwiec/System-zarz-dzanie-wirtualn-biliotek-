using LibraryManager;

class Program
{
    static async Task Main()
    {
        var repo = new RepozytoriumJson("library.json");
        var biblioteka = new Biblioteka();

        foreach (var k in await repo.LoadAsync())
            biblioteka.DodajKsiazke(k);

        if (!biblioteka.Ksiazki.Any())
            Seed(biblioteka);

        var ksiazki = new SerwisKsiazek(biblioteka);
        var wypozyczenia = new SerwisWypozyczen(biblioteka);
        var raporty = new SerwisRaportow(biblioteka);

        while (true)
        {
            Menu();
            var op = Console.ReadLine();

            switch (op)
            {
                case "1": DodajKsiazke(ksiazki); break;
                case "2": PokazKsiazki(biblioteka); break;
                case "3": Szukaj(ksiazki); break;
                case "4": DodajCzytelnika(biblioteka); break;
                case "5": Wypozycz(wypozyczenia); break;
                case "6": Zwroc(wypozyczenia); break;
                case "7": Raport(raporty); break;
                case "0":
                    await repo.SaveAsync(biblioteka.Ksiazki);
                    return;
            }
        }
    }

    static void Menu()
    {
        Console.WriteLine("\n1. Dodaj książkę");
        Console.WriteLine("2. Wyświetl książki");
        Console.WriteLine("3. Szukaj");
        Console.WriteLine("4. Dodaj czytelnika");
        Console.WriteLine("5. Wypożycz");
        Console.WriteLine("6. Zwróć");
        Console.WriteLine("7. Raport");
        Console.WriteLine("0. Wyjście");
        Console.Write("> ");
    }

    static void DodajKsiazke(SerwisKsiazek s)
    {
        Console.Write("Tytuł: ");
        var t = Console.ReadLine() ?? "";

        Console.Write("Autor: ");
        var a = Console.ReadLine() ?? "";

        Console.Write("Rok: ");
        int.TryParse(Console.ReadLine(), out int r);

        s.Dodaj(new Ksiazka { Tytul = t, Autor = a, RokWydania = r });
    }

    static void PokazKsiazki(Biblioteka b)
    {
        foreach (var k in b.Ksiazki)
            Console.WriteLine(k);
    }

    static void Szukaj(SerwisKsiazek s)
    {
        Console.Write("Szukaj: ");
        foreach (var k in s.Szukaj(Console.ReadLine()))
            Console.WriteLine(k);
    }

    static void DodajCzytelnika(Biblioteka b)
    {
        Console.Write("Imię: ");
        var im = Console.ReadLine() ?? "";

        Console.Write("Nazwisko: ");
        var na = Console.ReadLine() ?? "";

        Console.Write("Email: ");
        var em = Console.ReadLine() ?? "";

        Console.Write("Telefon: ");
        var tel = Console.ReadLine() ?? "";

        b.DodajCzytelnika(new Czytelnik
        {
            Imie = im,
            Nazwisko = na,
            Email = em,
            Telefon = tel
        });
    }

    static void Wypozycz(SerwisWypozyczen s)
    {
        Console.Write("ID książki: ");
        int.TryParse(Console.ReadLine(), out int k);

        Console.Write("ID czytelnika: ");
        int.TryParse(Console.ReadLine(), out int c);

        Console.WriteLine(s.Wypozycz(k, c) ? "OK" : "Błąd");
    }

    static void Zwroc(SerwisWypozyczen s)
    {
        Console.Write("ID książki: ");
        int.TryParse(Console.ReadLine(), out int k);

        Console.Write("ID czytelnika: ");
        int.TryParse(Console.ReadLine(), out int c);

        Console.WriteLine(s.Zwroc(k, c) ? "OK" : "Błąd");
    }

    static void Raport(SerwisRaportow r)
    {
        foreach (var k in r.Top(5))
            Console.WriteLine($"{k} — {k.LiczbaWypozyczen}");
    }

    static void Seed(Biblioteka b)
    {
        b.DodajKsiazke(new Ksiazka
        {
            Tytul = "Lalka",
            Autor = "Bolesław Prus",
            RokWydania = 1890
        });

        b.DodajCzytelnika(new Czytelnik
        {
            Imie = "Jan",
            Nazwisko = "Kowalski",
            Email = "jan.k@example.com",
            Telefon = "600000000"
        });
    }
}