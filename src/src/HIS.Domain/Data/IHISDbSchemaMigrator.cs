using System.Threading.Tasks;

namespace HIS.Data;

public interface IHISDbSchemaMigrator
{
    Task MigrateAsync();
}
