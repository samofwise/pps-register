using System.Reflection;

namespace VehicleRegistrationApi.Helpers;
public static class ApplicationServicesExtensions
{
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    var assembly = Assembly.GetExecutingAssembly();
    var allTypes = assembly.GetTypes();
    var serviceTypes = allTypes
        .Where(t => t.IsClass && !t.IsAbstract && t.Namespace != null && t.Namespace.Contains(".Services")).ToList();

    foreach (var serviceType in serviceTypes)
    {
      var interfaceType = serviceType.GetInterfaces()
          .FirstOrDefault(i => i.Name == $"I{serviceType.Name}");

      if (interfaceType != null)
        services.AddTransient(interfaceType, serviceType);
    }

    return services;
  }
}