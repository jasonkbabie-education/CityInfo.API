using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        //implement IQueryable<> to reduce duplicate code
        public async Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(int pageNumber, int pageSize, string? name, string? searchQuery, bool includePointsOfInterest = false)
        {
            var cities = _context.Cities as IQueryable<City>;

            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                cities = cities.Where(c => c.Name == name);
            }

            if (!string.IsNullOrEmpty(searchQuery))
            { 
                searchQuery = searchQuery.Trim();
                cities = cities.Where(c => c.Name.Contains(searchQuery) || (c.Description != null && c.Description.Contains(searchQuery)));
            }

            cities = cities.OrderBy(c => c.Name).Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            var totalItemCount = await cities.CountAsync();
            var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);
            IEnumerable<City> filteredCities;
            if (includePointsOfInterest)
                filteredCities = await cities.Include(c => c.PointsOfInterest).ToListAsync();
            else
                await cities.ToListAsync();
            return (cities, paginationMetadata);
        }

        public async Task<bool> CityExistsAsync(int cityId)
        { 
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task<bool> CityNameMatchesCityIdAsync(string? cityName, int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId && c.Name == cityName);
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
                return await _context.Cities.Include(c => c.PointsOfInterest).Where(c => c.Id == cityId).FirstOrDefaultAsync();
            return await _context.Cities.Where(c => c.Id == cityId).FirstOrDefaultAsync();
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            return await _context.PointOfInterests.Where(p => p.CityId == cityId && p.Id == pointOfInterestId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
        {
            return await _context.PointOfInterests.Where(p => p.CityId  == cityId).ToListAsync();
        }

        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);
            if (city != null)
            {
                city.PointsOfInterest.Add(pointOfInterest);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public void DeletePointOfInterest(int cityId, PointOfInterest pointOfInterest)
        {
            _context.PointOfInterests.Remove(pointOfInterest);
        }
    }
}
