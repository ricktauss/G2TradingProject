namespace OData
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            // if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            
            app.MapControllers();
            
            
            // Start a new thread for the OData client code
            // Client code 
            /*await Task.Run(async () =>
            {
                // Delay for 2 seconds
                await Task.Delay(2000);

                // Call ODataClient
                var odataClient = new ODataClient();

                // Example usage: Get all products
                var products = await odataClient.GetProductsAsync();
                foreach (var product in products)
                {
                    Console.WriteLine($"Product: {product.Name}, Price: {product.Price}");
                }
            });*/
            
            
            // https://localhost:5108/Products?$filter=Price%20gt%2020
            
            // Run server
            await app.RunAsync();
        }
    }
}