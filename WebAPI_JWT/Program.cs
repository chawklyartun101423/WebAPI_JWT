using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

    builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(options =>
{    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {  
     // Description = "Standard Authorization header using the bearer scheme (\"bearer{token}\")",Description is needed to add
        In = ParameterLocation.Header,
        Name= "Authorization",
        Type =SecuritySchemeType.ApiKey
    }); 
    options.OperationFilter<SecurityRequirementsOperationFilter>();  //Matt Frear
});

/*builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
    ValidateIssuer = false,
    ValidateAudience = false
};
}); //needed to include in dotnet 6 */
// builder.Services.AddAuthentication().AddJwtBearer();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer").AddJwtBearer(); //Default AddAuthentication
//builder.Services.AddAuthentication().AddJwtBearer(
//options => options.TokenValidationParameters = new TokenValidationParameters
//{
//    ValidateIssuerSigningKey = true,
//    ValidateAudience = false,
//    ValidateIssuer = false,
//    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
// builder.Configuration.GetSection("Authentication:Schemes:Bearer:SigningKeys:0:Value").Value!))
//});


var app = builder.Build();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
    app.UseSwagger();
    app.UseSwaggerUI();
    }

app.UseHttpsRedirection();

//app.UseAuthentication(); //make sure UseAuthentication is the line above UseAuthorization in dotnet 6 version

app.UseAuthorization();

app.MapControllers();

app.Run();
