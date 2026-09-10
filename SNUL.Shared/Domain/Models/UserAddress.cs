using SNUL.Shared.Common.Classes;

namespace SNUL.Shared.Domain.Models
{
    public class UserAddress : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;

        public Guid CountryId { get; set; }
        public virtual Country Country { get; set; } = null!;

        public Guid CityId { get; set; }
        public virtual City City { get; set; } = null!;

        public Guid ZoneId { get; set; }
        public virtual Zone Zone { get; set; } = null!;

        public string Street { get; set; } = null!;
        public string? Building { get; set; }
        public string? Floor { get; set; }
        public string? Apartment { get; set; }
        public bool IsDefault { get; set; }

        public static UserAddress Create(
            Guid userId,
            Guid countryId,
            Guid cityId,
            Guid zoneId,
            string street,
            string? building,
            string? floor,
            string? apartment,
            string createdBy,
            bool isDefault = false)
        {
            var address = new UserAddress
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CountryId = countryId,
                CityId = cityId,
                ZoneId = zoneId,
                Street = street,
                Building = building,
                Floor = floor,
                Apartment = apartment,
                IsDefault = isDefault
            };
            address.MarkAsCreated(createdBy);
            return address;
        }

        public void Update(
            Guid? countryId,
            Guid? cityId,
            Guid? zoneId,
            string? street,
            string? building,
            string? floor,
            string? apartment,
            string updatedBy,
            bool? isDefault = null)
        {
            if (countryId.HasValue) CountryId = countryId.Value;
            if (cityId.HasValue) CityId = cityId.Value;
            if (zoneId.HasValue) ZoneId = zoneId.Value;
            if (!string.IsNullOrWhiteSpace(street)) Street = street.Trim();
            if (building != null) Building = building;
            if (floor != null) Floor = floor;
            if (apartment != null) Apartment = apartment;
            if (isDefault.HasValue) IsDefault = isDefault.Value;
            MarkAsUpdated(updatedBy);
        }
    }
}
