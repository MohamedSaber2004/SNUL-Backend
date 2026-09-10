using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Persistance;

namespace SNUL.Shared.Persistance.Seeding
{
    public static class LandingPageSeeder
    {
        public const string AboutUsSlug = "about-us";

        public static async Task SeedAboutUsAsync(SnulDbContext db, ILogger logger)
        {
            var exists = await db.LandingPages
                .AnyAsync(x => !x.IsDeleted && x.Slug.ToLower() == AboutUsSlug);
            if (exists)
            {
                logger.LogInformation("About-us landing page already seeded, skipping");
                return;
            }

            var page = new LandingPage
            {
                Id = Guid.NewGuid(),
                Type = "Brand",
                Slug = AboutUsSlug,
                HeroTitle = "Manufacturer-direct surgical instruments",
                HeroBody = "SNUL designs and manufactures precision surgical instruments for hospitals, distributors, and clinics - with OEM private-label production and full ISO / CE documentation on every line.",
                ContentBlock = "For over three decades, SNUL has engineered precision surgical instruments trusted in operating rooms across the region.\n\nWhat we do:\n- Manufacturer-direct supply for hospitals, distributors, and clinics\n- OEM / private-label production with full documentation\n- ISO 13485 quality system with CE-marked lines\n- Export and logistics support across the Middle East and beyond\n\nQuality is documented on every line - not promised, proven."
            };
            page.MarkAsCreated("Seeder");

            await db.LandingPages.AddAsync(page);
            await db.SaveChangesAsync();
            logger.LogInformation("Seeded about-us landing page");
        }
    }
}
