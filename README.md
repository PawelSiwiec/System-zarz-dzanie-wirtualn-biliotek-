Library Manager — README

Cel projektu

Celem projektu Library Manager jest stworzenie prostego, konsolowego
systemu zarządzania biblioteką, który umożliwia: - dodawanie książek i
czytelników, - wyszukiwanie i filtrowanie zasobów, - wypożyczanie i
zwracanie książek, - generowanie podstawowych raportów, - zapisywanie
stanu biblioteki w pliku JSON.

Program demonstruje użycie: - programowania obiektowego (klasy,
dziedziczenie), - delegatów, - kolekcji i LINQ, - asynchronicznego
zapisu/odczytu danych, - walidacji danych przy pomocy wyrażeń
regularnych.

Struktura projektu

1. Repozytorium danych (RepozytoriumJson)

Odpowiada za wczytywanie i zapisywanie listy książek do pliku JSON.
Zapis odbywa się atomowo, przy użyciu pliku tymczasowego.

2. Modele danych

-   PozycjaBiblioteczna – klasa bazowa.
-   Ksiazka – informacje o książce, jej stanie i liczbie wypożyczeń.
-   Osoba oraz Czytelnik – reprezentacje użytkowników biblioteki.

3. Klasa Biblioteka

Zawiera logikę biznesową: - dodawanie książek i czytelników, -
wypożyczanie i zwracanie, - wyszukiwanie i filtrowanie, - grupowanie
książek po autorach, - raportowanie.

4. Interfejs konsolowy

Menu pozwala użytkownikowi wykonywać wszystkie operacje systemu, a także
zapisywać zmiany oraz przeglądać raporty.

Najważniejsze funkcje

-   Wyszukiwanie książek po fragmencie tytułu lub autora.
-   Przykładowy delegat filtrujący – np. książki starsze niż rok 2000.
-   Raporty: najczęściej wypożyczane książki, grupowanie według autorów.
-   Walidacja e-mail i numeru telefonu.

Zapisywanie danych

Dane przechowywane są w pliku JSON (library.json) w sposób bezpieczny i
atomowy.

Uruchamianie programu

1.  Utwórz projekt: dotnet new console -n LibraryManager
2.  Zastąp zawartość Program.cs kodem źródłowym.
3.  Uruchom program: dotnet run

Możliwe rozszerzenia

-   zapis i odczyt listy czytelników,
-   interfejs webowy (np. ASP.NET Core),
-   obsługa terminów zwrotu,
-   integracja z bazą danych,
-   eksport raportów do PDF lub CSV.
