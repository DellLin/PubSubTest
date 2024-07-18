using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/PubSubTest", async () =>
{
    await SendMessage();
})
.WithName("PubSubTest")
.WithOpenApi();
ThreadPool.SetMinThreads(80, 80);
app.Run();


async Task SendMessage()
{
    // 設置您的 Google Cloud 項目 ID 和 Pub/Sub 主題名稱
    string projectId = "dell-demo-project-03";
    string topicId = "pubsub-test";

    // 創建發布者客戶端
    PublisherClient publisher = await PublisherClient.CreateAsync(new TopicName(projectId, topicId));

    // 要發布的消息
    string message = "Hello, Google Cloud Pub/Sub!";

    try
    {
        // 將消息轉換為 ByteString
        ByteString data = ByteString.CopyFromUtf8(JsonConvert.SerializeObject(new { message }));
        PubsubMessage pubsubMessage = new PubsubMessage
        {
            Data = data,
            Attributes = { { "Content-Type", "application/json" } }

        };
        // 發布消息
        string messageId = await publisher.PublishAsync(pubsubMessage);

        Console.WriteLine($"消息已發布。Message ID: {messageId}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"發布消息時發生錯誤: {ex.Message}");
        throw;
    }
}
