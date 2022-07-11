using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Meeting.Core.GrpcClient.App_Infrastructure.Tests.Factories
{
    public static class ConfigurationFactory
    {
        public static IConfiguration GetConfiguration()
        {
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("testsettings.json", false, true);
            //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateAudience = true,
            //        ValidAudience = builder.Configuration.GetValue<string>("JwtAudience"),
            //        ValidateIssuer = true,
            //        ValidIssuer = builder.Configuration.GetValue<string>("JwtIssuer"),
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtKey"))),
            //        ValidateLifetime = true,
            //        ClockSkew = TimeSpan.FromMinutes(5)
            //    };
            //});
            builder.Build();
            return builder.Configuration;
        }
    }
}
