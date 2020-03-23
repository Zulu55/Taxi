using System.Collections.Generic;
using Taxi.Common.Models;
using Taxi.Web.Data.Entities;

namespace Taxi.Web.Helpers
{
    public interface IConverterHelper
    {
        List<UserGroupDetailResponse> ToUserGroupResponse(List<UserGroupDetailEntity> users);

        List<TripResponseWithTaxi> ToTripResponse(List<TripEntity> tripEntities);

        TaxiResponse ToTaxiResponse(TaxiEntity taxiEntity);

        TripResponse ToTripResponse(TripEntity tripEntity);

        UserResponse ToUserResponse(UserEntity user);
    }
}
