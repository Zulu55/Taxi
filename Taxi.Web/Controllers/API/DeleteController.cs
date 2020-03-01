using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taxi.Web.Data;
using Taxi.Web.Data.Entities;

namespace Taxi.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteController : ControllerBase
    {
        private readonly DataContext _context;

        public DeleteController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Delete
        [HttpGet]
        public IEnumerable<TaxiEntity> GetTaxis()
        {
            return _context.Taxis;
        }

        // GET: api/Delete/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaxiEntity([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var taxiEntity = await _context.Taxis.FindAsync(id);

            if (taxiEntity == null)
            {
                return NotFound();
            }

            return Ok(taxiEntity);
        }

        // PUT: api/Delete/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaxiEntity([FromRoute] int id, [FromBody] TaxiEntity taxiEntity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != taxiEntity.Id)
            {
                return BadRequest();
            }

            _context.Entry(taxiEntity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaxiEntityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Delete
        [HttpPost]
        public async Task<IActionResult> PostTaxiEntity([FromBody] TaxiEntity taxiEntity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Taxis.Add(taxiEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTaxiEntity", new { id = taxiEntity.Id }, taxiEntity);
        }

        // DELETE: api/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaxiEntity([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var taxiEntity = await _context.Taxis.FindAsync(id);
            if (taxiEntity == null)
            {
                return NotFound();
            }

            _context.Taxis.Remove(taxiEntity);
            await _context.SaveChangesAsync();

            return Ok(taxiEntity);
        }

        private bool TaxiEntityExists(int id)
        {
            return _context.Taxis.Any(e => e.Id == id);
        }
    }
}