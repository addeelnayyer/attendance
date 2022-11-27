using System.Collections.Generic;
using System.Threading.Tasks;
using Aquila360.Attendance.Models;

namespace Aquila360.Attendance.Contracts
{
    public interface ICosmosService<T> where T : BaseModel
    {
        Task BulkInsert(IEnumerable<T> models);

        Task BulkUpsert(IEnumerable<T> models);

        Task Insert(T model);

        Task<IEnumerable<T>> RetrieveAllAsync();

        Task Upsert(T model);
    }
}
