using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VacationRental.SharedKernel.Interfaces;
using VacationRental.SharedKernel.Services;

namespace VacationRental.SharedKernel
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSharedKernel(this IServiceCollection services)
        {
            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            return services;
        }
    }
}
