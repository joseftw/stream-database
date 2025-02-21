using JOS.ExampleWeb;
using JOS.StreamDatabase.Database;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddScoped<RealEstateQueryHandler>();
builder.Services.AddScoped<RealEstateImageQueryHandler>();
var app = builder.Build();

app.MapGet("/real-estate/{realEstateId:guid}", RealEstateEndpoint.Handle).WithName("ReadRealEstate");

app.MapGet("/real-estate/{realEstateId:guid}/images/{imageId:guid}", RealEstateImageEndpoint.Handle)
   .WithName("ReadRealEstateImage");
app.Run();
