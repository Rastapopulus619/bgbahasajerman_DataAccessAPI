using bgbahasajerman_DataAccessLibrary.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IMockRepository, MockRepository>();
builder.Services.AddDataAccessServices(builder.Configuration);

builder.Services.AddControllers();

// Swagger & OpenAPI
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// IMPORTANT: UseStaticFiles must come before UseSwagger
app.UseStaticFiles();

// Set up Swagger for both Development and Production
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "bgbahasajerman API v1");
    options.RoutePrefix = "swagger";
    // Add this to ensure static files are served correctly
    options.InjectStylesheet("/swagger-ui/custom.css");
});

// Optional HTTPS redirection — only if it's available
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "API is running. Swagger lives at /swagger/index.html");

app.Run();