
using LibMgt.LibDbContext;
using LibMgt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace LibMgt
{
    public static class DependencyInjection
    {
        public static IServiceCollection Configure(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddDbContext<LibraryDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("connection"));
            });
            services.AddIdentity<User, Role>(options =>
            {
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultProvider;
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
                options.Tokens.ChangeEmailTokenProvider = TokenOptions.DefaultEmailProvider;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.Lockout.MaxFailedAccessAttempts = 5;

            }).AddEntityFrameworkStores<LibraryDbContext>().AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultEmailProvider);
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description="JWT Authentication header using bearer scheme",
                    Name = "Authorization",
                    In=ParameterLocation.Header,
                    Type=SecuritySchemeType.ApiKey,

                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference=new OpenApiReference
                            {
                                Id="Bearer",
                                Type=ReferenceType.SecurityScheme
                            },

                        },new List<string>()
                    }
                });
            });
            return services;
        }
    }
}
