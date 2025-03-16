using Microsoft.EntityFrameworkCore;
using PhoneBookApp.Models;

namespace PhoneBookApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Contact> Contacts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfiguracija City tabele
            modelBuilder.Entity<City>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(c => c.Country)
                    .WithMany(c => c.Cities)
                    .HasForeignKey(c => c.CountryId)
                    .OnDelete(DeleteBehavior.Cascade); // Ako se država obriše, brišu se i gradovi
            });

            // Konfiguracija Country tabele
            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });


            // Definisanje veza između kontakta i grada
            modelBuilder.Entity<Contact>()
                .HasOne(c => c.City)
                .WithMany()
                .HasForeignKey(c => c.CityId)
                .OnDelete(DeleteBehavior.NoAction);// kontakt ne briše gradove

            // Definisanje veza između kontakta i države
            modelBuilder.Entity<Contact>()
                .HasOne(c => c.Country)
                .WithMany()
                .HasForeignKey(c => c.CountryId)
                .OnDelete(DeleteBehavior.NoAction);// kontakt ne briše države


            //definisanje polja za tabelu Contact
            modelBuilder.Entity<Contact>()
                .Property(c => c.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Contact>()
                .Property(c => c.LastName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Contact>()
                .Property(c => c.PhoneNumber)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Contact>()
                .Property(c => c.Gender)
                .IsRequired();

            modelBuilder.Entity<Contact>()
                .Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Contact>()
                .Property(c => c.BirthDate)
                .IsRequired()
                .HasColumnType("date");

            AddMockData(modelBuilder);

        }
        private void AddMockData(ModelBuilder modelBuilder)
        {
            // Dodavanje mock podataka za države
            modelBuilder.Entity<Country>().HasData(
                new Country { Id = 1, Name = "Bosna i Hercegovina" },
                new Country { Id = 2, Name = "Croatia" },
                new Country { Id = 3, Name = "Serbia" },
                new Country { Id = 4, Name = "Austria" },
                new Country { Id = 5, Name = "Germany" }
            );

            // Dodavanje mock podataka za gradove, povezivanje sa državama putem CountryId
            modelBuilder.Entity<City>().HasData(
                new City { Id = 1, Name = "Sarajevo", CountryId = 1 },
                new City { Id = 2, Name = "Mostar", CountryId = 1 },
                new City { Id = 3, Name = "Tuzla", CountryId = 1 },
                new City { Id = 4, Name = "Zenica", CountryId = 1 },
                new City { Id = 5, Name = "Zagreb", CountryId = 2 },
                new City { Id = 6, Name = "Split", CountryId = 2 },
                new City { Id = 7, Name = "Dubrovnik", CountryId = 2 },
                new City { Id = 8, Name = "Rijeka", CountryId = 2 },
                new City { Id = 9, Name = "Novi Sad", CountryId = 3 },
                new City { Id = 10, Name = "Beograd", CountryId = 3 },
                new City { Id = 11, Name = "Niš", CountryId = 3 },
                new City { Id = 12, Name = "Kragujevac", CountryId = 3 },
                new City { Id = 13, Name = "Wien", CountryId = 4 },
                new City { Id = 14, Name = "Graz", CountryId = 4 },
                new City { Id = 15, Name = "Linz", CountryId = 4 },
                new City { Id = 16, Name = "Salzburg", CountryId = 4 },
                new City { Id = 17, Name = "Berlin", CountryId = 5 },
                new City { Id = 18, Name = "Hamburg", CountryId = 5 },
                new City { Id = 19, Name = "München", CountryId = 5 },
                new City { Id = 20, Name = "Köln", CountryId = 5 }
            );

            // Dodavanje mock podataka za kontakte
            modelBuilder.Entity<Contact>().HasData(
                new Contact { Id = 1, FirstName = "John", LastName = "Doe", PhoneNumber = "123/456-789", Gender = (Gender)1, Email = "johndoe@example.com", BirthDate = DateTime.Parse("1985-05-01"), CityId = 1, CountryId = 1 },
                new Contact { Id = 2, FirstName = "Jane", LastName = "Doe", PhoneNumber = "987/654-321", Gender = (Gender)0, Email = "janedoe@example.com", BirthDate = DateTime.Parse("1990-10-15"), CityId = 5, CountryId = 2 },
                new Contact { Id = 3, FirstName = "Mark", LastName = "Smith", PhoneNumber = "555/555-555", Gender = (Gender)1, Email = "marksmith@example.com", BirthDate = DateTime.Parse("1980-07-20"), CityId = 10, CountryId = 3 },
                new Contact { Id = 4, FirstName = "Ibrahim", LastName = "Hasić", PhoneNumber = "123/456-789", Gender = (Gender)1, Email = "ibrahim@example.com", BirthDate = DateTime.Parse("1985-05-01"), CityId = 16, CountryId = 4 },
                new Contact { Id = 5, FirstName = "Amira", LastName = "Džanić", PhoneNumber = "987/654-321", Gender = (Gender)0, Email = "amira@example.com", BirthDate = DateTime.Parse("1990-10-15"), CityId = 3, CountryId = 1 },
                new Contact { Id = 6, FirstName = "Hasan", LastName = "Selimović", PhoneNumber = "555/555-555", Gender = (Gender)1, Email = "hasan@example.com", BirthDate = DateTime.Parse("1980-07-20"), CityId = 8, CountryId = 2 },
                new Contact { Id = 7, FirstName = "Elma", LastName = "Begović", PhoneNumber = "321/654-987", Gender = (Gender)0, Email = "elma@example.com", BirthDate = DateTime.Parse("1992-01-12"), CityId = 14, CountryId = 4 },
                new Contact { Id = 8, FirstName = "Mirza", LastName = "Kovačević", PhoneNumber = "123/321-456", Gender = (Gender)1, Email = "mirza@example.com", BirthDate = DateTime.Parse("1989-09-18"), CityId = 20, CountryId = 5 },
                new Contact { Id = 9, FirstName = "Maja", LastName = "Avdagić", PhoneNumber = "654/987-321", Gender = (Gender)0, Email = "maja@example.com", BirthDate = DateTime.Parse("1995-11-22"), CityId = 13, CountryId = 4 },
                new Contact { Id = 10, FirstName = "Nedim", LastName = "Salkanović", PhoneNumber = "987/123-654", Gender = (Gender)1, Email = "nedim@example.com", BirthDate = DateTime.Parse("1987-04-07"), CityId = 7, CountryId = 2 }
                );
        }
    }
}
