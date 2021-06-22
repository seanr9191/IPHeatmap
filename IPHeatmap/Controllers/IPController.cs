using IPHeatmap.Data;
using IPHeatmap.Data.Models;
using IPHeatmap.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPHeatmap.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IPController : ControllerBase
    {

        private readonly ILogger<IPController> _logger;

        public IPController(ILogger<IPController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        /* 
         * /api/ip?[page=][pageSize=][lowerLeftLat=][lowerLeftLong=][upperRightLat=][upperRightLong=]
         * 
         * This endpoint returns all IP addressed bounded by the optional coordinates provided.
         * If a bounding box is provided, all 4 bounding parameters must be sent or they will be ignored.
         * The default page size is 1000.
         * The default page is 1.
         */
        public async Task<List<HeatMapIP>> GetIPAddresses(
            int? page, 
            int? pageSize, 
            decimal? lowerLeftLat, 
            decimal? lowerLeftLong, 
            decimal? upperRightLat, 
            decimal? upperRightLong
        )
        {
            if (page == null)
                page = 1;
            if (pageSize == null)
                pageSize = 1000;

            //Initialize the query
            using var context = new IPContext();
            IQueryable<HeatMapIP> query = context.IPAddresses.AsQueryable()
                .OrderBy(ip => ip.ID)
                .GroupBy(ip => new { ip.Latitude, ip.Longitude })
                .Select(ipGroup => new HeatMapIP
                {
                    Latitude = ipGroup.Key.Latitude,
                    Longitude = ipGroup.Key.Longitude,
                    Count = ipGroup.Count()
                });
                
            //Bound the results if applicable
            if (lowerLeftLat != null &&
                lowerLeftLong != null &&
                upperRightLat != null &&
                upperRightLong != null)
            {
                query = query.Where(
                    ip => ip.Latitude >= lowerLeftLat.Value &&
                    ip.Latitude <= upperRightLat.Value &&
                    ip.Longitude >= lowerLeftLong.Value &&
                    ip.Longitude <= upperRightLong.Value
                );
            }

            //Paginate the results
            query = query
                .Skip(pageSize.Value * (page.Value - 1))
                .Take(pageSize.Value);

            return await query.ToListAsync();
        }


    }
}
