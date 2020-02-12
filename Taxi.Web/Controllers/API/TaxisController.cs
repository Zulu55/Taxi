using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Taxi.Web.Data;
using Taxi.Web.Data.Entities;
using Taxi.Web.Helpers;

namespace Taxi.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxisController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConverterHelper _converterHelper;

        public TaxisController(
            DataContext context,
            IConverterHelper converterHelper)
        {
            _context = context;
            _converterHelper = converterHelper;
        }

        [HttpGet("{plaque}")]
        public async Task<IActionResult> GetTaxiEntity([FromRoute] string plaque)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            plaque = plaque.ToUpper();
            TaxiEntity taxiEntity = await _context.Taxis
                .Include(t => t.User)
                .Include(t => t.Trips)
                .ThenInclude(t => t.TripDetails)
                .Include(t => t.Trips)
                .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(t => t.Plaque == plaque);

            if (taxiEntity == null)
            {
                _context.Taxis.Add(new TaxiEntity { Plaque = plaque });
                await _context.SaveChangesAsync();
                taxiEntity = await _context.Taxis.FirstOrDefaultAsync(t => t.Plaque == plaque);
            }

            return Ok(_converterHelper.ToTaxiResponse(taxiEntity));
        }
    }
}
