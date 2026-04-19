namespace AspNetCoreExampleProject.Api.Requests.Person;

public class AddFriendRequest
{
    public int FirstPersonId { get; set; }
    public int SecondPersonId { get; set; }
}
