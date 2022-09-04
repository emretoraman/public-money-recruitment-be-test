using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VacationRental.Infrastructure.Data;
using VacationRental.SharedKernel;
using VacationRental.SharedKernel.Interfaces;

namespace VacationRental.DependencyInversion
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services.AddSharedKernel();

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddDbContext<VacationRentalDbContext>(options =>
                options.UseInMemoryDatabase("VacationRental")
            );
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

            return services;
        }
    }
}
