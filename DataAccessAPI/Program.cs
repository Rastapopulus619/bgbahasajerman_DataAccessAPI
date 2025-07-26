using bgbahasajerman_DataAccessLibrary.DataAccess;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Register the mock repository for DI
builder.Services.AddScoped<IMockRepository, MockRepository>();
// Register data access services (uses MySqlConnectionFactory and QueryExecutor)
builder.Services.AddDataAccessServices(builder.Configuration);



builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();