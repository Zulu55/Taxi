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
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
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
                return BadRequest("Error001");
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
                        Address = tripRequest.Address,
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
        [Route("AddTripDetail")]
        public async Task<IActionResult> AddTripDetail([FromBody] TripDetailRequest tripDetailRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TripEntity trip = await _context.Trips
                .Include(t => t.TripDetails)
                .FirstOrDefaultAsync(t => t.Id == tripDetailRequest.TripId);
            if (trip == null)
            {
                return BadRequest("Error002");
            }

            if (trip.TripDetails == null)
            {
                trip.TripDetails = new List<TripDetailEntity>();
            }

            trip.TripDetails.Add(new TripDetailEntity
            {
                Address = tripDetailRequest.Address,
                Date = DateTime.UtcNow,
                Latitude = tripDetailRequest.Latitude,
                Longitude = tripDetailRequest.Longitude
            });

            _context.Trips.Update(trip);
            await _context.SaveChangesAsync();
            return Ok(_converterHelper.ToTripResponse(trip));
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
                return BadRequest("Error002");
            }

            trip.EndDate = DateTime.UtcNow;
            trip.Qualification = completeTripRequest.Qualification;
            trip.Remarks = completeTripRequest.Remarks;
            trip.Target = completeTripRequest.Target;
            trip.TargetLatitude = completeTripRequest.TargetLatitude;
            trip.TargetLongitude = completeTripRequest.TargetLongitude;
            trip.TripDetails.Add(new TripDetailEntity
            {
                 Address = completeTripRequest.Target,
                 Date = DateTime.UtcNow,
                 Latitude = completeTripRequest.TargetLatitude,
                 Longitude = completeTripRequest.TargetLongitude
            });

            _context.Trips.Update(trip);
            await _context.SaveChangesAsync();
            return Ok(_converterHelper.ToTripResponse(trip));
        }
    }
}
