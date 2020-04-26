using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestSnookerApi.Models;
using TestSnookerApi.Repositories;

namespace TestSnookerApi.Controllers
{
    [Route("")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly IEventsRepository _eventsRepository;
        private readonly IPlayersRepository _playersRepository;
        private readonly IMatchesRepository _matchesRepository;
        private readonly IRoundsRepository _roundsRepository;

        public MainController(
            IEventsRepository eventsRepository,
            IPlayersRepository playersRepository,
            IMatchesRepository matchesRepository,
            IRoundsRepository roundsRepository)
        {
            _eventsRepository = eventsRepository;
            _playersRepository = playersRepository;
            _matchesRepository = matchesRepository;
            _roundsRepository = roundsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] int? t,
            [FromQuery] int? s,
            [FromQuery] int? e,
            [FromQuery] int? r,
            [FromQuery] int? n,
            [FromQuery] int? p)
        {
            if (t.HasValue)
            {
                switch (t)
                {
                    case 5 when s.HasValue:
                    {
                        var events = _eventsRepository.GetSeasonEvents(s.Value);
                        return events == null || !events.Any() ? Ok("\"\"") : Ok(events);
                    }

                    case 6 when e.HasValue:
                    {
                        var matches = _matchesRepository.GetEventMatches(e.Value);
                        return matches == null || !matches.Any() ? Ok("\"\"") : Ok(matches);
                    }

                    case 7:
                    {
                        var ongoingMatches = _matchesRepository.GetOngoingMatches();
                        return ongoingMatches == null || !ongoingMatches.Any() ? Ok("\"\"") : Ok(ongoingMatches);
                    }

                    case 9 when e.HasValue:
                    {
                        var players = _playersRepository.GetEventPlayers(e.Value);
                        return players == null || !players.Any() ? Ok("\"\"") : Ok(players);
                    }

                    case 12 when e.HasValue:
                    {
                        var rounds = _roundsRepository.GetEventRounds(e.Value);
                        return rounds == null || !rounds.Any() ? Ok("\"\"") : Ok(rounds);
                    }

                    default:
                        return Ok();
                }
            }

            if (e.HasValue)
            {
                if (r.HasValue && n.HasValue)
                {
                    var match = _matchesRepository.GetMatch(e.Value, r.Value, n.Value);
                    return Ok(new[] { match });
                }

                var @event = _eventsRepository.GetEvent(e.Value);
                return Ok(new[] { @event });
            }

            if (p.HasValue)
            {
                var player = _playersRepository.GetPlayer(p.Value);
                return Ok(new[] { player });
            }

            return Ok();
        }

        [HttpPost("events")]
        public async Task<IActionResult> PostEvents([FromBody] Event[] events)
        {
            await _eventsRepository.SetEvents(events);
            return Ok();
        }

        [HttpPost("players")]
        public async Task<IActionResult> PostPlayers([FromBody] Player[] players)
        {
            await _playersRepository.SetPlayers(players);
            return Ok();
        }

        [HttpPost("rounds")]
        public async Task<IActionResult> PostRounds([FromBody] RoundInfo[] rounds)
        {
            await _roundsRepository.SetRounds(rounds);
            return Ok();
        }

        [HttpPost("matches")]
        public async Task<IActionResult> PostMatches([FromBody] Match[] matches)
        {
            await _matchesRepository.SetMatches(matches);
            return Ok();
        }

        [HttpPut("matches")]
        public async Task<IActionResult> UpdateMatches([FromBody] Match[] matches)
        {
            await _matchesRepository.UpdateMatches(matches);
            return Ok();
        }
    }
}