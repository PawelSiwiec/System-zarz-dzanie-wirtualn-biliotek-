using LibraryManager;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManager
{
    internal class Program
    {
        static async Task Main()
        {
            
            var repoKsiazki = new RepozytoriumJson<Ksiazka>("BibliotekaKsiazka.json");
            var repoCzytelnicy = new RepozytoriumJson<Czytelnik>("BibliotekaUser.json");

            
            var biblioteka = new Biblioteka();

            biblioteka.Ksiazki.AddRange(await repoKsiazki.LoadAsync());
            biblioteka.Czytelnicy.AddRange(await repoCzytelnicy.LoadAsync());

            biblioteka.OdbudujLiczniki(); 



            var serwisKsiazek = new SerwisKsiazek(biblioteka);
            var serwisWypozyczen = new SerwisWypozyczen(biblioteka);
            var serwisRaportow = new SerwisRaportow(biblioteka);

            bool exit = false;

            while (!exit)
            {
                Menu();
                Console.Write("Wybór: ");
                var wybor = Console.ReadLine();

                switch (wybor)
                {
                    case "1":
                        DodajKsiazke(serwisKsiazek);
                        break;

                    case "2":
                        PokazKsiazki(biblioteka);
                        break;

                    case "3":
                        DodajCzytelnika(biblioteka);
                        break;

                    case "4":
                        PokazCzytelnikow(biblioteka);
                        break;

                    case "5":
                        Wypozycz(serwisWypozyczen);
                        break;

                    case "6":
                        Zwroc(serwisWypozyczen);
                        break;

                    case "7":
                        Raport(serwisRaportow);
                        break;

                    case "8":
                        await repoKsiazki.SaveAsync(biblioteka.Ksiazki);
                        await repoCzytelnicy.SaveAsync(biblioteka.Czytelnicy);
                        Console.WriteLine("✔ Zapisano dane");
                        break;

                    case "0":
                        exit = true;
                        break;
                }
            }
        }

        static void Menu()
        {
            Console.WriteLine("\n--- MENU ---");
            Console.WriteLine("1. Dodaj książkę");
            Console.WriteLine("2. Wyświetl książki");
            Console.WriteLine("3. Dodaj czytelnika");
            Console.WriteLine("4. Wyświetl czytelników");
            Console.WriteLine("5. Wypożycz książkę");
            Console.WriteLine("6. Zwróć książkę");
            Console.WriteLine("7. Raport");
            Console.WriteLine("8. Zapisz dane");
            Console.WriteLine("0. Wyjście");
        }

        static void DodajKsiazke(SerwisKsiazek serwis)
        {
            Console.Write("Tytuł: ");
            var tytul = Console.ReadLine();

            Console.Write("Autor: ");
            var autor = Console.ReadLine();

            Console.Write("Rok wydania: ");
            int.TryParse(Console.ReadLine(), out int rok);

            serwis.Dodaj(new Ksiazka
            {
                Tytul = tytul,
                Autor = autor,
                RokWydania = rok
            });
        }

        static void PokazKsiazki(Biblioteka b)
        {
            foreach (var k in b.Ksiazki)
                Console.WriteLine(k);
        }

        static void DodajCzytelnika(Biblioteka b)
        {
            Console.Write("Imię: ");
            var imie = Console.ReadLine();

            Console.Write("Nazwisko: ");
            var nazwisko = Console.ReadLine();

            Console.Write("Email: ");
            var email = Console.ReadLine();

            Console.Write("Telefon: ");
            var tel = Console.ReadLine();

            b.DodajCzytelnika(new Czytelnik
            {
                Imie = imie,
                Nazwisko = nazwisko,
                Email = email,
                Telefon = tel
            });
        }

        static void PokazCzytelnikow(Biblioteka b)
        {
            foreach (var c in b.Czytelnicy)
                Console.WriteLine(c);
        }

        static void Wypozycz(SerwisWypozyczen s)
        {
            Console.Write("ID książki: ");
            int.TryParse(Console.ReadLine(), out int k);

            Console.Write("ID czytelnika: ");
            int.TryParse(Console.ReadLine(), out int c);

            Console.WriteLine(s.Wypozycz(k, c) ? "✔ Wypożyczono" : "✖ Błąd");
        }

        static void Zwroc(SerwisWypozyczen s)
        {
            Console.Write("ID książki: ");
            int.TryParse(Console.ReadLine(), out int k);

            Console.Write("ID czytelnika: ");
            int.TryParse(Console.ReadLine(), out int c);

            Console.WriteLine(s.Zwroc(k, c) ? "✔ Zwrócono" : "✖ Błąd");
        }

        static void Raport(SerwisRaportow r)
        {
            Console.WriteLine("TOP wypożyczane:");
            foreach (var k in r.TopWypozyczane(5))
                Console.WriteLine($"{k.Tytul} — {k.LiczbaWypozyczen}");
        }
    }
}
