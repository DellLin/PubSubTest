
using System.Diagnostics;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;

const int concurrentCalls = 1000;  // 並發呼叫數
const int totalCalls = 10000;      // 總呼叫次數

var stopwatch = Stopwatch.StartNew();

var tasks = new List<Task>();
for (int i = 0; i < totalCalls; i++)
{
    if (tasks.Count >= concurrentCalls)
    {
        await Task.WhenAny(tasks);
        tasks.RemoveAll(t => t.IsCompleted);
    }

    tasks.Add(Task.Run(async () =>
    {
        try
        {
            await SendMessage();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"錯誤: {ex.Message}");
        }
    }));
}

await Task.WhenAll(tasks);

stopwatch.Stop();

Console.WriteLine($"測試完成。總耗時: {stopwatch.ElapsedMilliseconds} ms");
Console.WriteLine($"平均每次呼叫耗時: {stopwatch.ElapsedMilliseconds / (double)totalCalls} ms");
Console.WriteLine($"每秒處理請求數: {totalCalls / (stopwatch.ElapsedMilliseconds / 1000.0):F2}");



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
        ByteString data = ByteString.CopyFromUtf8(message);

        // 發布消息
        string messageId = await publisher.PublishAsync(data);

        // Console.WriteLine($"消息已發布。Message ID: {messageId}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"發布消息時發生錯誤: {ex.Message}");
    }
}