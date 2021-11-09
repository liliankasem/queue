using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.ServiceBus;

namespace QueueApp
{
  public class Program
  {
    static readonly HttpClient client = new HttpClient();

    public static async Task Main(string[] args)
    {
      var msgText = "helloyou";
      var numOfMessages = 10000;

      Console.WriteLine($"Starting to queue {numOfMessages} messages...");

      var blobQueueName = "myqueue-items";
      var sbQueueName = "myqueue";
      var blobConnectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
      var sbConnectionString = Environment.GetEnvironmentVariable("AZURE_SERVICE_BUS_CONNECTION_STRING");
      var functionUrl = Environment.GetEnvironmentVariable("FUNCTION_URL") ?? "https://www.google.com";

      // Blob Queue
      var blobQueue = new Azure.Storage.Queues.QueueClient(blobConnectionString, blobQueueName);

      // Service Bus Queue
      var sbQueue = new Microsoft.Azure.ServiceBus.QueueClient(sbConnectionString, sbQueueName);
      var sbMessage = new Message(Encoding.UTF8.GetBytes(msgText));

      var count = 0;
      while (count < numOfMessages)
      {
        Console.WriteLine($"{count} {msgText}");
        await blobQueue.SendMessageAsync(msgText);
        await sbQueue.SendAsync(sbMessage);
        await Ping(functionUrl);
        count++;
      }

      await sbQueue.CloseAsync();
    }

    public static async Task Ping(string url)
    {
      HttpResponseMessage response = await client.GetAsync(url);
      response.EnsureSuccessStatusCode();
      string responseBody = await response.Content.ReadAsStringAsync();
      Console.WriteLine(responseBody);
    }
  }
}
