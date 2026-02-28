using kurswork_back.Data;
using kurswork_back.Models;
using MongoDB.Driver;

namespace kurswork_back.Repositories
{
    public interface ISubscriberRepository
    {
        Task<List<Subscriber>> GetAllAsync();
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
        public async Task<List<Subscriber>> GetAllAsync()
        {
            return await _subscribers
                            .Find(_ => true)
                            .ToListAsync();
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
