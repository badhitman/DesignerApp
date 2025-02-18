
using DbcLib;

namespace ToolsMauiAppMigration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.Services.AddOptions();
            builder.Services.AddDbContextFactory<ToolsAppContext>(opt =>
            {
#if DEBUG
                opt.EnableSensitiveDataLogging(true);
                opt.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
#endif
            });

            WebApplication app = builder.Build();
            app.Run();
        }
    }
}
