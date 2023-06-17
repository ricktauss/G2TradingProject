
using IEGEasyCreditcardService.Services;
using System.Reflection;

namespace IEGEasyCreditcardService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            builder.Services.AddScoped<ICreditcardValidator, CreditcardValidator>();
            builder.Services.AddSingleton<LoadBalancerService>();
            builder.Services.AddSingleton<ICustomLogger, CustomLoggerService>();

            var app = builder.Build();

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
        }
    }
}