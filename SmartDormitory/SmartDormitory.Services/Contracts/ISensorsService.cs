using SmartDormitory.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDormitory.Services.Contracts
{
    public interface ISensorsService
    {
        //remove after test
        void SeedSomeSensorsForMaps();

        Task<IEnumerable<Coordinates>> GetAllPublicSensorsCoordinates();
    }
}
