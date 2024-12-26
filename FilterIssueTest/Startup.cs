using FilterIssueTest.GraphQl;
using HotChocolate.Types.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FilterIssueTest
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddGraphQLServer()
                .AddQueryType<Query>();

            // Force to use custom filter visitor with new conditions order
            QueryableFilterVisitor.Default = new QueryableCustomFilterVisitor();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapGraphQL("/api/"); });
        }
    }
}
