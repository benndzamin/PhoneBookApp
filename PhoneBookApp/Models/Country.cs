namespace PhoneBookApp.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<City>? Cities { get; set; } // = new List<City>(); Ovo osigurava da lista nije null, već je prazna lista, i omogućava dodavanje objekata tipa City bez brige o null referencama.
    }
}
