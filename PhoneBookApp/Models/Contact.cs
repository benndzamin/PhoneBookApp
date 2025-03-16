using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PhoneBookApp.Models
{
    public enum Gender
    {
        Male = 1,
        Female = 0
    }

    public class Contact
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string PhoneNumber { get; set; }
        public Gender Gender { get; set; }
        public required string Email { get; set; }
        public DateTime BirthDate { get; set; }

        [NotMapped] // neće se čuvati u bazi
        public int Age => DateTime.Now.Year - BirthDate.Year -
                      (DateTime.Now.DayOfYear < BirthDate.DayOfYear ? 1 : 0); // zaokruživanje godina (provjera rođendana)

        // konvertuje enum u ljudski čitljiv tekst
        [NotMapped] // Ne čuva se u bazi
        public string GenderDisplay => Gender == Gender.Male ? "Male" : "Female";

        public int CityId { get; set; }
        public City? City { get; set; }

        public int CountryId { get; set; }
        public Country? Country { get; set; }

        // formatiranje datuma
        public string FormattedBirthDate
        {
            get
            {
                return BirthDate.ToString("dd. MM. yyyy."); // Formatiraj datum po želji
            }
        }
    }
}
