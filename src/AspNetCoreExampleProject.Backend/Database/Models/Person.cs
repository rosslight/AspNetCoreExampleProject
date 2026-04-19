using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreExampleProject.Backend.Database.Models;

[Index(nameof(FirstName))]
[Index(nameof(FirstName))]
public class Person : Entity
{
    [MaxLength(50)]
    public required string FirstName { get; set; }

    [MaxLength(50)]
    public required string LastName { get; set; }

    [Comment("The city the person lives in")]
    public required City City { get; set; }

    [InverseProperty(nameof(Friendship.FirstPerson))]
    public ICollection<Friendship> FriendshipsInitiated { get; set; } = [];

    [InverseProperty(nameof(Friendship.SecondPerson))]
    public ICollection<Friendship> FriendshipsReceived { get; set; } = [];
}
