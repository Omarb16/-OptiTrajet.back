using Microsoft.EntityFrameworkCore;
using OptiTrajet.Domain.Entities;
using OptiTrajet.Persistence;
using OptiTrajet.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OptiTrajet.Services
{
    public class PlaceService : IPlaceService
    {
        private readonly OptiTrajetContext _dbContext;

        public PlaceService(OptiTrajetContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Place> Add(AddPlace command)
        {
            if (command == null) throw new Exception("Input not valid");

            // TODO : lat long outside idf?

            var place = await _dbContext.Places.FirstOrDefaultAsync(x => x.Lat == command.Lat && x.Lon == command.Lon);

            if (place == null)
            {
                place = new Place
                {
                    Lat = command.Lat,
                    Lon = command.Lon,
                    Adresse = command.Adresse,
                };

                await _dbContext.Places.AddAsync(place);

                await _dbContext.SaveChangesAsync();
            }

            return place!;
        }
    }

    public record AddPlace
    {
        public decimal Lat { get; set; } = 0;
        public decimal Lon { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;
    }
}
