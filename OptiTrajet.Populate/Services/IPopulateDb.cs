namespace OptiTrajet.Populate.Services
{
    public interface IPopulateDb
    {
        public Task Populate();
        public void ModifiedStations();
        public void ModifiedCities();
    }
}
