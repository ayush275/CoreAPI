using Microsoft.EntityFrameworkCore;
using WebAPI.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>

{
    options.AddDefaultPolicy(

        policy =>
        {
            policy.WithOrigins("*").WithMethods("*").WithHeaders("*");
 
        });
});

// Add services to the container.

builder.Services.AddControllers();
string connString = builder.Configuration.GetConnectionString("dbconn");
builder.Services.AddDbContext<MyDbContext>(options => options.UseMySql(connString, ServerVersion.AutoDetect(connString)));
builder.Services.AddDbContext<MyDbContext>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
