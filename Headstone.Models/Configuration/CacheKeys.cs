using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Configuration
{
    public class CacheKeys
    {
        // Geolocations
        public static readonly string ActiveCities = "cities_active";
        public static readonly string ActiveCityDistricts = "districts_for_city_";

        // Tax offices
        public static readonly string ActiveTaxOffices = "taxoffices_active";

        // Content entities
        public static readonly string ActiveContentEntities = "contententities_active";
        public static readonly string ActiveContentEntitiesFull = "contententities_active_full";

    }
}
