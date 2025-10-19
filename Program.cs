using FrankfurterTest;
using FrankfurterTest.Endpoints;
using FrankfurterTest.Repositories;
using FrankfurterTest.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var allowedHosts = builder.Configuration.GetValue<string>("allowedHosts")!;

#region Services

builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(config =>
{
    config.WithOrigins(allowedHosts).AllowAnyHeader().AllowAnyMethod();
}));

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
opt.UseSqlServer("name = DefaultConnection"));

builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<IRepositoryCurrencies, CurrencyRepository>();
builder.Services.AddScoped<IRepositoryExchangeRates, ExchangeRatesRepository>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<FrankfurterService>();

builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = 
    System.Text.Json.Serialization.ReferenceHandler.Preserve;
});
#endregion Services

var app = builder.Build();



#region middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.UseOutputCache();


#endregion middelware

#region endpoints
app.MapGroup("/currencies").MapCurrencies();
app.MapGroup("/rates").MapExchangeRates();
app.MapGet("/", () => "Hola");



#endregion endpoints

app.Run();
