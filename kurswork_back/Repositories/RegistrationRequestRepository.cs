using kurswork_back.Data;
using kurswork_back.Models;
using Microsoft.AspNetCore.Http.Features;
using MongoDB.Driver;

namespace kurswork_back.Repositories
{
    public interface IRegistrationRequestRepository
    {
        Task<List<RegistrationRequest>> GetAllAsync();
        Task<List<RegistrationRequest>> GetByStatusAsync(string status);
        Task<RegistrationRequest?> GetByIdAsync(string id);
        Task<RegistrationRequest?> GetPendingByEmailAsync(string email);
        Task CreateAsync(RegistrationRequest request);
        Task DeleteAsync(string id);
        Task UpdateStatusAsync(string id, string status);
    }

    public class RegistrationRequestRepository : IRegistrationRequestRepository
    {
        private readonly IMongoCollection<RegistrationRequest> _requests;

        public RegistrationRequestRepository(MongoContext context)
        {
            _requests = context.RegistrationRequests;
        }

        public async Task<List<RegistrationRequest>> GetAllAsync()
        {
            var items = await _requests.Find(_ => true).ToListAsync();

            return items;
        } 
            

        public async Task<List<RegistrationRequest>> GetByStatusAsync(string status) =>
            await _requests.Find(r => r.Status == status).ToListAsync();

        public async Task<RegistrationRequest?> GetByIdAsync(string id) =>
            await _requests.Find(r => r.Id == id).FirstOrDefaultAsync();

        public async Task<RegistrationRequest?> GetPendingByEmailAsync(string email) =>
            await _requests.Find(r => r.Email == email && r.Status == "Pending").FirstOrDefaultAsync();

        public async Task CreateAsync(RegistrationRequest request) =>
            await _requests.InsertOneAsync(request);

        public async Task UpdateStatusAsync(string id, string status)
        {
            var update = Builders<RegistrationRequest>.Update.Set(r => r.Status, status);
            await _requests.UpdateOneAsync(r => r.Id == id, update);
        }
        public async Task DeleteAsync(string id)
        {
            await _requests.DeleteOneAsync(r => r.Id == id);
        }
    }
}