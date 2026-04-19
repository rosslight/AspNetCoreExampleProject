using Microsoft.EntityFrameworkCore;

namespace AspNetCoreExampleProject.Backend.Database.Models;

[Index(nameof(FirstPersonId), nameof(SecondPersonId), IsUnique = true)]
public class Friendship : Entity
{
    public int FirstPersonId { get; set; }
    public required Person FirstPerson { get; set; }

    public int SecondPersonId { get; set; }
    public required Person SecondPerson { get; set; }
}
