using kurswork_back.Models;
using kurswork_back.Repositories;

namespace kurswork_back.Services
{
    public interface ISubscriberService
    {
        Task<object> GetAllAsync(int page);
        Task<List<Subscriber>> SearchAsync(string fullName);
        Task<List<Subscriber>> FilterAsync(string? simStatus, string? tarifId);
        Task<Subscriber?> GetByIdAsync(string id);
        Task CreateAsync(Subscriber subscriber);
        Task DeleteAsync(string id);
        Task<bool> UpdateAsync(string id, Subscriber newSubscriber);
    }
    public class SubscriberService : ISubscriberService
    {
        private readonly ISubscriberRepository _repository;

        public SubscriberService(ISubscriberRepository repository)
        {
            _repository = repository;
        }

        private const int PageSize = 10;

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

        public async Task<List<Subscriber>> SearchAsync(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Введіть ім'я для пошуку");
            return await _repository.SearchByNameAsync(fullName);
        }

        public async Task<List<Subscriber>> FilterAsync(string? simStatus, string? tarifId)
        {
            if (string.IsNullOrWhiteSpace(simStatus) && string.IsNullOrWhiteSpace(tarifId))
                throw new ArgumentException("Вкажіть хоча б один фільтр");
            return await _repository.FilterAsync(simStatus, tarifId);
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

            await _repository.CreateAsync(subscriber);
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
    }
}
