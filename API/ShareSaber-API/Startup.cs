using System.Text;
using ShareSaber_API.Models;
using ShareSaber_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace ShareSaber_API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ShareSaberDatabaseSettings>(
                Configuration.GetSection(nameof(ShareSaberDatabaseSettings)));
            services.Configure<DiscordAuthenticationSettings>(
                Configuration.GetSection(nameof(DiscordAuthenticationSettings)));
            services.Configure<JWT>(
                Configuration.GetSection(nameof(JWT)));

            services.AddSingleton<IShareSaberDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<ShareSaberDatabaseSettings>>().Value);
            services.AddSingleton<IDiscordAuthenticationSettings>(sp =>
                sp.GetRequiredService<IOptions<DiscordAuthenticationSettings>>().Value);
            services.AddSingleton<IJWT>(sp =>
                sp.GetRequiredService<IOptions<JWT>>().Value);

            services.AddSingleton<SchemaService>();
            services.AddSingleton<UserService>();
            services.AddSingleton<JWTService>();
            services.AddSingleton<DiscordService>();
            services.AddSingleton<FileService>();

            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins, opt =>
                {
                    opt.WithOrigins("https://localhost:44380",
                        "https://localhost:44333",
                        "https://sharesaber.com")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["JWT:Issuer"],
                        ValidAudience = Configuration["JWT:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"]))
                    };
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthentication();
            app.UseAuthorization();

            var path = Path.Combine(Directory.GetCurrentDirectory(), "Files");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("ShareSaber OK!");
                });
                endpoints.MapControllers();
            });
        }
    }
}
