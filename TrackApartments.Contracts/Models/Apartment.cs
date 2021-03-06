using System;
using System.Collections.Generic;
using System.Linq;
using TrackApartments.Contracts.Enums;

namespace TrackApartments.Contracts.Models
{
    public class Apartment
    {
        public Guid UniqueId { get; set; }

        public string Address { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public List<string> Phones { get; set; } = new List<string>(2);

        public bool IsCreatedByOwner { get; set; }

        public float Price { get; set; }

        public int Rooms { get; set; }

        public Uri Uri { get; set; }

        public string SourceId { get; set; }

        public DataSource Source { get; set; }

        public override string ToString()
        {
            return $"{Address}, {Phones.FirstOrDefault()}, ${Price}, R:{Rooms}, {Uri}";
        }

        public override bool Equals(Object other)
        {
            if (other == null || GetType() != other.GetType())
            {
                return false;
            }

            var item = (Apartment)other;
            return Address == item.Address &&
                   (Uri == item.Uri || (Price.Equals(item.Price) && Rooms == item.Rooms));
        }

        public override int GetHashCode()
        {
            return Address.GetHashCode() ^
                Uri.GetHashCode() ^
                Rooms.GetHashCode() ^
                Price.GetHashCode();
        }
    }
}
