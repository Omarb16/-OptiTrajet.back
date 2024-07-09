using Microsoft.AspNetCore.Mvc;
using OptiTrajet.Services;
using OptiTrajet.Services.Interfaces;
using System.Net.Mime;

namespace OptiTrajet.Controllers
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api")]
    public class Controller : ControllerBase
    {
        private readonly IItineraryService _itinerariesService;
        private readonly IPlaceService _placeService;
        private readonly IStationService _stationService;
        private readonly ICityService _cityService;

        public Controller(IItineraryService itinerariesService, IPlaceService placeService, IStationService stationService, ICityService cityService)
        {
            _itinerariesService = itinerariesService;
            _placeService = placeService;
            _stationService = stationService;
            _cityService = cityService;
        }

        [HttpGet("GetStations")]
        public async Task<IActionResult> GetStations()
        {
            try
            {
                return Ok(await _stationService.Get());
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("GetCities")]
        public async Task<IActionResult> GetCities()
        {
            try
            {
                return Ok(await _cityService.Get());
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("FindOptimalCommute")]
        public async Task<IActionResult> FindOptimalCommute([FromBody] FindOptimalCommute command)
        {
            try
            {
                await _itinerariesService.FindOptimalCommute(command);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("AddPlace")]
        public async Task<IActionResult> AddPlace([FromBody] AddPlace command)
        {
            try
            {
                return Ok(await _placeService.Add(command));
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
