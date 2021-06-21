using System;

namespace IPHeatmap.Data.Models
{
    public class IPAddress
    {
        public int ID { get; set; }
        public string Network { get; set; }
        public int GeonameId { get; set; }
        public int RegisteredCountryGeonameId { get; set; }
        public int RepresentedCountryGeonameId { get; set; }
        public bool IsAnonymousProxy { get; set; }
        public bool IsSatelliteProvider { get; set; }
        public string PostalCode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int AccuracyRadius { get; set; }
    }
}
