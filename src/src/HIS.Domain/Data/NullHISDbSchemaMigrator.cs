using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace HIS.Data;

/* This is used if database provider does't define
 * IHISDbSchemaMigrator implementation.
 */
public class NullHISDbSchemaMigrator : IHISDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
