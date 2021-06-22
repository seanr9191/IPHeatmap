using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPHeatmap.Data.ViewModels
{
    public class HeatMapIP
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int Count { get; set; }
    }
}
