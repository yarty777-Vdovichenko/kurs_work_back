using kurswork_back.Data;
using kurswork_back.Models;
using MongoDB.Driver;

namespace kurswork_back.Infrastructure.Seed
{
    public interface IDatabaseSeeder
    {
        Task ResetDemoDataAsync();
    }

    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly MongoContext _context;
        private readonly IPasswordHasher _hasher;

        public DatabaseSeeder(MongoContext context, IPasswordHasher hasher)
        {
            _context = context;
            _hasher = hasher;
        }

        public async Task ResetDemoDataAsync()
        {
            // 1. чистимо все
            await _context.Users.DeleteManyAsync(FilterDefinition<User>.Empty);
            await _context.Tarifs.DeleteManyAsync(FilterDefinition<Tarif>.Empty);
            await _context.Subscribers.DeleteManyAsync(FilterDefinition<Subscriber>.Empty);
            await _context.RegistrationRequests.DeleteManyAsync(FilterDefinition<RegistrationRequest>.Empty);

            // 2. фіксовані demo-акаунти з відомими рекрутеру паролями
            var users = new List<User>
            {
                new User { Name = "Admin Demo", Email = "admin@demo.com", Role = Roles.Admin,
                           PasswordHash = _hasher.HashPassword("Demo12345!") },
                new User { Name = "Manager Demo", Email = "manager@demo.com", Role = Roles.Manager,
                           PasswordHash = _hasher.HashPassword("Demo12345!") },
                new User { Name = "User Demo", Email = "user@demo.com", Role = Roles.User,
                           PasswordHash = _hasher.HashPassword("Demo12345!") },
            };
            await _context.Users.InsertManyAsync(users);

            // 3. базові тарифи
            var tarifs = new List<Tarif>
            {
                new Tarif { Name = "Basic", Internet_capacity = 5, Minutes = 100, Price = 99 },
                new Tarif { Name = "Standard", Internet_capacity = 20, Minutes = 500, Price = 199 },
                new Tarif { Name = "Premium", Internet_capacity = 100, Minutes = 3000, Price = 399 },
            };
            await _context.Tarifs.InsertManyAsync(tarifs);

            // 4. кілька абонентів для наглядності
            var subscribers = new List<Subscriber>
            {
                new Subscriber
                {
                    FullName = "Іваненко Іван Іванович",
                    CreatedAt = DateTime.UtcNow,
                    Sims = new List<SimCard>
                    {
                        new SimCard { SimNumber = "+380501112233", Status = "active", CreatedAt = DateTime.UtcNow, TarifId = tarifs[0].Id }
                    }
                },
                // додай ще пару за потреби
            };
            await _context.Subscribers.InsertManyAsync(subscribers);
        }
    }
}