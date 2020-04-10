using Newtonsoft.Json;

namespace TestSnookerApi.Models
{
    public class Player
    {
        [JsonProperty("ID")]
        public int Id { get; set; }

        [JsonProperty("Type")]
        public int Type { get; set; }

        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("MiddleName")]
        public string MiddleName { get; set; }

        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("TeamName")]
        public string TeamName { get; set; }

        [JsonProperty("TeamNumber")]
        public int TeamNumber { get; set; }

        [JsonProperty("TeamSeason")]
        public int TeamSeason { get; set; }

        [JsonProperty("ShortName")]
        public string ShortName { get; set; }

        [JsonProperty("Nationality")]
        public string Nationality { get; set; }

        [JsonProperty("Sex")]
        public string Sex { get; set; }

        [JsonProperty("BioPage")]
        public string BioPage { get; set; }

        [JsonProperty("Born")]
        public string Born { get; set; }

        [JsonProperty("Twitter")]
        public string Twitter { get; set; }

        [JsonProperty("SurnameFirst")]
        public bool SurnameFirst { get; set; }

        [JsonProperty("License")]
        public string License { get; set; }

        [JsonProperty("Club")]
        public string Club { get; set; }

        [JsonProperty("URL")]
        public string Url { get; set; }

        [JsonProperty("Photo")]
        public string Photo { get; set; }

        [JsonProperty("PhotoSource")]
        public string PhotoSource { get; set; }

        [JsonProperty("FirstSeasonAsPro")]
        public int FirstSeasonAsPro { get; set; }

        [JsonProperty("LastSeasonAsPro")]
        public int LastSeasonAsPro { get; set; }

        [JsonProperty("Info")]
        public string Info { get; set; }
    }
}