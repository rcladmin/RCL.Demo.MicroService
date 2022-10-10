#nullable disable

using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using RCL.Core.Api.Authorization;
using RCL.Demo.Data;

namespace RCL.Demo.MicroService.Functions
{
    public class Webhook
    {
        private readonly BookingDbContext _db;
        private readonly IApiAuthorization _auth;

        public Webhook(BookingDbContext db,
            IAuthorizationFactory authFactory)
        {
            _db = db;
            _auth = authFactory.Create(AuthType.SecretKey);
        }

        [Function("Webhook")]
        public async Task<MultipleOutput> RunWebhook([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/demo/booking/receiver")] HttpRequestData req)
        {
            try
            {
                bool isAuthorized = _auth.IsAuthorized(req, SecretKeyName:ApiAuthorizationKeys.SecretKeyName, SecretKeyValue:ApiAuthorizationKeys.SecretKeyValue);

                if (isAuthorized == false)
                {
                    return new MultipleOutput
                    {
                        resp = req.CreateResponse(HttpStatusCode.Unauthorized)
                    };
                }

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                Payment payment = JsonSerializer.Deserialize<Payment>(requestBody);

                if (string.IsNullOrEmpty(payment?.transactionId))
                {
                    var response = req.CreateResponse(HttpStatusCode.BadRequest);
                    response.Headers.Add("Content-Type", "text/plain ; charset=utf-8");
                    response.WriteString($"Could not deserialize Payment object");

                    return new MultipleOutput
                    {
                        resp = response
                    };
                }

                return new MultipleOutput()
                {
                    queueCollector = payment,
                    resp = req.CreateResponse(HttpStatusCode.OK)
                };
            }
            catch (Exception ex)
            {
                var response = req.CreateResponse(HttpStatusCode.BadRequest);
                response.Headers.Add("Content-Type", "text/plain ; charset=utf-8");
                response.WriteString($"{ex.Message}");

                return new MultipleOutput
                {
                    resp = response
                };
            }
        }

        [Function("Webhook_HandleEvent_V1")]
        public async Task RunProcessQueue([QueueTrigger("demowebhookqueue", Connection = "AzureWebJobsStorage")] Payment payment)
        {
            try
            {
                Booking booking = new Booking
                {
                    BeginDate = DateTime.Now.AddDays(2),
                    EndDate = DateTime.Now.AddDays(4),
                    PaymentId = payment.transactionId,
                    RoomNumber = Guid.NewGuid().ToString(),
                    UserId = payment.userId
                };

                _db.Bookings.Add(booking);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
            }
        }
    }

    public class MultipleOutput
    {
        [QueueOutput("demowebhookqueue", Connection = "AzureWebJobsStorage")]
        public Payment queueCollector { get; set; }
        public HttpResponseData resp { get; set; }
    }
}
