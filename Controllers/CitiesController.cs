using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using System.Text.Json;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        const int maxPageSize = 20;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<IActionResult> GetCities(string? name, string? searchQuery, bool includePointsOfInterest = false, int pageNumber = 1, int pageSize = 10)
        {
            if (pageSize > maxPageSize)
                pageSize = maxPageSize;
            var (cityEntities, paginationMetadata) = await _cityInfoRepository.GetCitiesAsync(pageNumber, pageSize, name, searchQuery, includePointsOfInterest);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
            if (includePointsOfInterest)
                return Ok(_mapper.Map<IEnumerable<CityDTO>>(cityEntities));
            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDTO>>(cityEntities));
        }

        /// <summary>
        /// Gets a city by id
        /// </summary>
        /// <param name="id">The id of the city to get</param>
        /// <param name="includePointsOfInterest">Whether or not to include the points of interest in the payload</param>
        /// <returns>An IActionResult</returns>
        /// <response code="200">Returns the requested city</response>
        /// <response code="404">The requested city is not in the database</response>
        /// <response code="400">The action is not being used correctly</response>
        /// 
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest = false)
        {
            var city = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);
            if (city == null) { return NotFound(); }
            if (includePointsOfInterest)
            {
                return Ok(_mapper.Map<CityDTO>(city));
            }
            return Ok(_mapper.Map<CityWithoutPointsOfInterestDTO>(city));
        }
    }
}