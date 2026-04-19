using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreExampleProject.Backend.Database.Models;

[Index(nameof(Name), IsUnique = true)]
public class City : Entity
{
    [MaxLength(50)]
    public required string Name { get; set; }

    public ICollection<Person> Persons { get; set; } = [];
}
