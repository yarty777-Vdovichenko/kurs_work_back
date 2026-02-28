using kurswork_back.Models;
using kurswork_back.Repositories;

namespace kurswork_back.Services
{
    public interface ISubscriberService
    {
        Task<List<Subscriber>> GetAllAsync();
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
        public async Task<List<Subscriber>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<Subscriber?> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Id is empty");

            return await _repository.GetByIdAsync(id);
        }
        public async Task CreateAsync(Subscriber subscriber)
        {
            if (string.IsNullOrWhiteSpace(subscriber.FullName) || subscriber.FullName.Length < 4)
                throw new ArgumentException("Name is too short");

            subscriber.CreatedAt = DateTime.UtcNow;

            await _repository.CreateAsync(subscriber);
        }
        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Id is empty");

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
