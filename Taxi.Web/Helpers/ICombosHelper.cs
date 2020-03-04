using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Taxi.Web.Helpers
{
    public interface ICombosHelper
    {
        IEnumerable<SelectListItem> GetComboRoles();
    }
}
