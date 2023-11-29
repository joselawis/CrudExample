using System.Text.Json;
using Microsoft.EntityFrameworkCore;

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
    }

    public IEnumerable<Person> sp_GetAllPersons()
    {
        return Persons.FromSqlRaw("SELECT * FROM get_all_persons()").ToList();
    }
}