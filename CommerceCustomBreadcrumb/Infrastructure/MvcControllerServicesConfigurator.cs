using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using CommerceCustomBreadcrumb.Repositories;

namespace CommerceCustomBreadcrumb.Infrastructure
{
    public class MvcControllerServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<Sitecore.XA.Feature.Navigation.Repositories.Breadcrumb.IBreadcrumbRepository, BreadcrumbRepository>();
        }
    }
}