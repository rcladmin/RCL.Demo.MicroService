using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using RCL.Core.Api.Authorization;
using RCL.Demo.Data;

namespace RCL.Demo.MicroService.Functions
{
    public class BookingService
    {
        private readonly BookingDbContext _db;
        private readonly IApiAuthorization _auth;

        public BookingService(BookingDbContext db,
            IAuthorizationFactory authFactory)
        {
            _db = db;
            _auth = authFactory.Create(AuthType.ClientCredentials);
        }

        [Function("Booking_GetByUserId")]
        public async Task<HttpResponseData> RunGetByUserId([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/demo/booking/userid/{userid}/getall")] HttpRequestData req,
            string userid)
        {
            try
            {
                bool isAuthorized = _auth.IsAuthorized(req, ClientIds:ApiAuthorizationKeys.ClientIds);

                if (isAuthorized == false)
                {
                    var response = req.CreateResponse(HttpStatusCode.Unauthorized);
                    response.Headers.Add("Content-Type", "text/plain ; charset=utf-8");
                    return response;
                }

                List<Booking> bookings = await _db.Bookings
                    .Where(w => w.UserId == userid)
                    .AsNoTracking()
                    .ToListAsync();

                if(bookings?.Count > 0)
                {
                    var response = req.CreateResponse(HttpStatusCode.OK);
                    response.Headers.Add("Content-Type", "application/json ; charset=utf-8");
                    response.WriteString(JsonSerializer.Serialize(bookings));
                    return response;
                }
                else
                {
                    var response = req.CreateResponse(HttpStatusCode.NotFound);
                    response.Headers.Add("Content-Type", "text/plain ; charset=utf-8");
                    response.WriteString($"No bookings were found.");
                    return response;
                }
            }
            catch (Exception ex)
            {
                var response = req.CreateResponse(HttpStatusCode.BadRequest);
                response.Headers.Add("Content-Type", "text/plain ; charset=utf-8");
                response.WriteString($"{ex.Message}");
                return response;
            }
        }
    }
}
