using Microsoft.AspNetCore.Mvc;
using OptiTrajet.Domain.In;
using OptiTrajet.Exceptions;
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
            catch (Exception ex)
            {
                _logger.Error(ex, "{ex}");
                return BadRequest();
            }
        }

        [HttpPost("Report")]
        public async Task<IActionResult> GetReport([FromBody] GetReport command)
        {
            try
            {
                var stream = await _itinerariesService.GetReport(command);

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            catch(FunctionalException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "{ex}");
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
                _logger.Error(ex, "{ex}");
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
                _logger.Error(ex, "{ex}");
                return BadRequest();
            }
        }

        [HttpPost("FindItineraries")]
        public async Task<IActionResult> FindItineraries([FromBody] FindOptimalCommute command)
        {
            try
            {
                await _itinerariesService.FindItineraries(command);
                return Ok();
            }
            catch (FunctionalException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "{ex}");
                return BadRequest();
            }
        }
    }
}
