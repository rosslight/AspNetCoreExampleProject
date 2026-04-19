namespace AspNetCoreExampleProject.Api.Requests.Person;

public class RemoveFriendRequest
{
    public int FirstPersonId { get; set; }

    public int SecondPersonId { get; set; }
}
