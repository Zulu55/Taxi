using Taxi.Common.Models;
using System.Threading.Tasks;

namespace Taxi.Common.Services
{
    public interface IApiService
    {
        Task<Response> GetTaxiAsync(string plaque, string urlBase, string servicePrefix, string controller);
    }
}
