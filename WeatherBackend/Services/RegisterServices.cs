using MongoDB.Driver;
using WeatherBackend.Models;

namespace WeatherBackend.Services
{
    public class RegisterServices
    {
        private readonly IMongoCollection<RegisterData> _loginCollection;
        private readonly IMongoCollection<GoogleData> _googleCollection;

        public RegisterServices(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDb"));
            var database = client.GetDatabase("weatherdb");
            _loginCollection = database.GetCollection<RegisterData>("register");
            _googleCollection = database.GetCollection<GoogleData>("register");
        }

        public async Task CreateAsyncUser(RegisterData userData) =>
        await _loginCollection.InsertOneAsync(userData);

        public async Task CreateAsyncUserGoogle(GoogleData userData)=>
            await _googleCollection.InsertOneAsync(userData);
        public async Task<RegisterData> GetUser(string email) =>
            await _loginCollection.Find(w => w.Email == email).FirstOrDefaultAsync();
        //return user.Password;

        public async Task AddFavouriteAsync(string email, string favourite)
        {
            var filter = Builders<RegisterData>.Filter.Eq(u => u.Email, email);
            var update = Builders<RegisterData>.Update.AddToSet(u => u.Favourites, favourite);
            await _loginCollection.UpdateOneAsync(filter, update);
        }


        public async Task<List<string>> GetFavouritesAsync(string email)
        {
            var user = await _loginCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
            return user?.Favourites ?? new List<string>();
        }

        

    }
}
