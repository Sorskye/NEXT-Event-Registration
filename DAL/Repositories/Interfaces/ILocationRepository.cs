namespace DAL.Repositories.Interfaces;

public interface ILocationRepository
{
    int GetOrCreateLocationId(string locationName);
}
