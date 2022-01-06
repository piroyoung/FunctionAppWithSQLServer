using FunctionAppWithSQLServer.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;

namespace FunctionAppWithSQLServer
{
    public static class Function1
    {
        private static readonly string connectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");

        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name ??= data?.name;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                RowRepository repo = SQLServerRowRepository.OfConnection(conn);

                try
                {
                    string value = repo.GetFirstValue(name);
                    string responseMessage = $"key: {name}, value: {value}";
                    return new OkObjectResult(responseMessage);
                }
                catch (SqlException)
                {
                    return new InternalServerErrorResult();
                }
                catch (Exception ex)
                {
                    return new NotFoundObjectResult(ex.Message);
                }
            }
        }
    }
}
