
using Microsoft.Ajax.Utilities;

namespace EU.CqrXs.Srv.Svc.Swashbuckle
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Area23.At.Framework.Core.Cache.PersistInCache.SetRedis(Area23.At.Framework.Core.Cache.PersistType.RedisValkey);

            // Add services to the container.            
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            var app = builder.Build();
            Area23.At.Framework.Core.Cache.PersistInCache.SetRedis(Area23.At.Framework.Core.Cache.PersistType.RedisValkey);


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
