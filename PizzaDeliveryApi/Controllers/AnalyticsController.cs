using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using PizzaDelivery.Application.Options;
using SQLitePCL;

namespace PizzaDeliveryApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly string _connectionString;

    public AnalyticsController(IOptions<ConnectionStringsOptions> configuration)
    {
        var options = configuration.Value;
        _connectionString = options.SqliteConnection;
    }

    [HttpGet("mostOrderedPizzaByMonth")]
    public IActionResult GetMostOrderedPizzaByMonth(int year = 2023,int month = 7)
    {
        if(year <= 2000 || year > DateTime.Now.Year) return BadRequest("Invalid year value.");
        if(month< 1 || month > 12 ) return BadRequest("Invalid month value. Month must be between 1 and 12.");

        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            var sql = @"
                    SELECT p.Id,p.Name ,COUNT(*) AS OrderCount
                    FROM Pizzas p
                    JOIN OrderItems op ON p.Id = op.PizzaId
                    JOIN Orders o ON op.OrderId = o.Id
                    WHERE strftime('%Y-%m', o.OrderDate) = 'selected_month'
                    GROUP BY p.Id, p.Name
                    ORDER BY OrderCount DESC
                    LIMIT 1
                ";

            DateTime date = new DateTime(year, month, 1);
            string formattedDate = date.ToString("yyyy-MM");
            sql = sql.Replace("'selected_month'", $"'{formattedDate}'");

            using (var command = new SqliteCommand(sql, connection))
            {
                var result = command.ExecuteReader();
                var pizzaId = string.Empty;
                var pizzaName = string.Empty;
                var orderCount = 0;

                if (result.Read())
                {
                    pizzaId = result.GetString(0);
                    pizzaName = result.GetString(1);
                    orderCount = result.GetInt32(2);
                }

                var response = new Dictionary<string, object>
                {
                    { "PizzaId", pizzaId },
                    { "PizzaName" ,pizzaName },
                    { "OrderCount", orderCount }
                };

                return Ok(response);
            }
        }
    }

    [HttpGet("usersWithHighAverageOrder")]
    public IActionResult GetUsersWithHighAverageOrder()
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            var sql = @"
                    SELECT u.Id , u.UserName, AVG(o.TotalPrice) AS AverageOrderAmount
                    FROM AspNetUsers u
                    JOIN Orders o ON u.Id = o.UserId
                    GROUP BY u.Id,u.UserName
                    HAVING COUNT(o.Id) >= 1 AND AVG(o.TotalPrice) >= (
                        SELECT AVG(TotalPrice)
                        FROM Orders)
                "
            ;

            using (var command = new SqliteCommand(sql, connection))
            {
                var result = command.ExecuteReader();
                var users = new List<Dictionary<string, object>>();

                while (result.Read())
                {
                    var user = new Dictionary<string, object>
                    {
                        { "UserId", result.GetString(0) },
                        { "UserName", result.GetString(1) },
                        { "AverageOrderAmount", result.GetDecimal(2) }
                    };
                    users.Add(user);
                }

                return Ok(users);
            }
        }
    }
}
