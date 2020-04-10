using System;
using Newtonsoft.Json;

namespace TestSnookerApi.Models
{
    public class Match
    {
        [JsonProperty("ID")]
        public int Id { get; set; }

        [JsonProperty("EventID")]
        public int EventId { get; set; }

        [JsonProperty("Round")]
        public int Round { get; set; }

        [JsonProperty("Number")]
        public int Number { get; set; }

        [JsonProperty("Player1ID")]
        public int Player1Id { get; set; }

        [JsonProperty("Score1")]
        public int Score1 { get; set; }

        [JsonProperty("Walkover1")]
        public bool Walkover1 { get; set; }

        [JsonProperty("Player2ID")]
        public int Player2Id { get; set; }

        [JsonProperty("Score2")]
        public int Score2 { get; set; }

        [JsonProperty("Walkover2")]
        public bool Walkover2 { get; set; }

        [JsonProperty("WinnerID")]
        public int WinnerId { get; set; }

        [JsonProperty("Unfinished")]
        public bool Unfinished { get; set; }

        [JsonProperty("OnBreak")]
        public bool OnBreak { get; set; }

        [JsonProperty("WorldSnookerID")]
        public int WorldSnookerId { get; set; }

        [JsonProperty("LiveUrl")]
        public string LiveUrl { get; set; }

        [JsonProperty("DetailsUrl")]
        public string DetailsUrl { get; set; }

        [JsonProperty("PointsDropped")]
        public bool PointsDropped { get; set; }

        [JsonProperty("ShowCommonNote")]
        public bool ShowCommonNote { get; set; }

        [JsonProperty("Estimated")]
        public bool Estimated { get; set; }

        [JsonProperty("Type")]
        public int Type { get; set; }

        [JsonProperty("TableNo")]
        public int TableNo { get; set; }

        [JsonProperty("VideoURL")]
        public Uri VideoUrl { get; set; }

        [JsonProperty("InitDate")]
        public string InitDate { get; set; }

        [JsonProperty("ModDate")]
        public string ModDate { get; set; }

        [JsonProperty("StartDate")]
        public string StartDate { get; set; }

        [JsonProperty("EndDate")]
        public string EndDate { get; set; }

        [JsonProperty("ScheduledDate")]
        public string ScheduledDate { get; set; }

        [JsonProperty("FrameScores")]
        public string FrameScores { get; set; }

        [JsonProperty("Sessions")]
        public string Sessions { get; set; }

        [JsonProperty("Note")]
        public string Note { get; set; }

        [JsonProperty("ExtendedNote")]
        public string ExtendedNote { get; set; }
    }
}