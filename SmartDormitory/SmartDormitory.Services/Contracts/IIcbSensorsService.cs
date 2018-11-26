using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDormitory.Services.Contracts
{
    public interface IIcbSensorsService
    {
        Task<IReadOnlyList<string>> AddSensors();
    }
}