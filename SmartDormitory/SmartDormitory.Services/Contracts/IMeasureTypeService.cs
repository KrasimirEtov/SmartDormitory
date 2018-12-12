using SmartDormitory.Data.Models;
using SmartDormitory.Services.Models.MeasureTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDormitory.Services.Contracts
{
    public interface IMeasureTypeService
    {
        Task<IEnumerable<MeasureTypeServiceModel>> GetAllNotDeleted();

        Task<bool> Exists(string id);

        Task<int> TotalCount();

		Task Create(string measureUnit, string sensorType);

		Task<MeasureType> GetMeasureType(string measureUnit, string sensorType);

		Task<MeasureType> GetType(string typeId);

		Task DeleteType(string typeId);

		Task<IEnumerable<MeasureTypeServiceModel>> GetAllDeleted();

	}
}
