using Microsoft.AspNetCore.Mvc;
using OptiTrajet.Dtos.In;
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
        private readonly IStationService _stationService;
        private readonly ICityService _cityService;
        private readonly ILineService _lineService;
        private readonly Serilog.ILogger _logger;

        public Controller(IItineraryService itinerariesService, IStationService stationService, ICityService cityService, ILineService lineService, Serilog.ILogger logger)
        {
            _itinerariesService = itinerariesService;
            _stationService = stationService;
            _cityService = cityService;
            _lineService = lineService;
            _logger = logger;
        }

        [HttpPost("GetStations")]
        public async Task<IActionResult> GetStations([FromBody] GetStations command)
        {
            try
            {
                return Ok(await _stationService.Get(command));
            }
            catch(Exception ex)
            {
                _logger.Error("{ex}", ex);
                return BadRequest();
            }
        }

        [HttpGet("GetReport/{id}")]
        public async Task<IActionResult> GetReport(Guid id)
        {
            try
            {
                return Ok(await _itinerariesService.GetReport(id));
            }
            catch (Exception ex)
            {
                _logger.Error("{ex}", ex);
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
            catch (Exception ex)
            {
                _logger.Error("{ex}", ex);
                return BadRequest();
            }
        }

        [HttpGet("GetLines")]
        public async Task<IActionResult> GetLines()
        {
            try
            {
                return Ok(await _lineService.Get());
            }
            catch (Exception ex)
            {
                _logger.Error("{ex}", ex);
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
            catch (Exception ex)
            {
                _logger.Error("{ex}", ex);
                return BadRequest();
            }
        }
    }
}
