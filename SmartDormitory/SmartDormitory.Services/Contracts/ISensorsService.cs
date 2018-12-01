using SmartDormitory.Data.Models;
using SmartDormitory.Services.Models.Sensors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDormitory.Services.Contracts
{
    public interface ISensorsService
    {
        //remove after test
        void SeedSomeSensorsForMaps();

        Task<IEnumerable<MapSensorServiceModel>> GetAllPublicSensorsCoordinates();
    }
}
