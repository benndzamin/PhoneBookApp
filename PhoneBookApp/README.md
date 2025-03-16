# PhoneBookApp

Aplikacija za upravljanje kontaktima koja omogućava korisnicima da dodaju, uređuju i brišu kontakte.
Aplikacija koristi ASP.NET Core, Entity Framework Core, Linq, Microsoft SQL Server i front-end JavaScript, HTML, CSS, jQuery i Bootstrap.

## Funkcionalnosti

- **Dodavanje kontakata**: Korisnici mogu dodati nove kontakte sa osnovnim podacima kao što su ime, prezime, broj telefona, email, datum rođenja, i lokacija (grad i država).
- **Editovanje kontakata**: Korisnici mogu ažurirati informacije o kontaktu.
- **Brisanje kontakata**: Korisnici mogu obrisati kontakte sa liste.
- **Dropdown za gradove i države**: Gradovi se dinamički učitavaju na osnovu odabrane države.
- **Responsive dizajn**: Aplikacija je dizajnirana da bude responsivna i korisnička prijatna na različitim uređajima.

## Tehnologije koje se koriste

- **Frontend**: HTML, CSS, Bootstrap, JavaScript, jQuery
- **Backend**: ASP.NET Core (C#)
- **Baza podataka**: Microsoft SQL Server
- **ORM**: Entity Framework Core
- **AJAX**: Za dinamičko učitavanje podataka i slanje formi bez ponovnog učitavanja stranice.

## Instalacija

1. Klonirajte repozitorijum na svoj lokalni računar.

    ```bash
    git clone https://github.com/yourusername/contact-list-app.git
    cd contact-list-app
    ```

2. Restorujte NuGet pakete za projekat:

    ```bash
    dotnet restore
    ```

4. U fajlu appsettings.json postavite konekciju na svoju bazu podataka - lokalni server.

    "ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=PhoneBookDB;Trusted_Connection=True;TrustServerCertificate=True;"
}

4. Kreirajte i primenite migracije baze podataka:

    ```bash
    dotnet ef migrations add InitialCreate
    dotnet ef database update
    ```

5. Pokrenite aplikaciju:

    ```bash
    dotnet run
    ```

## Korištenje aplikacije

Nakon što pokrenete aplikaciju, moći ćete da:

- Pogledate listu kontakata.
- Dodate novi kontakt koristeći ponuđeni obrazac.
- Uredite i obrišete postojeće kontakte.

### API Endpoints

Aplikacija ima sledeće API endpoint-e:

- **GET /contacts/getContacts**: Dohvati sve kontakte.
- **GET /contacts/getCountries**: Dohvati sve sve države.
- **GET /Contacts/GetCitiesByCountry**: Dohvati sve sve gradove po izabranoj državi.
- **GET /contacts/getContactById/{id}**: Dohvati specifičan kontakt po ID-u.
- **POST /contacts/create**: Kreiraj novi kontakt.
- **PUT /contacts/update/{id}**: Ažuriraj informacije o kontaktu.
- **DELETE /contacts/delete/{id}**: Obrisi kontakt.

### Frontend

- Aplikacija koristi **AJAX** za dinamičko učitavanje gradova na osnovu odabrane države.
- **Dropdown za gradove** se ažurira kada korisnik izabere državu iz liste.
