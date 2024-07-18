using WebData;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TestDbContext>();
var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/ReceivePubSubMessage", async (Msg msg, TestDbContext dbContext) =>
{
    var pubSubTest = new PubSubTest { Message = msg.Message };
    dbContext.PubSubTest.Add(pubSubTest);
    await dbContext.SaveChangesAsync();

    return Results.Ok(new { Message = "Message received" });
}).WithName("ReceivePubSubMessage")
.WithOpenApi();

app.Run();

class Msg

{
    public string Message { get; set; }
}