# CLOTHERS

## Opis projektu

Clothers to internetowy sklep z ubraniami. Aplikacja jest zbudowana przy użyciu technologii - ASP.NET Core MVC oraz Entity Framework Core.

## Funkcjonalności

- Przeglądanie ubrań dodanych
- Kupowanie tych ubrań
- Tworzenie nowych ofert ubrań przez użytkownika z rolą "Firma"
- Usuwanie ofert z ubraniami (dla administratorów)
- Przeglądanie własnych ofert (dla firm)
- Finalizacja swojego koszyka oraz możliwość pobrania potwierdzenia zamówienia w pliku PDF (dla zwykłego zalogowanego użytkownika)
- Przeglądanie zamówionych ubrań przez tą firme, której obrania zostały zamówione (dla firm)
- Akcpetowanie ofert, które wylądują na stronie głównej (dla administratora)
- Edytowanie, usuwanie oraz dodawanie użytkowników (dla administratora)

## Technologie

- ASP.NET Core MVC
- Entity Framework Core
- Microsoft Identity
- MS SQL
- QuestPDF

## Instalacja

1. Sklonuj repozytorium:
https://github.com/winkielmikolaj/Clothers.git
2. Skonfiguruj swoje połączenie w `appsettings.json`
3. Zaktualizuj bazę danych:
database-update 
4. Uruchom aplikację:
Dostępni seedowani użytkownicy:
- Rola - Admin | Login - admin@clothers.com Passw - Admin123!
- Rola - SiteUser | Login - user@clothers.com Passw - User123!
- Rola - Company | Login - company@clothers.com Passw - Company123!

## Konfiguracja

### Plik `appsettings.json`

Upewnij się, że plik `appsettings.json` zawiera poprawne ustawienia połączenia z bazą danych:

Domyślne połączenie jest ustawione na lokalną bazę danych wbudowaną w Visual Studio 2022

## Przykładowe scenariusze użycia

### Tworzenie nowej oferty

1. Zaloguj się jako firma.
2. Login - company@clothers.com Passw - Clothers123!
3. Przejdź do sekcji "Nowa oferta".
4. Wypełnij formularz i kliknij "Dodaj".

### Akcpetacja ofert przez Administratora

1. Zaloguj się jako administrator.
2. Login - admin@clothers.com Passw - Admin123!
3. Jeśli wszystko zgadza się z ofertą, kliknij zaakcpeptuj.

### Kupowanie ubrań przez użytkownika

1. Zaloguj się jako użytkownik.
2. Login - user@clothers.com Passw - User123!
3. Wybierz ilość jaką chcesz dodać do koszyka danego ubrania.
4. Przejdź do koszyka.
5. Kliknij "Zrealizuj Zamówienie".
6. Wypełnij formularz potrzebny do wysyłki.
7. Sprawdź PDF-a z potwierdzeniem zamówienia.
   
## Autorzy

https://github.com/winkielmikolaj
