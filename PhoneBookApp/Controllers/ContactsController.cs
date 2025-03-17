using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhoneBookApp.Data;
using PhoneBookApp.Models;

namespace PhoneBookApp.Controllers
{
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: API endpoint za dohvaćanje svih kontakata za prikaz u tabeli s paginacijom
        [HttpGet]
        public async Task<IActionResult> GetContacts(int page = 1, int resultsPerPage = 5, string sortColumn = "FirstName", string sortOrder = "asc")
        {
            // Izračunavanje početka za Skip
            int skip = (page - 1) * resultsPerPage;

            // Kreiraj dinamički izraz za sortiranje
            IQueryable<Contact> query = _context.Contacts.Include(c => c.City).Include(c => c.Country);

            // Dinamičko sortiranje na osnovu proslijeđene kolone i reda
            switch (sortColumn)
            {
                case "FirstName":
                    query = sortOrder == "asc" ? query.OrderBy(c => c.FirstName) : query.OrderByDescending(c => c.FirstName);
                    break;
                case "LastName":
                    query = sortOrder == "asc" ? query.OrderBy(c => c.LastName) : query.OrderByDescending(c => c.LastName);
                    break;
            }

            // Dohvatanje kontakata sa paginacijom
            var contacts = await query
                .Skip(skip)                // Preskoči prethodne stranice
                .Take(resultsPerPage)      // Uzmi samo potrebne rezultate
                .Select(c => new
                {
                    c.Id,
                    c.FirstName,
                    c.LastName,
                    c.PhoneNumber,
                    Gender = c.GenderDisplay,
                    c.Email,
                    BirthDate = c.FormattedBirthDate,
                    c.Age,
                    City = c.City == null ? "N/A" : c.City.Name,
                    Country = c.Country == null ? "N/A" : c.Country.Name
                })
                .ToListAsync();

            // Ukupan broj kontakata za izračunavanje ukupnih stranica
            int totalContacts = await _context.Contacts.CountAsync();

            // Kreiranje rezultata za paginaciju
            var result = new
            {
                items = contacts,
                totalPages = (int)Math.Ceiling((double)totalContacts / resultsPerPage)
            };

            return Json(result);
        }

        // GET: API endpoint za dohvaćanje jednog kontakta prema ID-u
        [HttpGet("/contacts/getContactById/{id}")]
        public async Task<IActionResult> GetContactsById(int id)
        {
            var contact = await _context.Contacts
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            if (contact == null)
            {
                return Json(new { success = false, message = "Contact not found!" });
            }

            // Dohvati sve gradove za odabranu državu
            var cities = await _context.Cities
                .Where(c => c.CountryId == contact.CountryId)
                .Select(c => new
                {
                    c.Id,
                    c.Name
                })
                .ToListAsync();

            return Json(new
            {
                success = true,
                contact = contact,
                cities = cities
            });
        }


        // DELETE: API endpoint za brisanje kontakta
        [HttpDelete("/contacts/delete/{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            // Pronalazi se kontakt sa datim ID-om
            var contact = await _context.Contacts.FindAsync(id);

            // Ako kontakt nije pronađen, vraća se error message
            if (contact == null)
            {
                return Json(new { success = false, message = "Kontakt not found!" });
            }

            // Brisanje kontakta
            _context.Contacts.Remove(contact);

            // Snimanje promjene u bazi
            await _context.SaveChangesAsync();

            // Vraća uspješan odgovor
            return Json(new { success = true, message = "Contact deleted successfully!" });
        }


        // POST: API endpoint sa snimanje u bazu /Contacts/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Contact contact)
        {
            if (contact == null)
            {
                return Json(new { success = false, message = "Invalid contact data!" });
            }
            if (ModelState.IsValid)
            {
                // Dodaje se kontakt u bazu
                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();

                // Vraća se uspješan odgovor
                return Json(new { success = true, message = "Contact saved successfully!" });
            }

            // Ako podaci nisu ispravni, vraća  grešku
            return Json(new { success = false });
        }


        // PUT: API endpoint za edit kontakta
        [HttpPut("/contacts/update/{id}")]
        public async Task<IActionResult> UpdateContact(int id, [FromBody] Contact contactData)
        {
            if (contactData == null || id != contactData.Id)
            {
                return Json(new { success = false, message = "Invalid user data!" });
            }

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return Json(new { success = false, message = "Contact not found!" });
            }

            // edit podataka kontakta
            contact.FirstName = contactData.FirstName;
            contact.LastName = contactData.LastName;
            contact.PhoneNumber = contactData.PhoneNumber;
            contact.Email = contactData.Email;
            contact.BirthDate = contactData.BirthDate;
            contact.Gender = contactData.Gender;
            contact.CityId = contactData.CityId;
            contact.CountryId = contactData.CountryId;

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Contact edited successfully!" });
        }


        //Endpoint za dodavanje svih država
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _context.Countries
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();

            return Json(countries);
        }

        //Endpoint za dodavanje gradova po ID-ju države
        public async Task<IActionResult> GetCitiesByCountry(int countryId)
        {
            if (countryId == 0)
            {
                return Json(new List<SelectListItem>());
            }

            var cities = await _context.Cities
                .Where(c => c.CountryId == countryId)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();

            return Json(cities);
        }

    }
}


