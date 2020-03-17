using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Taxi.Common.Models;
using Taxi.Web.Data;
using Taxi.Web.Data.Entities;
using Taxi.Web.Helpers;

namespace Taxi.Web.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;

        public TripsController(DataContext context, IUserHelper userHelper, IConverterHelper converterHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTripEntity([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tripEntity = await _context.Trips
                .Include(t => t.TripDetails)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tripEntity == null)
            {
                return NotFound();
            }

            _context.Trips.Remove(tripEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTripEntity([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TripEntity tripEntity = await _context.Trips
                .Include(t => t.TripDetails)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tripEntity == null)
            {
                return BadRequest("Trip not found.");
            }

            return Ok(_converterHelper.ToTripResponse(tripEntity));
        }

        [HttpPost]
        public async Task<IActionResult> PostTripEntity([FromBody] TripRequest tripRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserEntity userEntity = await _userHelper.GetUserAsync(tripRequest.UserId);
            if (userEntity == null)
            {
                return BadRequest("User doesn't exists.");
            }

            TaxiEntity taxiEntity = await _context.Taxis.FirstOrDefaultAsync(t => t.Plaque == tripRequest.Plaque);
            if (taxiEntity == null)
            {
                _context.Taxis.Add(new TaxiEntity { Plaque = tripRequest.Plaque.ToUpper() });
                await _context.SaveChangesAsync();
                taxiEntity = await _context.Taxis.FirstOrDefaultAsync(t => t.Plaque == tripRequest.Plaque);
            }

            TripEntity tripEntity = new TripEntity
            {
                Source = tripRequest.Address,
                SourceLatitude = tripRequest.Latitude,
                SourceLongitude = tripRequest.Longitude,
                StartDate = DateTime.UtcNow,
                Taxi = taxiEntity,
                TripDetails = new List<TripDetailEntity>
                {
                    new TripDetailEntity
                    {
                        Date = DateTime.UtcNow,
                        Latitude = tripRequest.Latitude,
                        Longitude = tripRequest.Longitude
                    }
                },
                User = userEntity,
            };

            _context.Trips.Add(tripEntity);
            await _context.SaveChangesAsync();
            return Ok(_converterHelper.ToTripResponse(tripEntity));
        }

        [HttpPost]
        [Route("CompleteTrip")]
        public async Task<IActionResult> CompleteTrip([FromBody] CompleteTripRequest completeTripRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TripEntity trip = await _context.Trips
                .Include(t => t.TripDetails)
                .FirstOrDefaultAsync(t => t.Id == completeTripRequest.TripId);
            if (trip == null)
            {
                return BadRequest("Trip not found.");
            }

            trip.EndDate = DateTime.UtcNow;
            trip.Qualification = completeTripRequest.Qualification;
            trip.Remarks = completeTripRequest.Remarks;
            trip.Target = completeTripRequest.Target;
            trip.TargetLatitude = completeTripRequest.TargetLatitude;
            trip.TargetLongitude = completeTripRequest.TargetLongitude;
            trip.TripDetails.Add(new TripDetailEntity
            {
                Date = DateTime.UtcNow,
                Latitude = completeTripRequest.TargetLatitude,
                Longitude = completeTripRequest.TargetLongitude
            });

            _context.Trips.Update(trip);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
