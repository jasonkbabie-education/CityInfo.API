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
        public async Task<IEnumerable<City>> GetCitiesAsync(bool includePointsOfInterest = false)
        {
            if (includePointsOfInterest)
                return await _context.Cities.Include(c => c.PointsOfInterest).OrderBy(c => c.Name).ToListAsync();
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<IEnumerable<City>> GetCitiesAsync(string? name, bool includePointsOfInterest = false)
        {
            if (string.IsNullOrEmpty(name))
            {
                return await GetCitiesAsync(includePointsOfInterest);
            }
            if (includePointsOfInterest)
                return await _context.Cities.Include(c => c.PointsOfInterest).Where(c => c.Name == name).OrderBy(c => c.Name).ToListAsync();
            return await _context.Cities.Where(c => c.Name == name).OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<bool> CityExistsAsync(int cityId)
        { 
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
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
