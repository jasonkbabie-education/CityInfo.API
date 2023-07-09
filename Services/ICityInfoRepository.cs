using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(int pageNumber, int pageSize, string? name, string? searchQuery, bool includePointsOfInterest = false);
        Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest);
        Task<bool> CityExistsAsync(int cityId);
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);
        Task AddPointOfInterestForCityAsync(int cityId,  PointOfInterest pointOfInterest);
        void DeletePointOfInterest(int cityId, PointOfInterest pointOfInterestId);
        Task<bool> SaveChangesAsync();
    }
}
