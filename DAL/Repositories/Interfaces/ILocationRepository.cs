namespace DAL.Repositories.Interfaces;

public interface ILocationRepository // Verantwoordelijk voor [LOCATIES]. dus zoek locatie op naam bestaat die niet? maak hem aan
{
    int GetOrCreateLocationId(string locationName);
}
