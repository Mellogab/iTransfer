using Microsoft.EntityFrameworkCore;

namespace iTransferencia.Infrastructure.EntityExtensions
{
    public static class EntityExtensions
    {
        public static void AddEntityConfigurations(this IEnumerable<Type> configurations, ModelBuilder modelBuilder)
        {
            configurations
                .ToList()
                .ForEach(configuration =>
                {
                    dynamic instance = Activator.CreateInstance(configuration);
                    modelBuilder.ApplyConfiguration(instance);
                });
        }
    }
}
