using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPHeatmap.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IPController : ControllerBase
    {

        private readonly ILogger<IPController> _logger;

        public IPController(ILogger<IPController> logger)
        {
            _logger = logger;
        }


    }
}
