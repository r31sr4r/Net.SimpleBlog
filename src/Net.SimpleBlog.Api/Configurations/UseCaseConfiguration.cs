using Net.SimpleBlog.Application.Interfaces;
using Net.SimpleBlog.Application.UseCases.User.CreateUser;
using Net.SimpleBlog.Domain.Repository;
using Net.SimpleBlog.Infra.Data.EF.Repositories;
using MediatR;
using Net.SimpleBlog.Infra.Data.EF;

namespace Net.SimpleBlog.Api.Configurations;

public static class UseCaseConfiguration
{
    public static IServiceCollection AddUseCases(
               this IServiceCollection services
           )
    {
        services.AddMediatR(typeof(CreateUser));
        services.AddRepositories();  

        return services;
    }

    private static IServiceCollection AddRepositories(
           this IServiceCollection services
       )
    {
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        return services;
    }


}
