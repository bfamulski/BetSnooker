using Newtonsoft.Json;

namespace TestSnookerApi.Models
{
    public class RoundInfo
    {
        [JsonProperty("Round")]
        public int Round { get; set; }

        [JsonProperty("RoundName")]
        public string RoundName { get; set; }

        [JsonProperty("EventID")]
        public int EventId { get; set; }

        [JsonProperty("MainEvent")]
        public int MainEvent { get; set; }

        [JsonProperty("Distance")]
        public int Distance { get; set; }

        [JsonProperty("NumLeft")]
        public int NumLeft { get; set; }

        [JsonProperty("NumMatches")]
        public int NumMatches { get; set; }

        [JsonProperty("Note")]
        public string Note { get; set; }

        [JsonProperty("ValueType")]
        public string ValueType { get; set; }

        [JsonProperty("Rank")]
        public int Rank { get; set; }

        [JsonProperty("Money")]
        public int Money { get; set; }

        [JsonProperty("SeedGetsHalf")]
        public int SeedGetsHalf { get; set; }

        [JsonProperty("ActualMoney")]
        public int ActualMoney { get; set; }

        [JsonProperty("Currency")]
        public string Currency { get; set; }

        [JsonProperty("ConversionRate")]
        public double ConversionRate { get; set; }

        [JsonProperty("Points")]
        public int Points { get; set; }

        [JsonProperty("SeedPoints")]
        public int SeedPoints { get; set; }
    }
}