using Taxi.Common.Models;

namespace Taxi.Web.Helpers
{
	public interface IMailHelper
	{
		Response SendMail(string to, string subject, string body);
	}
}
