using kurswork_back.Data;
using kurswork_back.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace kurswork_back.Repositories
{
    public interface ISubscriberRepository
    {
        Task<(List<Subscriber> items, long total)> GetAllPagedAsync(int page, int pageSize);
        Task<List<Subscriber>> SearchByNameAsync(string fullName);
        Task<List<Subscriber>> FilterAsync(string? simStatus, string? tarifId);
        Task<Subscriber?> GetByIdAsync(string id);
        Task CreateAsync(Subscriber subscriber);
        Task DeleteAsync(string id);
        Task UpdateAsync(Subscriber subscriber);
    }
    public class SubscriberRepository : ISubscriberRepository
    {
        private readonly IMongoCollection<Subscriber> _subscribers;
        public SubscriberRepository (MongoContext context)
        {
            _subscribers = context.Subscribers;
        }
        public async Task<(List<Subscriber> items, long total)> GetAllPagedAsync(int page, int pageSize)
        {
            var total = await _subscribers.CountDocumentsAsync(_ => true);
            var items = await _subscribers.Find(_ => true).Skip((page - 1) * pageSize).Limit(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<List<Subscriber>> SearchByNameAsync(string fullName)
        {
            var filter = Builders<Subscriber>.Filter.Regex(s => s.FullName, new BsonRegularExpression(fullName, "i"));
            return await _subscribers.Find(filter).ToListAsync();
        }

        public async Task<List<Subscriber>> FilterAsync(string? simStatus, string? tarifId)
        {
            var filterBuilder = Builders<Subscriber>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrWhiteSpace(simStatus))
                filter &= filterBuilder.ElemMatch(s => s.Sims, sim => sim.Status == simStatus);

            if (!string.IsNullOrWhiteSpace(tarifId))
                filter &= filterBuilder.ElemMatch(s => s.Sims, sim => sim.TarifId == tarifId);

            return await _subscribers.Find(filter).ToListAsync();
        }
        public async Task<Subscriber?> GetByIdAsync(string id)
        {
            return await _subscribers.Find(subscriber => subscriber.Id == id).FirstOrDefaultAsync();
        }
        public async Task CreateAsync(Subscriber subscriber)
        {
            await _subscribers.InsertOneAsync(subscriber);
        }
        public async Task DeleteAsync(string id)
        {
            await _subscribers.DeleteOneAsync(subscriber => subscriber.Id == id);
        }
        public async Task UpdateAsync(Subscriber subscriber)
        {
            await _subscribers.ReplaceOneAsync(s=>s.Id ==subscriber.Id,subscriber);
        }
    }
}
