namespace OptiTrajet.Populate.Services
{
    public interface IPopulateDb
    {
        public Task Populate();
        public void Count();
        // https://data.iledefrance-mobilites.fr/explore/dataset/emplacement-des-gares-idf/export/
        public void ModifiedStations();
        // https://france-geojson.gregoiredavid.fr/
        public void ModifiedCities();
    }
}
