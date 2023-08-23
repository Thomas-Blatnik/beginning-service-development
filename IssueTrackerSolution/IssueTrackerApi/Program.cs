using IssueTrackerApi;
using IssueTrackerApi.Services;
using Marten;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dataConnectionString = builder.Configuration.GetConnectionString("data") ?? throw new Exception("Need A Connection String");
builder.Services.AddMarten(options =>
{
    options.Connection(dataConnectionString);
    options.AutoCreateSchemaObjects = Weasel.Core.AutoCreate.All;
});

var businessClockAddress = builder.Configuration.GetValue<string>("business-clock-api") ?? throw new Exception("Need an address for the business clock");
builder.Services.AddHttpClient<BusinessClockApiAdapter>(client =>
{
    client.BaseAddress = new Uri(businessClockAddress);
}).AddPolicyHandler(SrePolicies.GetDefaultRetryPolicy())
  .AddPolicyHandler(SrePolicies.GetDefaultCircuitBreaker());

builder.Services.AddSingleton<ISystemTime, SystemTime>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
