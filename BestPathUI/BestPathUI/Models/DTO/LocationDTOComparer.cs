using System;
using System.Collections.Generic;
using System.Text;

namespace Models.DTO
{
    public class LocationDTOComparer : IEqualityComparer<LocationDTO>
    {
        public bool Equals(LocationDTO o1, LocationDTO o2)
        {
            if (o1 == null || o2 == null)
                return false;
            if (o1.lat != o2.lat || o1.lng != o2.lng)
                return false;
            return true;
        }
        public int GetHashCode(LocationDTO o)
        {
            return o.lat.GetHashCode() * 17 + o.lng.GetHashCode();
        }
    }
}
