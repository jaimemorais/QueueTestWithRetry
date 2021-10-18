
using System.Text;

var clientId = DateTime.Now.Millisecond;

int delay = new Random().Next(700, 1500);

using HttpClient httpClient = new HttpClient();

for (int i = 1; i <= 20; i++)
{
    var msg = $"{clientId}-msg-{i}";
    
    Console.Write($"Sending msg = '{msg}' ... ");

    var content = new StringContent(msg, Encoding.UTF8, "text/plain");
    var response = httpClient.PostAsync("http://localhost:5240/api/values", content).GetAwaiter().GetResult();
    
    Console.WriteLine($" > Response = {response.StatusCode}");

    Task.Delay(delay).GetAwaiter().GetResult();
}

