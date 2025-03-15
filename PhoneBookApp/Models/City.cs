namespace PhoneBookApp.Models
{
    public class City
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? CountryId { get; set; }  //poveznica sa Country
        public Country? Country { get; set; }   //Navigacija ka Country

    }
}
