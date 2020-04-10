using System;
using Newtonsoft.Json;

namespace TestSnookerApi.Models
{
    public class Event
    {
        [JsonProperty("ID")]
        public int Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("StartDate")]
        public string StartDate { get; set; }

        [JsonProperty("EndDate")]
        public string EndDate { get; set; }

        [JsonProperty("Sponsor")]
        public string Sponsor { get; set; }

        [JsonProperty("Season")]
        public int Season { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("Num")]
        public int Num { get; set; }

        [JsonProperty("Venue")]
        public string Venue { get; set; }

        [JsonProperty("City")]
        public string City { get; set; }

        [JsonProperty("Country")]
        public string Country { get; set; }

        [JsonProperty("Discipline")]
        public string Discipline { get; set; }

        [JsonProperty("Main")]
        public int Main { get; set; }

        [JsonProperty("Sex")]
        public string Sex { get; set; }

        [JsonProperty("AgeGroup")]
        public string AgeGroup { get; set; }

        [JsonProperty("Url")]
        public string Url { get; set; }

        [JsonProperty("Related")]
        public string Related { get; set; }

        [JsonProperty("Stage")]
        public string Stage { get; set; }

        [JsonProperty("ValueType")]
        public string ValueType { get; set; }

        [JsonProperty("ShortName")]
        public string ShortName { get; set; }

        [JsonProperty("WorldSnookerId")]
        public int WorldSnookerId { get; set; }

        [JsonProperty("RankingType")]
        public string RankingType { get; set; }

        [JsonProperty("EventPredictionID")]
        public int EventPredictionId { get; set; }

        [JsonProperty("Team")]
        public bool Team { get; set; }

        [JsonProperty("Format")]
        public int Format { get; set; }

        [JsonProperty("Twitter")]
        public string Twitter { get; set; }

        [JsonProperty("HashTag")]
        public string HashTag { get; set; }

        [JsonProperty("ConversionRate")]
        public double ConversionRate { get; set; }

        [JsonProperty("AllRoundsAdded")]
        public bool AllRoundsAdded { get; set; }

        [JsonProperty("PhotoURLs")]
        public string PhotoUrLs { get; set; }

        [JsonProperty("NumCompetitors")]
        public int NumCompetitors { get; set; }

        [JsonProperty("NumUpcoming")]
        public int NumUpcoming { get; set; }

        [JsonProperty("NumActive")]
        public int NumActive { get; set; }

        [JsonProperty("NumResults")]
        public int NumResults { get; set; }

        [JsonProperty("Note")]
        public string Note { get; set; }

        [JsonProperty("CommonNote")]
        public string CommonNote { get; set; }

        [JsonProperty("DefendingChampion")]
        public int DefendingChampion { get; set; }

        [JsonProperty("PreviousEdition")]
        public int PreviousEdition { get; set; }
    }
}