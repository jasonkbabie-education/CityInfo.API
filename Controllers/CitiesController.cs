using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly CityDataStore _cityDataStore;

        public CitiesController(CityDataStore cityDataStore)
        {
            _cityDataStore = cityDataStore ?? throw new ArgumentNullException(nameof(cityDataStore));
        }

        [HttpGet]
        public ActionResult<IEnumerable<CityDTO>> GetCities()
        {
            return Ok(_cityDataStore.Cities);
        }

        [HttpGet("{id}")]
        public ActionResult<CityDTO> GetCity(int id)
        {
            //find city
            var cityToReturn = _cityDataStore.Cities.FirstOrDefault(x => x.Id == id);
            if (cityToReturn == null)
            {
                return NotFound();
            }
            return Ok(cityToReturn);
        }
    }
}