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
                new Country { Id = 2, Name = "Hrvatska" },
                new Country { Id = 3, Name = "Srbija" }
            );

            // Dodavanje mock podataka za gradove, povezivanje sa državama putem CountryId
            modelBuilder.Entity<City>().HasData(
                new City { Id = 1, Name = "Sarajevo", CountryId = 1 },
                new City { Id = 2, Name = "Mostar", CountryId = 1 },
                new City { Id = 3, Name = "Tuzla", CountryId = 1 },
                new City { Id = 4, Name = "Zenica", CountryId = 1 },
                new City { Id = 5, Name = "Zagreb", CountryId = 2 },
                new City { Id = 6, Name = "Split", CountryId = 2 },
                new City { Id = 8, Name = "Dubrovnik", CountryId = 2 },
                new City { Id = 9, Name = "Rijeka", CountryId = 2 },
                new City { Id = 10, Name = "Novi Sad", CountryId = 3 },
                new City { Id = 11, Name = "Beograd", CountryId = 3 },
                new City { Id = 12, Name = "Niš", CountryId = 3 }
            );

            // Dodavanje mock podataka za kontakte
            modelBuilder.Entity<Contact>().HasData(
                new Contact { Id = 1, FirstName = "John", LastName = "Doe", PhoneNumber = "123/456-789", Gender = (Gender)1, Email = "johndoe@example.com", BirthDate = DateTime.Parse("1985-05-01"), CityId = 1, CountryId = 1 },
                new Contact { Id = 2, FirstName = "Jane", LastName = "Doe", PhoneNumber = "987/654-321", Gender = (Gender)0, Email = "janedoe@example.com", BirthDate = DateTime.Parse("1990-10-15"), CityId = 5, CountryId = 2 },
                new Contact { Id = 3, FirstName = "Mark", LastName = "Smith", PhoneNumber = "555/555-555", Gender = (Gender)1, Email = "marksmith@example.com", BirthDate = DateTime.Parse("1980-07-20"), CityId = 10, CountryId = 3 }
            );
        }


    }
}
