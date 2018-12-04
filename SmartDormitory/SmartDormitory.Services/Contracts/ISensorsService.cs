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

        Task<IEnumerable<AdminListSensorModel>> AllAdmin(string measureTypeId = "all", int isPublic = -1, int alarmSet = -1, int page = 1, int pageSize = 10);

        Task ToggleDeleteSensor(string sensorId);

        Task<string> RegisterNewSensor(string ownerId, string icbSensorId, string name, string description,
            int userPollingInterval, bool isPublic, bool alarmOn, float alarmMinRange, float alarmMaxRange,
            double longtitude, double latitude);

        Task<Sensor> GetSensorById(string sensorId);

        Task<int> TotalSensorsByCriteria(string measureTypeId, int isPublic = -1, int alarmSet = -1);
    }
}
