using kurswork_back.Data;
using kurswork_back.Models;
using MongoDB.Driver;

namespace kurswork_back.Repositories
{
    public interface ITarifRepository
    {
        Task<List<Tarif>> GetAllAsync();
        Task<Tarif?> GetByIdAsync(string id);
        Task CreateAsync(Tarif tarif);
        Task DeleteAsync(string id);
        Task UpdateAsync(Tarif tarif);
    }
    public class TarifRepository : ITarifRepository
    {
        private readonly IMongoCollection<Tarif> _tarifs;

        public TarifRepository(MongoContext context)
        {
            _tarifs = context.Tarifs;
        }

        public async Task<List<Tarif>> GetAllAsync()
        {
            return await _tarifs
                .Find(_ => true)
                .ToListAsync();
        }
        public async Task<Tarif?> GetByIdAsync(string id)
        {
            return await _tarifs
                .Find(tarif => tarif.Id == id)
                .FirstOrDefaultAsync();
        }
        public async Task CreateAsync(Tarif tarif)
        {
            await _tarifs.InsertOneAsync(tarif);
        }
        public async Task DeleteAsync(string id)
        {
            await _tarifs.DeleteOneAsync(t => t.Id == id);
        }
        public async Task UpdateAsync(Tarif tarif)
        {
            await _tarifs.ReplaceOneAsync(t=>t.Id==tarif.Id,tarif);
        }
    }
}
