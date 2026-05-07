using kurswork_back.DTOs;
using kurswork_back.Models;
using kurswork_back.Repositories;
using MongoDB.Bson;

namespace kurswork_back.Services
{
    public interface ISubscriberService
    {
        Task<object> GetAllAsync(int page);
        Task<object> SearchAsync(string number, string fullName, int page);
        Task<object> FilterAsync(string? simStatus, string? tarifId, int page);
        Task<Subscriber?> GetByIdAsync(string id);
        Task CreateAsync(Subscriber subscriber);
        Task DeleteAsync(string id);
        Task<bool> UpdateAsync(string id, Subscriber newSubscriber);
        Task<bool> AddSimAsync(string subscriberId, CreateSimDto dto);
        Task<SimCard?> GetSimAsync(string subscriberId, string simId);
        Task<bool> UpdateSimAsync(string subscriberId, string simId, UpdateSimDto dto);
        Task<bool> DeleteSimAsync(string subscriberId, string simId);
        Task<object> GetStatsAsync();
    }

    public class SubscriberService : ISubscriberService
    {
        private readonly ISubscriberRepository _repository;
        private readonly ITarifRepository _tarifRepository;
        private static readonly Random _rng = new Random();

        public SubscriberService(ISubscriberRepository repository, ITarifRepository tarifRepository)
        {
            _repository = repository;
            _tarifRepository = tarifRepository;
        }

        private const int PageSize = 4;

        private async Task<string> GenerateUniqueSimNumberAsync()
        {
            string number;
            bool exists;
            do
            {
                long suffix = (long)(_rng.NextDouble() * 1_000_000_000);
                number = $"+380{suffix:D9}";
                exists = await _repository.SimNumberExistsAsync(number);
            } while (exists);

            return number;
        }

        public async Task<object> GetAllAsync(int page)
        {
            if (page < 1) page = 1;
            var (items, total) = await _repository.GetAllPagedAsync(page, PageSize);
            return new
            {
                items,
                totalCount = total,
                page,
                pageSize = PageSize,
                totalPages = (int)Math.Ceiling((double)total / PageSize)
            };
        }

        public async Task<object> SearchAsync(string number, string fullName, int page)
        {
            if (string.IsNullOrWhiteSpace(fullName) && string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Введіть ім'я або номер для пошуку");

            if (page < 1) page = 1;

            var (items, total) = await _repository.SearchPagedAsync(number, fullName, page, PageSize);

            return new
            {
                items,
                totalCount = total,
                page,
                pageSize = PageSize,
                totalPages = (int)Math.Ceiling((double)total / PageSize)
            };
        }

        public async Task<object> FilterAsync(string? simStatus, string? tarifId, int page)
        {
            if (string.IsNullOrWhiteSpace(simStatus) && string.IsNullOrWhiteSpace(tarifId))
                throw new ArgumentException("Вкажіть хоча б один фільтр");

            if (page < 1) page = 1;

            var (items, total) = await _repository.FilterPagedAsync(simStatus, tarifId, page, PageSize);

            return new
            {
                items,
                totalCount = total,
                page,
                pageSize = PageSize,
                totalPages = (int)Math.Ceiling((double)total / PageSize)
            };
        }

        public async Task<Subscriber?> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Пустий ід");

            return await _repository.GetByIdAsync(id);
        }

        public async Task CreateAsync(Subscriber subscriber)
        {
            if (string.IsNullOrWhiteSpace(subscriber.FullName) || subscriber.FullName.Length < 4)
                throw new ArgumentException("Коротке ім'я");

            subscriber.CreatedAt = DateTime.UtcNow;

            if (subscriber.Sims != null)
            {
                foreach (var sim in subscriber.Sims)
                {
                    sim.SimNumber = await GenerateUniqueSimNumberAsync();
                    if (sim.CreatedAt == default)
                        sim.CreatedAt = DateTime.UtcNow;  // було .ToString("...")
                }
            }

            await _repository.CreateAsync(subscriber);
        }

        public async Task<bool> AddSimAsync(string subscriberId, CreateSimDto dto)
        {
            var subscriber = await _repository.GetByIdAsync(subscriberId);

            if (subscriber == null)
                return false;

            var newSim = new SimCard
            {
                Id = ObjectId.GenerateNewId().ToString(),
                SimNumber = await GenerateUniqueSimNumberAsync(),
                TarifId = dto.TarifId,
                CreatedAt = DateTime.UtcNow  // було .ToString("...")
            };

            if (subscriber.Sims == null)
                subscriber.Sims = new List<SimCard>();

            subscriber.Sims.Add(newSim);
            await _repository.UpdateAsync(subscriber);

            return true;
        }

        public async Task<SimCard?> GetSimAsync(string subscriberId, string simId)
        {
            var subscriber = await _repository.GetByIdAsync(subscriberId);

            if (subscriber == null || subscriber.Sims == null)
                return null;

            return subscriber.Sims.FirstOrDefault(s => s.Id == simId);
        }

        public async Task<bool> UpdateSimAsync(string subscriberId, string simId, UpdateSimDto dto)
        {
            var subscriber = await _repository.GetByIdAsync(subscriberId);

            if (subscriber == null || subscriber.Sims == null)
                return false;

            var sim = subscriber.Sims.FirstOrDefault(s => s.Id == simId);

            if (sim == null)
                return false;

            sim.Status = dto.Status;
            sim.TarifId = dto.TarifId;

            await _repository.UpdateAsync(subscriber);
            return true;
        }

        public async Task<bool> DeleteSimAsync(string subscriberId, string simId)
        {
            var subscriber = await _repository.GetByIdAsync(subscriberId);

            if (subscriber == null || subscriber.Sims == null)
                return false;

            var sim = subscriber.Sims.FirstOrDefault(s => s.Id == simId);

            if (sim == null)
                return false;

            subscriber.Sims.Remove(sim);
            await _repository.UpdateAsync(subscriber);

            return true;
        }

        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Пустий ід");

            await _repository.DeleteAsync(id);
        }

        public async Task<bool> UpdateAsync(string id, Subscriber newSubscriber)
        {
            var sub = await _repository.GetByIdAsync(id);

            if (sub == null)
                return false;

            newSubscriber.Id = id;
            await _repository.UpdateAsync(newSubscriber);
            return true;
        }

        public async Task<object> GetStatsAsync()
        {
            var totalSubscribers = await _repository.CountSubscribersAsync();
            var activeSims = await _repository.CountSimsByStatusAsync("active");
            var blockedSims = await _repository.CountSimsByStatusAsync("blocked");
            var newLast7Days = await _repository.CountNewSubscribersAsync(7);
            var totalTarifs = await _tarifRepository.CountAsync();

            return new
            {
                totalSubscribers,
                activeSims,
                blockedSims,
                newSubscribersLast7Days = newLast7Days,
                totalTarifs
            };
        }
    }
}