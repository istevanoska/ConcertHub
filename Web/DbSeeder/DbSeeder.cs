using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace Web.DbSeeder;

public static class DbSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ConcertApplicationUser> userManager)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Users.AnyAsync()) return;

        var rng = new Random(42);

        var firstNames = new[]
        {
            "Liam", "Olivia", "Noah", "Emma", "Oliver", "Ava", "Elijah", "Sophia", "James", "Isabella",
            "William", "Mia", "Benjamin", "Charlotte", "Lucas", "Amelia", "Henry", "Harper", "Alexander", "Evelyn",
            "Mason", "Abigail", "Ethan", "Emily", "Daniel", "Ella", "Matthew", "Elizabeth", "Aiden", "Camila",
            "Jackson", "Luna", "Sebastian", "Sofia", "Jack", "Avery", "Owen", "Mila", "Samuel", "Aria"
        };
        var lastNames = new[]
        {
            "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
            "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin"
        };
        var roles = new[] { UserRole.Attendee, UserRole.Organizer, UserRole.Admin };

        var users = new List<ConcertApplicationUser>();
        for (var i = 0; i < 60; i++)
        {
            var firstName = firstNames[i % firstNames.Length];
            var lastName = lastNames[i % lastNames.Length];
            var username = $"{firstName.ToLower()}.{lastName.ToLower()}{i}";
            var user = new ConcertApplicationUser
            {
                UserName = username,
                Email = $"{username}@concerts.dev",
                EmailConfirmed = true,
                FirstName = firstName,
                LastName = lastName,
                Role = i == 0 ? UserRole.Admin : roles[i % roles.Length]
            };
            var result = await userManager.CreateAsync(user, "Password123!");
            if (result.Succeeded) users.Add(user);
        }

        var genres = Enum.GetValues<Genre>();
        var countries = new[] { "USA", "UK", "Germany", "Sweden", "France", "Canada", "Australia", "Macedonia" };
        var artistNames = new[]
        {
            "The Midnight Echoes", "Neon Pulse", "Velvet Thunder", "Aurora Skies", "Iron Verse",
            "Crimson Tide", "Electric Mirage", "Silver Lining", "The Wandering Notes", "Cosmic Drift",
            "Golden Hour", "Static Bloom", "Lunar Waves", "The Wildfire", "Echo Chamber",
            "Marble Kings", "Sapphire Nights", "Phantom Groove", "Solar Flare", "Midnight Society"
        };
        var artists = artistNames.Select((name, i) => new Artist
        {
            Id = Guid.NewGuid(),
            Name = name,
            Genre = genres[i % genres.Length],
            Country = countries[i % countries.Length],
            FormedYear = rng.Next(1975, 2022),
            Bio = $"{name} is a {genres[i % genres.Length]} act known for energetic live shows."
        }).ToList();
        await context.Artists.AddRangeAsync(artists);

        var venueData = new (string Name, string City)[]
        {
            ("Arena Hall", "Skopje"), ("Riverside Amphitheatre", "London"), ("The Grand Stage", "Berlin"),
            ("Sunset Pavilion", "Paris"), ("Metro Club", "New York"), ("Harbor Lights", "Sydney"),
            ("Old Town Square", "Prague"), ("Crystal Dome", "Toronto"), ("The Underground", "Manchester"),
            ("Lakeside Arena", "Zurich")
        };
        var venues = venueData.Select(v => new Venue
        {
            Id = Guid.NewGuid(),
            Name = v.Name,
            City = v.City,
            Address = $"{rng.Next(1, 200)} Main St, {v.City}",
            Capacity = rng.Next(200, 5000)
        }).ToList();
        await context.Venues.AddRangeAsync(venues);

        var categories = new List<TicketCategory>
        {
            new() { Id = Guid.NewGuid(), Name = "Early Bird", PriceMultiplier = 0.8m, Description = "Discounted early purchase." },
            new() { Id = Guid.NewGuid(), Name = "Standard", PriceMultiplier = 1.0m, Description = "Regular admission." },
            new() { Id = Guid.NewGuid(), Name = "VIP", PriceMultiplier = 2.5m, Description = "Front stage + lounge access." },
            new() { Id = Guid.NewGuid(), Name = "Backstage", PriceMultiplier = 4.0m, Description = "Meet & greet with the artists." }
        };
        await context.TicketCategories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        var now = DateTime.UtcNow;
        var admin = users.First();
        var concerts = new List<Concert>();
        for (var i = 0; i < 40; i++)
        {
            var venue = venues[rng.Next(venues.Count)];
            var start = now.Date.AddDays(rng.Next(-30, 120)).AddHours(rng.Next(17, 22));
            concerts.Add(new Concert
            {
                Id = Guid.NewGuid(),
                Title = $"{artists[rng.Next(artists.Count)].Name} Live",
                StartTime = start,
                EndTime = start.AddHours(rng.Next(2, 5)),
                VenueId = venue.Id,
                BasePrice = rng.Next(20, 120),
                TicketsSold = 0,
                CreatedAt = now.AddDays(-rng.Next(1, 40)),
                CreatedById = admin.Id,
                LastModifiedAt = now,
                LastModifiedById = admin.Id
            });
        }
        await context.Concerts.AddRangeAsync(concerts);

        var performances = new List<Performance>();
        foreach (var concert in concerts)
        {
            var lineupSize = rng.Next(1, 4);
            var chosen = artists.OrderBy(_ => rng.Next()).Take(lineupSize).ToList();
            for (var slot = 0; slot < chosen.Count; slot++)
            {
                performances.Add(new Performance
                {
                    Id = Guid.NewGuid(),
                    ArtistId = chosen[slot].Id,
                    ConcertId = concert.Id,
                    SlotOrder = slot + 1,
                    DurationMinutes = rng.Next(30, 90),
                    CreatedAt = now.AddDays(-rng.Next(1, 20)),
                    CreatedById = admin.Id,
                    LastModifiedAt = now,
                    LastModifiedById = admin.Id
                });
            }
        }
        await context.Performances.AddRangeAsync(performances);

        var statuses = Enum.GetValues<TicketStatus>();
        var tickets = new List<Ticket>();
        foreach (var concert in concerts)
        {
            var buyerCount = rng.Next(3, 20);
            var buyers = users.OrderBy(_ => rng.Next()).Take(buyerCount).ToList();
            foreach (var buyer in buyers)
            {
                var category = categories[rng.Next(categories.Count)];
                var status = statuses[rng.Next(statuses.Length)];
                tickets.Add(new Ticket
                {
                    Id = Guid.NewGuid(),
                    UserId = buyer.Id,
                    ConcertId = concert.Id,
                    TicketCategoryId = category.Id,
                    Status = status,
                    Price = Math.Round(concert.BasePrice * category.PriceMultiplier, 2),
                    SeatNumber = $"{(char)('A' + rng.Next(0, 10))}{rng.Next(1, 40)}"
                });
                if (status != TicketStatus.Cancelled)
                    concert.TicketsSold += 1;
            }
        }
        await context.Tickets.AddRangeAsync(tickets);

        await context.ApiClients.AddAsync(new ApiClient
        {
            Id = Guid.NewGuid(),
            Name = "Partner Ticketing Gateway",
            ApiKey = "concert-external-key-123",
            IsActive = true,
            RateLimitMinutes = 1
        });

        await context.SaveChangesAsync();

        Console.WriteLine($"Seeded: {users.Count} users, {artists.Count} artists, {venues.Count} venues, " +
                          $"{categories.Count} categories, {concerts.Count} concerts, " +
                          $"{performances.Count} performances, {tickets.Count} tickets.");
    }
}
