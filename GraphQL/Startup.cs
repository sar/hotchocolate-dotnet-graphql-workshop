namespace GraphQL
{
    using GraphQL.Data;
    using GraphQL.Mutations;
    using GraphQL.Providers;
    using GraphQL.Resolvers;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private ILogger<Startup> _logger;

        public Startup(
            IWebHostEnvironment env,
            IConfiguration configuration)
        {
            this._env = env;
            Configuration = configuration;
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile($"appsettings.{env.EnvironmentName.ToLower()}.json", false, true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public static IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkNpgsql()
                .AddDbContextFactory<ApiContext>();

            services.AddGraphQLServer()
                .AddQueryType<GetSpeakers>()
                .AddMutationType<MutationSpeaker>();
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ILogger<Startup> logger,
            ILoggerFactory loggerFactory)
        {
            this._logger = logger;
            LogProvider._loggerFactory = loggerFactory;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => await context
                    .Response
                    .WriteAsync("Hello World!")
                    .ConfigureAwait(false));

                endpoints.MapGraphQL();
            });
        }
    }
}
