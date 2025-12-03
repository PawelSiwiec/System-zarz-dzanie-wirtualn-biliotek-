using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LibraryManager
{
    public delegate bool FiltrKsiazki(Ksiazka k);

    public interface IRepozytoriumKsiazek
    {
        Task<List<Ksiazka>> LoadAsync();
        Task SaveAsync(List<Ksiazka> ksiazki);
    }

    public class RepozytoriumJson : IRepozytoriumKsiazek
    {
        private readonly string path;
        private readonly object fileLock = new object();

        public RepozytoriumJson(string path)
        {
            this.path = path;
        }

        public async Task<List<Ksiazka>> LoadAsync()
        {
            if (!File.Exists(path)) return new List<Ksiazka>();
            using var stream = File.OpenRead(path);
            var ks = await JsonSerializer.DeserializeAsync<List<Ksiazka>>(stream) ?? new List<Ksiazka>();
            return ks;
        }

        public async Task SaveAsync(List<Ksiazka> ksiazki)
        {
            var tmp = path + ".tmp";
            using (var s = File.Create(tmp))
            {
                await JsonSerializer.SerializeAsync(s, ksiazki, new JsonSerializerOptions { WriteIndented = true });
            }
            File.Copy(tmp, path, true);
            File.Delete(tmp);
        }
    }

    public abstract class PozycjaBiblioteczna
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Tytul { get; set; } = string.Empty;
    }

    public class Ksiazka : PozycjaBiblioteczna
    {
        public string Autor { get; set; } = string.Empty;
        public int RokWydania { get; set; }
        public bool Wypozyczona { get; set; } = false;
        public int LiczbaWypozyczen { get; set; } = 0;

        public override string ToString()
        {
            return $"[{Id}] {Tytul} — {Autor} ({RokWydania}) {(Wypozyczona ? "[WYPOŻYCZONA]" : "")}";
        }
    }

    public class Osoba
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Imie { get; set; } = string.Empty;
        public string Nazwisko { get; set; } = string.Empty;
    }

    public class Czytelnik : Osoba
    {
        public string Email { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public List<Guid> WypozyczoneKsiazki { get; set; } = new List<Guid>();

        public override string ToString()
        {
            return $"[{Id}] {Imie} {Nazwisko} — {Email} — {Telefon} (wypożyczone: {WypozyczoneKsiazki.Count})";
        }
    }

    public class Biblioteka
    {
        private readonly IRepozytoriumKsiazek repo;
        private readonly object locker = new object();

        public List<Ksiazka> Ksiazki { get; private set; } = new List<Ksiazka>();
        public List<Czytelnik> Czytelnicy { get; private set; } = new List<Czytelnik>();

        public Biblioteka(IRepozytoriumKsiazek repo)
        {
            this.repo = repo;
        }

        public async Task LoadAsync()
        {
            var loaded = await repo.LoadAsync();
            lock (locker)
            {
                Ksiazki = loaded;
            }
        }

        public async Task SaveAsync()
        {

            List<Ksiazka> copy;
            lock (locker)
            {
                copy = Ksiazki.Select(k => k).ToList();
            }
            await repo.SaveAsync(copy);
        }

        public void DodajKsiazke(Ksiazka k)
        {
            lock (locker)
            {
                Ksiazki.Add(k);
            }
        }

        public bool Wypozycz(Guid ksiazkaId, Guid czytelnikId)
        {
            lock (locker)
            {
                var k = Ksiazki.FirstOrDefault(x => x.Id == ksiazkaId);
                var c = Czytelnicy.FirstOrDefault(x => x.Id == czytelnikId);
                if (k == null || c == null || k.Wypozyczona) return false;
                k.Wypozyczona = true;
                k.LiczbaWypozyczen++;
                c.WypozyczoneKsiazki.Add(k.Id);
                return true;
            }
        }

        public bool Zwroc(Guid ksiazkaId, Guid czytelnikId)
        {
            lock (locker)
            {
                var k = Ksiazki.FirstOrDefault(x => x.Id == ksiazkaId);
                var c = Czytelnicy.FirstOrDefault(x => x.Id == czytelnikId);
                if (k == null || c == null || !k.Wypozyczona) return false;
                k.Wypozyczona = false;
                c.WypozyczoneKsiazki.Remove(k.Id);
                return true;
            }
        }

        public IEnumerable<Ksiazka> FiltrujKsiazki(FiltrKsiazki filtr)
        {
            lock (locker)
            {
                return Ksiazki.Where(k => filtr(k)).ToList();
            }
        }

        public IEnumerable<Ksiazka> Szukaj(string fragment)
        {
            fragment = fragment?.ToLower() ?? string.Empty;
            lock (locker)
            {
                return Ksiazki.Where(k => k.Tytul.ToLower().Contains(fragment) || k.Autor.ToLower().Contains(fragment)).ToList();
            }
        }

        public IEnumerable<IGrouping<string, Ksiazka>> GrupujPoAutorze()
        {
            lock (locker)
            {
                return Ksiazki.GroupBy(k => k.Autor).ToList();
            }
        }
    }

    public static class Validator
    {
        private static readonly Regex emailRx = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        private static readonly Regex phoneRx = new Regex(@"^[0-9 \-+]{6,20}$", RegexOptions.Compiled);

        public static bool ValidEmail(string e) => emailRx.IsMatch(e ?? string.Empty);
        public static bool ValidPhone(string p) => phoneRx.IsMatch(p ?? string.Empty);
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Library Manager (konsola) ===");
            var repo = new RepozytoriumJson("library.json");
            var biblioteka = new Biblioteka(repo);

            await biblioteka.LoadAsync();
            if (!biblioteka.Ksiazki.Any()) Seed(biblioteka);

            bool exit = false;
            while (!exit)
            {
                ShowMenu();
                var key = Console.ReadLine();
                switch (key)
                {
                    case "1": DodajKsiazkeInteractive(biblioteka); break;
                    case "2": WyswietlKsiazki(biblioteka); break;
                    case "3": SzukajKsiazkiInteractive(biblioteka); break;
                    case "4": DodajCzytelnikaInteractive(biblioteka); break;
                    case "5": WypozyczInteractive(biblioteka); break;
                    case "6": ZwrocInteractive(biblioteka); break;
                    case "7": Raporty(biblioteka); break;
                    case "9": await biblioteka.SaveAsync(); Console.WriteLine("Zapisano."); break;
                    case "0": exit = true; await biblioteka.SaveAsync(); Console.WriteLine("Wyjście — zapisano zmiany."); break;
                    default: Console.WriteLine("Nieznana opcja."); break;
                }
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine();
            Console.WriteLine("1. Dodaj książkę");
            Console.WriteLine("2. Wyświetl książki");
            Console.WriteLine("3. Szukaj książki (LINQ)");
            Console.WriteLine("4. Dodaj czytelnika");
            Console.WriteLine("5. Wypożycz książkę");
            Console.WriteLine("6. Zwróć książkę");
            Console.WriteLine("7. Raporty (grupowania, statystyki)");
            Console.WriteLine("9. Zapisz teraz");
            Console.WriteLine("0. Wyjdź (zapis)");
            Console.Write("> ");
        }

        static void DodajKsiazkeInteractive(Biblioteka b)
        {
            Console.Write("Tytuł: "); var t = Console.ReadLine();
            Console.Write("Autor: "); var a = Console.ReadLine();
            Console.Write("Rok wydania: "); var rS = Console.ReadLine();
            if (!int.TryParse(rS, out var r)) r = 0;
            var k = new Ksiazka { Tytul = t ?? string.Empty, Autor = a ?? string.Empty, RokWydania = r };
            b.DodajKsiazke(k);
            Console.WriteLine("Dodano: " + k);
        }

        static void WyswietlKsiazki(Biblioteka b)
        {
            var ks = b.Ksiazki.OrderBy(k => k.Tytul).ToList();
            if (!ks.Any()) { Console.WriteLine("Brak książek."); return; }
            foreach (var k in ks) Console.WriteLine(k);
        }

        static void SzukajKsiazkiInteractive(Biblioteka b)
        {
            Console.Write("Fragment tytułu/autora: "); var q = Console.ReadLine();
            var wyn = b.Szukaj(q);
            Console.WriteLine($"Znaleziono: {wyn.Count()} pozycji");
            foreach (var k in wyn) Console.WriteLine(k);

            Console.WriteLine("Przykładowy filtr: książki wydane przed 2000");
            var filtr = new FiltrKsiazki(x => x.RokWydania > 0 && x.RokWydania < 2000);
            var starsze = b.FiltrujKsiazki(filtr);
            foreach (var s in starsze) Console.WriteLine(s);
        }

        static void DodajCzytelnikaInteractive(Biblioteka b)
        {
            Console.Write("Imię: "); var im = Console.ReadLine();
            Console.Write("Nazwisko: "); var naz = Console.ReadLine();
            Console.Write("Email: "); var email = Console.ReadLine();
            Console.Write("Telefon: "); var tel = Console.ReadLine();
            if (!Validator.ValidEmail(email)) Console.WriteLine("Uwaga: nieprawidłowy e-mail.");
            if (!Validator.ValidPhone(tel)) Console.WriteLine("Uwaga: nieprawidłowy numer telefonu.");
            var c = new Czytelnik { Imie = im ?? string.Empty, Nazwisko = naz ?? string.Empty, Email = email ?? string.Empty, Telefon = tel ?? string.Empty };
            b.Czytelnicy.Add(c);
            Console.WriteLine("Dodano czytelnika: " + c);
        }

        static void WypozyczInteractive(Biblioteka b)
        {
            Console.Write("Id książki: "); var kS = Console.ReadLine();
            Console.Write("Id czytelnika: "); var cS = Console.ReadLine();
            if (!Guid.TryParse(kS, out var kid) || !Guid.TryParse(cS, out var cid)) { Console.WriteLine("Niepoprawne ID."); return; }
            var ok = b.Wypozycz(kid, cid);
            Console.WriteLine(ok ? "Wypożyczono." : "Nie udało się wypożyczyć.");
        }

        static void ZwrocInteractive(Biblioteka b)
        {
            Console.Write("Id książki: "); var kS = Console.ReadLine();
            Console.Write("Id czytelnika: "); var cS = Console.ReadLine();
            if (!Guid.TryParse(kS, out var kid) || !Guid.TryParse(cS, out var cid)) { Console.WriteLine("Niepoprawne ID."); return; }
            var ok = b.Zwroc(kid, cid);
            Console.WriteLine(ok ? "Zwrócono." : "Nie udało się zwrócić.");
        }

        static void Raporty(Biblioteka b)
        {
            Console.WriteLine("Najczęściej wypożyczane książki (top 5):");
            var top = b.Ksiazki.OrderByDescending(k => k.LiczbaWypozyczen).Take(5);
            foreach (var t in top) Console.WriteLine($"{t} — wypożeń: {t.LiczbaWypozyczen}");

            Console.WriteLine("Liczba książek według autora:");
            var grupy = b.GrupujPoAutorze();
            foreach (var g in grupy.OrderByDescending(g => g.Count())) Console.WriteLine($"{g.Key}: {g.Count()}");
        }

        static void Seed(Biblioteka b)
        {
            b.DodajKsiazke(new Ksiazka { Tytul = "Lalka", Autor = "Bolesław Prus", RokWydania = 1890 });
            b.DodajKsiazke(new Ksiazka { Tytul = "Pan Tadeusz", Autor = "Adam Mickiewicz", RokWydania = 1834 });
            b.DodajKsiazke(new Ksiazka { Tytul = "Harry Potter i Kamień Filozoficzny", Autor = "J.K. Rowling", RokWydania = 1997 });
            b.Czytelnicy.Add(new Czytelnik { Imie = "Jan", Nazwisko = "Kowalski", Email = "jan.k@example.com", Telefon = "+48 600 000 000" });
        }
    }
}
