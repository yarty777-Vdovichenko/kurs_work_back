using kurswork_back.Data;
using kurswork_back.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace kurswork_back.Repositories
{
    public interface ISubscriberRepository
    {
        Task<(List<Subscriber> items, long total)> GetAllPagedAsync(int page, int pageSize);
        Task<(List<Subscriber>, long)> SearchPagedAsync(string number, string fullName, int page, int pageSize);
        Task<(List<Subscriber>, long)> FilterPagedAsync(string? simStatus, string? tarifId, int page, int pageSize);
        Task<Subscriber?> GetByIdAsync(string id);
        Task CreateAsync(Subscriber subscriber);
        Task DeleteAsync(string id);
        Task UpdateAsync(Subscriber subscriber);
        Task<bool> SimNumberExistsAsync(string simNumber);

    }
    public class SubscriberRepository : ISubscriberRepository
    {
        private readonly IMongoCollection<Subscriber> _subscribers;
        public SubscriberRepository (MongoContext context)
        {
            _subscribers = context.Subscribers;
        }
        public async Task<bool> SimNumberExistsAsync(string simNumber)
        {
            var filter = Builders<Subscriber>.Filter
                .ElemMatch(s => s.Sims, sim => sim.SimNumber == simNumber);
            return await _subscribers.Find(filter).AnyAsync();
        }
        public async Task<(List<Subscriber> items, long total)> GetAllPagedAsync(int page, int pageSize)
        {
            var total = await _subscribers.CountDocumentsAsync(_ => true);
            var items = await _subscribers.Find(_ => true).Skip((page - 1) * pageSize).Limit(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<(List<Subscriber>, long)> SearchPagedAsync(
        string number,
        string fullName,
        int page,
        int pageSize)
        {

            var filterBuilder = Builders<Subscriber>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                filter &= filterBuilder.Regex(
                    s => s.FullName,
                    new BsonRegularExpression(fullName, "i")
                );
            }

            if (!string.IsNullOrWhiteSpace(number))
            {
                number = number.Replace("+", "").Replace(" ", "");
                filter &= filterBuilder.ElemMatch(
                    s => s.Sims,
                    sim => sim.SimNumber.Contains(number)
                );
            }

            var total = await _subscribers.CountDocumentsAsync(filter);

            var items = await _subscribers
                .Find(filter)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<(List<Subscriber>, long)> FilterPagedAsync(string? simStatus, string? tarifId, int page, int pageSize)
        {
            var filterBuilder = Builders<Subscriber>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrWhiteSpace(simStatus))
                filter &= filterBuilder.ElemMatch(s => s.Sims, sim => sim.Status == simStatus);

            if (!string.IsNullOrWhiteSpace(tarifId))
                filter &= filterBuilder.ElemMatch(s => s.Sims, sim => sim.TarifId == tarifId);

            var total = await _subscribers.CountDocumentsAsync(filter);

            var items = await _subscribers
                .Find(filter)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return (items, total);
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
