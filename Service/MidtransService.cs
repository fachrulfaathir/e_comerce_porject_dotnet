using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class MidtransService
{
    private readonly HttpClient _httpClient;
    private readonly string _serverKey;

    public MidtransService(IConfiguration config)
    {
        _serverKey = "SB-Mid-server-graBzJ4LkBuCH14lCy8DD5mW";
        _httpClient = new HttpClient();
    }

    public async Task<string> CreatePaymentAsync(Order order, List<OrderDetail> orderDetails)
    {
        var transactionDetails = new
        {
            order_id = "TRX" + order.Id,
            gross_amount = orderDetails.Sum(d => d.UnitPrice * d.Quantity)
        };

        var items = orderDetails.Select(d => new
        {
            id = d.BookId.ToString(),
            price = d.UnitPrice,
            quantity = d.Quantity,
            name = d.Book?.BookName ?? "Item"
        }).ToList();

        var customer = new
        {
            first_name = order.Name,
            email = order.Email,
            phone = order.MobileNumber
        };

        var payload = new
        {
            transaction_details = transactionDetails,
            customer_details = customer,
            item_details = items
        };

        var baseUrl = "https://app.sandbox.midtrans.com/snap/v1/transactions";

        var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes(_serverKey+ ":"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(baseUrl, content);
        var result = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Midtrans Error: {response.StatusCode} - {result}");
        }

        return result;
    }

}
