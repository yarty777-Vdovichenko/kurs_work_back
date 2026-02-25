using kurswork_back.Models;
using kurswork_back.Repositories;

namespace kurswork_back.Services
{
    public interface ITarifService
    {
        Task<List<Tarif>> GetAllAsync();
        Task<Tarif?> GetByIdAsync(string id);
        Task CreateAsync(Tarif tarif);
        Task DeleteAsync(string id);
        Task<bool> UpdateAsync(string id, Tarif updatedTarif);
    }
    public class TarifService : ITarifService
    {
        private readonly ITarifRepository _repository;
        
        public TarifService(ITarifRepository repository)
        {
            _repository = repository;
        }
        public async Task<List<Tarif>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<Tarif?> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("id is empty");

            return await _repository.GetByIdAsync(id);
        }
        public async Task CreateAsync(Tarif tarif)
        {
            if (tarif == null)
                throw new ArgumentNullException(nameof(tarif));

            if (string.IsNullOrWhiteSpace(tarif.Name))
                throw new Exception("WE NEED NAME!!!!!!!!!!!!");

            if (tarif.Internet_capacity<0)
                throw new Exception("WE NEED Internet_capacity!!!!!!!!!!!!");

            if (tarif.Minutes>0)
                throw new Exception("WE NEED Minutes!!!!!!!!!!!!");

            if (string.IsNullOrWhiteSpace(tarif.Additional))
                tarif.Additional = "NONE";

            if (tarif.Price<0)
                throw new Exception("WE NEED Price!!!!!!!!!!!!");

            await _repository.CreateAsync(tarif);
        }
        public async Task DeleteAsync(string id)
        {
            if(string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("id is empty");

            await _repository.DeleteAsync(id);
        }
        public async Task<bool> UpdateAsync(string id, Tarif updatedTarif)
        {
            var existingTarif = await _repository.GetByIdAsync(id);

            if (existingTarif == null)
                return false;

            updatedTarif.Id = existingTarif.Id;

            await _repository.UpdateAsync(updatedTarif);
            return true;
        }
    }
}
