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
        Task<long> CountSubscribersAsync();
        Task<long> CountSimsByStatusAsync(string status);
        Task<long> CountNewSubscribersAsync(int days);
        Task<List<(string TarifId, long Count)>> CountSimsByTarifAsync();
        Task<List<(DateTime Date, long Count)>> CountSubscribersByDayAsync(int days);
    }

    public class SubscriberRepository : ISubscriberRepository
    {
        private readonly IMongoCollection<Subscriber> _subscribers;

        public SubscriberRepository(MongoContext context)
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
            var items = await _subscribers.Find(_ => true)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
            return (items, total);
        }

        public async Task<(List<Subscriber>, long)> SearchPagedAsync(
            string number, string fullName, int page, int pageSize)
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
                    Builders<SimCard>.Filter.Regex(
                        sim => sim.SimNumber,
                        new BsonRegularExpression(number, "i")
                    )
                );
            }

            var total = await _subscribers.CountDocumentsAsync(filter);
            var items = await _subscribers.Find(filter)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<(List<Subscriber>, long)> FilterPagedAsync(
            string? simStatus, string? tarifId, int page, int pageSize)
        {
            var filterBuilder = Builders<Subscriber>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrWhiteSpace(simStatus))
                filter &= filterBuilder.ElemMatch(s => s.Sims, sim => sim.Status == simStatus);

            if (!string.IsNullOrWhiteSpace(tarifId))
                filter &= filterBuilder.ElemMatch(s => s.Sims, sim => sim.TarifId == tarifId);

            var total = await _subscribers.CountDocumentsAsync(filter);

            var items = await _subscribers.Find(filter)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            // Фільтруємо SIM-карти всередині кожного абонента
            foreach (var subscriber in items)
            {
                subscriber.Sims = subscriber.Sims.Where(sim =>
                    (string.IsNullOrWhiteSpace(simStatus) || sim.Status == simStatus) &&
                    (string.IsNullOrWhiteSpace(tarifId) || sim.TarifId == tarifId)
                ).ToList();
            }

            return (items, total);
        }

        public async Task<Subscriber?> GetByIdAsync(string id)
        {
            return await _subscribers
                .Find(subscriber => subscriber.Id == id)
                .FirstOrDefaultAsync();
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
            await _subscribers.ReplaceOneAsync(s => s.Id == subscriber.Id, subscriber);
        }

        public async Task<long> CountSubscribersAsync()
        {
            return await _subscribers.CountDocumentsAsync(_ => true);
        }

        public async Task<long> CountSimsByStatusAsync(string status)
        {
            var filter = Builders<Subscriber>.Filter
                .ElemMatch(s => s.Sims, sim => sim.Status == status);

            var subscribers = await _subscribers.Find(filter).ToListAsync();

            return subscribers
                .SelectMany(s => s.Sims ?? new List<SimCard>())
                .Count(sim => sim.Status == status);
        }

        public async Task<long> CountNewSubscribersAsync(int days)
        {
            var since = DateTime.UtcNow.AddDays(-days);
            var filter = Builders<Subscriber>.Filter.Gte(s => s.CreatedAt, since);
            return await _subscribers.CountDocumentsAsync(filter);
        }
        public async Task<List<(string TarifId, long Count)>> CountSimsByTarifAsync()
        {
            var pipeline = new[]
            {
                new BsonDocument("$unwind", "$sims"),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$sims.TarifId" },
                    { "count", new BsonDocument("$sum", 1) }
                })
            };

            var results = await _subscribers
                .Aggregate<BsonDocument>(pipeline)
                .ToListAsync();

            return results
                .Select(r => (
                    TarifId: r["_id"].AsString,
                    Count: (long)r["count"].AsInt32
                ))
                .ToList();
        }

        public async Task<List<(DateTime Date, long Count)>> CountSubscribersByDayAsync(int days)
        {
            var allSubscribers = await _subscribers
                .Find(_ => true)
                .ToListAsync();

            var today = DateTime.UtcNow.Date;
            var since = today.AddDays(-days + 1);

            var countPerDay = allSubscribers
                .GroupBy(s => s.CreatedAt.Date)
                .ToDictionary(
                    g => g.Key,   
                    g => (long)g.Count()  
                );

            var result = new List<(DateTime Date, long Count)>();
            long runningTotal = 0;

            for (var date = since; date <= today; date = date.AddDays(1))
            {
                if (date == since)
                {
                    runningTotal = allSubscribers.Count(s => s.CreatedAt.Date < since);
                }

                if (countPerDay.TryGetValue(date, out var newToday))
                {
                    runningTotal += newToday;
                }

                result.Add((date, runningTotal));
            }

            return result;
        }
    }
}