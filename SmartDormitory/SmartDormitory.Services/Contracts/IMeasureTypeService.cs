using SmartDormitory.Services.Models.MeasureTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDormitory.Services.Contracts
{
    public interface IMeasureTypeService
    {
        Task<IEnumerable<MeasureTypeServiceModel>> GetAll();

        Task<bool> Exists(string id);

        Task<int> TotalCount();
    }
}
