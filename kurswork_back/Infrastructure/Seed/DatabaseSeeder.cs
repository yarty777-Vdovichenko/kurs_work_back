using kurswork_back.Data;
using kurswork_back.Models;
using MongoDB.Driver;
using kurswork_back.Services;

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
            await _context.Users.DeleteManyAsync(FilterDefinition<User>.Empty);
            await _context.Tarifs.DeleteManyAsync(FilterDefinition<Tarif>.Empty);
            await _context.Subscribers.DeleteManyAsync(FilterDefinition<Subscriber>.Empty);
            await _context.RegistrationRequests.DeleteManyAsync(FilterDefinition<RegistrationRequest>.Empty);

            var users = new List<User>
            {
                new User { Name = "Admin_Demo", Email = "admin@demo.com", Role = Roles.Admin,
                           PasswordHash = _hasher.HashPassword("Demo12345") },
                new User { Name = "Manager_Demo", Email = "manager@demo.com", Role = Roles.Manager,
                           PasswordHash = _hasher.HashPassword("Demo12345") },
                new User { Name = "User_Demo", Email = "user@demo.com", Role = Roles.User,
                           PasswordHash = _hasher.HashPassword("Demo12345") },
            };
            await _context.Users.InsertManyAsync(users);

            var tarifs = new List<Tarif>
            {
                new Tarif { Name = "Basic", Internet_capacity = 5, Minutes = 100, Additional = "Базовий тариф для дзвінків", Price = 99 },
                new Tarif { Name = "Standard", Internet_capacity = 20, Minutes = 500, Additional = "Оптимальний баланс інтернету й хвилин", Price = 199 },
                new Tarif { Name = "Premium", Internet_capacity = 100, Minutes = 3000, Additional = "Максимум трафіку та хвилин", Price = 399 },
                new Tarif { Name = "Unlimited Max", Internet_capacity = 1000, Minutes = 10000, Additional = "Безлімітний тариф без обмежень", Price = 599 },
            };
            await _context.Tarifs.InsertManyAsync(tarifs);

            var subscribers = new List<Subscriber>
            {
                new Subscriber
                {
                    FullName = "Іваненко Іван Іванович",
                    CreatedAt = DateTime.UtcNow.AddDays(-40),
                    Sims = new List<SimCard>
                    {
                        new SimCard { SimNumber = "+380501112233", Status = "active", CreatedAt = DateTime.UtcNow.AddDays(-40), TarifId = tarifs[0].Id },
                        new SimCard { SimNumber = "+380501112234", Status = "inactive", CreatedAt = DateTime.UtcNow.AddDays(-20), TarifId = tarifs[1].Id }
                    }
                },
                new Subscriber
                {
                    FullName = "Петренко Марія Олексіївна",
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    Sims = new List<SimCard>
                    {
                        new SimCard { SimNumber = "+380672223344", Status = "active", CreatedAt = DateTime.UtcNow.AddDays(-30), TarifId = tarifs[1].Id }
                    }
                },
                new Subscriber
                {
                    FullName = "Коваленко Олег Дмитрович",
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    Sims = new List<SimCard>
                    {
                        new SimCard { SimNumber = "+380933334455", Status = "blocked", CreatedAt = DateTime.UtcNow.AddDays(-15), TarifId = tarifs[2].Id }
                    }
                },
                new Subscriber
                {
                    FullName = "Сидоренко Анна Василівна",
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    Sims = new List<SimCard>
                    {
                        new SimCard { SimNumber = "+380994445566", Status = "active", CreatedAt = DateTime.UtcNow.AddDays(-10), TarifId = tarifs[3].Id },
                        new SimCard { SimNumber = "+380994445567", Status = "active", CreatedAt = DateTime.UtcNow.AddDays(-5), TarifId = tarifs[0].Id }
                    }
                },
                new Subscriber
                {
                    FullName = "Мельник Тарас Ігорович",
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    Sims = new List<SimCard>
                    {
                        new SimCard { SimNumber = "+380685556677", Status = "active", CreatedAt = DateTime.UtcNow.AddDays(-3), TarifId = tarifs[2].Id }
                    }
                },
                new Subscriber
                {
                    FullName = "Шевченко Андрій Миколайович",
                    CreatedAt = DateTime.UtcNow.AddDays(-25),
                    Sims = new List<SimCard>
                    {
                        new SimCard { SimNumber = "+380671234001", Status = "active", CreatedAt = DateTime.UtcNow.AddDays(-25), TarifId = tarifs[0].Id }
                    }
                },
                new Subscriber
                {
                    FullName = "Бондаренко Оксана Сергіївна",
                    CreatedAt = DateTime.UtcNow.AddDays(-18),
                    Sims = new List<SimCard>
                    {
                        new SimCard { SimNumber = "+380501234002", Status = "inactive", CreatedAt = DateTime.UtcNow.AddDays(-18), TarifId = tarifs[1].Id }
                    }
                },
                new Subscriber
                {
                    FullName = "Ткаченко Максим Юрійович",
                    CreatedAt = DateTime.UtcNow.AddDays(-12),
                    Sims = new List<SimCard>
                    {
                        new SimCard { SimNumber = "+380931234003", Status = "active", CreatedAt = DateTime.UtcNow.AddDays(-12), TarifId = tarifs[2].Id }
                    }
                },
                new Subscriber
                {
                    FullName = "Лисенко Катерина Володимирівна",
                    CreatedAt = DateTime.UtcNow.AddDays(-8),
                    Sims = new List<SimCard>
                    {
                        new SimCard { SimNumber = "+380991234004", Status = "blocked", CreatedAt = DateTime.UtcNow.AddDays(-8), TarifId = tarifs[3].Id }
                    }
                },
                new Subscriber
                {
                    FullName = "Гриценко Денис Олександрович",
                    CreatedAt = DateTime.UtcNow.AddDays(-22),
                    Sims = new List<SimCard>
                    {
                        new SimCard { SimNumber = "+380681234005", Status = "active", CreatedAt = DateTime.UtcNow.AddDays(-22), TarifId = tarifs[1].Id },
                        new SimCard { SimNumber = "+380681234006", Status = "active", CreatedAt = DateTime.UtcNow.AddDays(-20), TarifId = tarifs[2].Id }
                    }
                },
                new Subscriber
                {
                    FullName = "Романюк Юлія Василівна",
                    CreatedAt = DateTime.UtcNow.AddDays(-16),
                    Sims = new List<SimCard>
                    {
                        new SimCard { SimNumber = "+380951234007", Status = "inactive", CreatedAt = DateTime.UtcNow.AddDays(-16), TarifId = tarifs[0].Id }
                    }
                },
                new Subscriber
                {
                    FullName = "Данилюк Владислав Ігорович",
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                    Sims = new List<SimCard>
                    {
                        new SimCard { SimNumber = "+380631234008", Status = "active", CreatedAt = DateTime.UtcNow.AddDays(-7), TarifId = tarifs[3].Id }
                    }
                },
                new Subscriber
                {
                    FullName = "Кравченко Наталія Петрівна",
                    CreatedAt = DateTime.UtcNow.AddDays(-14),
                    Sims = new List<SimCard>
                    {
                        new SimCard { SimNumber = "+380731234009", Status = "blocked", CreatedAt = DateTime.UtcNow.AddDays(-14), TarifId = tarifs[2].Id }
                    }
                },
                new Subscriber
                {
                    FullName = "Мороз Артем Сергійович",
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    Sims = new List<SimCard>
                    {
                        new SimCard { SimNumber = "+380661234010", Status = "active", CreatedAt = DateTime.UtcNow.AddDays(-5), TarifId = tarifs[1].Id },
                        new SimCard { SimNumber = "+380661234011", Status = "inactive", CreatedAt = DateTime.UtcNow.AddDays(-2), TarifId = tarifs[0].Id }
                    }
                },
                new Subscriber
                {
                    FullName = "Поліщук Софія Андріївна",
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    Sims = new List<SimCard>
                    {
                        new SimCard { SimNumber = "+380981234012", Status = "active", CreatedAt = DateTime.UtcNow.AddDays(-2), TarifId = tarifs[3].Id }
                    }
                },
            };
            await _context.Subscribers.InsertManyAsync(subscribers);

            var registrationRequests = new List<RegistrationRequest>
            {
                new RegistrationRequest
                {
                    Name = "Новий Кандидат",
                    Email = "candidate1@demo.com",
                    PasswordHash = _hasher.HashPassword("Demo12345"),
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow.AddHours(-5)
                },
                new RegistrationRequest
                {
                    Name = "Друга Заявка",
                    Email = "candidate2@demo.com",
                    PasswordHash = _hasher.HashPassword("Demo12345"),
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow.AddHours(-2)
                },
            };
            await _context.RegistrationRequests.InsertManyAsync(registrationRequests);
        }
    }
}