using System.Threading.Tasks;
using BetSnooker.Services;
using Microsoft.AspNetCore.Mvc;

namespace BetSnooker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScoresController : ControllerBase
    {
        private readonly IScoresService _scoresService;

        public ScoresController(IScoresService scoresService)
        {
            _scoresService = scoresService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _scoresService.GetScore("");
            return Ok(result);
        }
    }
}