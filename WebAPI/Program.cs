using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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

builder.Services.AddControllers();
string connString = builder.Configuration.GetConnectionString("dbconn");
builder.Services.AddDbContext<MyDbContext>(options => options.UseMySql(connString, ServerVersion.AutoDetect(connString)));
builder.Services.AddDbContext<MyDbContext>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],   
        ValidAudience = builder.Configuration["Jwt:Audiance"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // Replace actual key
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
