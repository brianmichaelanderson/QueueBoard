using System;
using System.Linq;
using QueueBoard.Api.Entities;

namespace QueueBoard.Api.Data
{
    public static class SeedData
    {
        public static void EnsureSeeded(QueueBoardDbContext context)
        {
            var now = DateTimeOffset.UtcNow;

            // Queues (8)
            var queues = new[]
            {
                new Queue { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Support", Description = "Customer support queue", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Queue { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Sales", Description = "Sales inquiries", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Queue { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Billing", Description = "Billing and invoices", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Queue { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Onboarding", Description = "New customer onboarding", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Queue { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Escalations", Description = "Escalated tickets", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Queue { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Name = "Returns", Description = "Returns and exchanges", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Queue { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Name = "Technical", Description = "Technical support", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Queue { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), Name = "General", Description = "General inquiries", IsActive = true, CreatedAt = now, UpdatedAt = now },
            };

            foreach (var q in queues)
            {
                if (!context.Queues.Any(x => x.Id == q.Id))
                {
                    context.Queues.Add(q);
                }
            }

            // Agents (20)
            var agents = new[]
            {
                new Agent { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), FirstName = "Alice", LastName = "Anderson", Email = "alice.anderson@example.com", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Agent { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), FirstName = "Bob", LastName = "Brown", Email = "bob.brown@example.com", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Agent { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), FirstName = "Carol", LastName = "Clark", Email = "carol.clark@example.com", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Agent { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), FirstName = "Dan", LastName = "Davis", Email = "dan.davis@example.com", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Agent { Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), FirstName = "Eve", LastName = "Edwards", Email = "eve.edwards@example.com", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Agent { Id = Guid.Parse("00000000-0000-0000-0000-000000000006"), FirstName = "Frank", LastName = "Foster", Email = "frank.foster@example.com", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Agent { Id = Guid.Parse("00000000-0000-0000-0000-000000000007"), FirstName = "Grace", LastName = "Green", Email = "grace.green@example.com", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Agent { Id = Guid.Parse("00000000-0000-0000-0000-000000000008"), FirstName = "Hank", LastName = "Hill", Email = "hank.hill@example.com", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Agent { Id = Guid.Parse("00000000-0000-0000-0000-000000000009"), FirstName = "Ivy", LastName = "Iverson", Email = "ivy.iverson@example.com", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Agent { Id = Guid.Parse("00000000-0000-0000-0000-00000000000a"), FirstName = "Jack", LastName = "Jackson", Email = "jack.jackson@example.com", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Agent { Id = Guid.Parse("00000000-0000-0000-0000-00000000000b"), FirstName = "Kara", LastName = "King", Email = "kara.king@example.com", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Agent { Id = Guid.Parse("00000000-0000-0000-0000-00000000000c"), FirstName = "Liam", LastName = "Lewis", Email = "liam.lewis@example.com", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Agent { Id = Guid.Parse("00000000-0000-0000-0000-00000000000d"), FirstName = "Mona", LastName = "Moore", Email = "mona.moore@example.com", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Agent { Id = Guid.Parse("00000000-0000-0000-0000-00000000000e"), FirstName = "Ned", LastName = "Nelson", Email = "ned.nelson@example.com", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Agent { Id = Guid.Parse("00000000-0000-0000-0000-00000000000f"), FirstName = "Olga", LastName = "Owens", Email = "olga.owens@example.com", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Agent { Id = Guid.Parse("00000000-0000-0000-0000-000000000010"), FirstName = "Paul", LastName = "Parker", Email = "paul.parker@example.com", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Agent { Id = Guid.Parse("00000000-0000-0000-0000-000000000011"), FirstName = "Quinn", LastName = "Quincy", Email = "quinn.quincy@example.com", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Agent { Id = Guid.Parse("00000000-0000-0000-0000-000000000012"), FirstName = "Rita", LastName = "Reed", Email = "rita.reed@example.com", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Agent { Id = Guid.Parse("00000000-0000-0000-0000-000000000013"), FirstName = "Sam", LastName = "Stone", Email = "sam.stone@example.com", IsActive = true, CreatedAt = now, UpdatedAt = now },
                new Agent { Id = Guid.Parse("00000000-0000-0000-0000-000000000014"), FirstName = "Tina", LastName = "Turner", Email = "tina.turner@example.com", IsActive = true, CreatedAt = now, UpdatedAt = now },
            };

            foreach (var a in agents)
            {
                if (!context.Agents.Any(x => x.Email == a.Email))
                {
                    context.Agents.Add(a);
                }
            }

            context.SaveChanges();

            // Assignments (~25) - varied SkillLevel and primaries
            var assignments = new[]
            {
                // Support queue assignments
                Tuple.Create(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("11111111-1111-1111-1111-111111111111"), true, 5),
                Tuple.Create(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("11111111-1111-1111-1111-111111111111"), false, 3),
                Tuple.Create(Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("11111111-1111-1111-1111-111111111111"), false, 4),

                // Sales
                Tuple.Create(Guid.Parse("00000000-0000-0000-0000-000000000004"), Guid.Parse("22222222-2222-2222-2222-222222222222"), true, 5),
                Tuple.Create(Guid.Parse("00000000-0000-0000-0000-000000000005"), Guid.Parse("22222222-2222-2222-2222-222222222222"), false, 2),

                // Billing
                Tuple.Create(Guid.Parse("00000000-0000-0000-0000-000000000006"), Guid.Parse("33333333-3333-3333-3333-333333333333"), true, 4),
                Tuple.Create(Guid.Parse("00000000-0000-0000-0000-000000000007"), Guid.Parse("33333333-3333-3333-3333-333333333333"), false, 3),

                // Onboarding
                Tuple.Create(Guid.Parse("00000000-0000-0000-0000-000000000008"), Guid.Parse("44444444-4444-4444-4444-444444444444"), true, 4),
                Tuple.Create(Guid.Parse("00000000-0000-0000-0000-000000000009"), Guid.Parse("44444444-4444-4444-4444-444444444444"), false, 2),

                // Escalations
                Tuple.Create(Guid.Parse("00000000-0000-0000-0000-00000000000a"), Guid.Parse("55555555-5555-5555-5555-555555555555"), true, 5),

                // Returns
                Tuple.Create(Guid.Parse("00000000-0000-0000-0000-00000000000b"), Guid.Parse("66666666-6666-6666-6666-666666666666"), true, 3),
                Tuple.Create(Guid.Parse("00000000-0000-0000-0000-00000000000c"), Guid.Parse("66666666-6666-6666-6666-666666666666"), false, 2),

                // Technical
                Tuple.Create(Guid.Parse("00000000-0000-0000-0000-00000000000d"), Guid.Parse("77777777-7777-7777-7777-777777777777"), true, 5),
                Tuple.Create(Guid.Parse("00000000-0000-0000-0000-00000000000e"), Guid.Parse("77777777-7777-7777-7777-777777777777"), false, 4),

                // General
                Tuple.Create(Guid.Parse("00000000-0000-0000-0000-00000000000f"), Guid.Parse("88888888-8888-8888-8888-888888888888"), true, 1),
                Tuple.Create(Guid.Parse("00000000-0000-0000-0000-000000000010"), Guid.Parse("88888888-8888-8888-8888-888888888888"), false, 1),

                // Additional cross assignments
                Tuple.Create(Guid.Parse("00000000-0000-0000-0000-000000000011"), Guid.Parse("11111111-1111-1111-1111-111111111111"), false, 2),
                Tuple.Create(Guid.Parse("00000000-0000-0000-0000-000000000012"), Guid.Parse("22222222-2222-2222-2222-222222222222"), false, 3),
                Tuple.Create(Guid.Parse("00000000-0000-0000-0000-000000000013"), Guid.Parse("33333333-3333-3333-3333-333333333333"), false, 2),
                Tuple.Create(Guid.Parse("00000000-0000-0000-0000-000000000014"), Guid.Parse("44444444-4444-4444-4444-444444444444"), false, 3),
                Tuple.Create(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("22222222-2222-2222-2222-222222222222"), false, 2),
                Tuple.Create(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("33333333-3333-3333-3333-333333333333"), false, 1),
                Tuple.Create(Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("44444444-4444-4444-4444-444444444444"), false, 2),
            };

            foreach (var t in assignments)
            {
                var agentId = t.Item1;
                var queueId = t.Item2;
                var isPrimary = t.Item3;
                var skill = t.Item4;

                if (!context.AgentQueues.Any(x => x.AgentId == agentId && x.QueueId == queueId))
                {
                    context.AgentQueues.Add(new AgentQueue
                    {
                        AgentId = agentId,
                        QueueId = queueId,
                        IsPrimary = isPrimary,
                        SkillLevel = skill,
                        AssignedAt = now
                    });
                }
            }

            context.SaveChanges();
        }
    }
}
