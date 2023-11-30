using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace Entities;

public class PersonsDbContext : DbContext
{
    public PersonsDbContext(DbContextOptions<PersonsDbContext> options) : base(options)
    {
    }

    public DbSet<Country> Countries { get; set; }
    public DbSet<Person> Persons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Country>().ToTable("Countries");
        modelBuilder.Entity<Person>().ToTable("Persons");

        // Seed to Countries
        var countriesJson = File.ReadAllText("countries.json");
        var countries = JsonSerializer.Deserialize<List<Country>>(countriesJson);

        if (countries != null)
            foreach (var country in countries)
                modelBuilder.Entity<Country>().HasData(country);

        // Seed to Persons
        var personsJson = File.ReadAllText("persons.json");
        var persons = JsonSerializer.Deserialize<List<Person>>(personsJson);
        if (persons != null)
            foreach (var person in persons)
                modelBuilder.Entity<Person>().HasData(person);

        // Fluent API
        modelBuilder.Entity<Person>().Property(temp => temp.Tin)
            .HasColumnName("TaxIdentificationNumber")
            .HasColumnType("varchar(8)")
            .HasDefaultValue("ABC12345");

        // modelBuilder.Entity<Person>().HasIndex(temp => temp.Tin).IsUnique();

        modelBuilder.Entity<Person>().ToTable(table =>
            table.HasCheckConstraint("CHK_TIN", "length(\"TaxIdentificationNumber\") = 8"));

        // Table relations
        //  modelBuilder.Entity<Person>(entity =>
        //  {
        //      entity.HasOne<Country>(c => c.Country)
        //          .WithMany(p => p.Persons)
        //          .HasForeignKey(p => p.CountryId);
        //  });
    }

    public IEnumerable<Person> sp_GetAllPersons()
    {
        return Persons.FromSqlRaw("SELECT * FROM get_all_persons()").ToList();
    }

    public int sp_InsertPerson(Person person)
    {
        NpgsqlParameter[] parameters =
        {
            new("@PersonId", NpgsqlDbType.Uuid) { Value = person.PersonId },
            new("@PersonName", NpgsqlDbType.Varchar) { Value = person.PersonName },
            new("@Email", NpgsqlDbType.Varchar) { Value = person.Email },
            new("@DateOfBirth", NpgsqlDbType.Date) { Value = person.DateOfBirth ?? (object)DBNull.Value },
            new("@Gender", NpgsqlDbType.Varchar) { Value = person.Gender },
            new("@CountryId", NpgsqlDbType.Uuid) { Value = person.CountryId },
            new("@Address", NpgsqlDbType.Varchar) { Value = person.Address },
            new("@ReceiveNewsLetters", NpgsqlDbType.Boolean) { Value = person.ReceiveNewsLetters }
        };

        return Database.ExecuteSqlRaw(
            "SELECT insert_person(@PersonId, @PersonName, @Email, @DateOfBirth, @Gender, @CountryId, @Address, @ReceiveNewsLetters)",
            parameters);
    }
}