namespace BetSnooker.Models.API
{
    public class Player
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public override string ToString() => FirstName == "TBD" ? string.Empty : $"{FirstName} {LastName}";
    }
}