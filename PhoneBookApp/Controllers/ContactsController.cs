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

        // GET: API endpoint za dohvaćanje svih kontakata za prikaz u tabeli
        [HttpGet]
        public async Task<IActionResult> GetContacts()
        {
            var contacts = await _context.Contacts
                .Include(c => c.City)
                .Include(c => c.Country)
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

            return Json(contacts);
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


