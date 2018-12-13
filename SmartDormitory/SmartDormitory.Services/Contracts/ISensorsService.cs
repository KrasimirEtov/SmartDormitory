using SmartDormitory.Data.Models;
using SmartDormitory.Services.Models.Sensors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDormitory.Services.Contracts
{
    public interface ISensorsService
    {
        Task<IEnumerable<MapSensorServiceModel>> GetAllCoordinates();

        Task<IEnumerable<MapSensorServiceModel>> GetAllPublicCoordinates();

        Task<IEnumerable<MapSensorServiceModel>> GetAllUserPrivateCoordinates(string userId);

        Task<IEnumerable<AdminListSensorModel>> AllAdmin(string measureTypeId = "all", int isPublic = -1, int alarmSet = -1, int page = 1, int pageSize = 10);

        Task ToggleSoftDeleteSensor(string sensorId);

        Task<Sensor> GetSensorById(string sensorId);

        Task<int> TotalSensorsByCriteria(string measureTypeId, int isPublic = -1, int alarmSet = -1);
        Task<int> TotalSensors();

        Task<IEnumerable<UserSensorListModel>> GetUserSensors(string userId, string searchTerm = "", string measureTypeId = "all", int alarmOn = -1, int isPublic = -1);
        Task<GaugeDataServiceModel> GetGaugeData(string sensorId);

        Task<string> Update(string sensorId, string userId, string icbSensorId, string name, string description,
            int pollingInterval, bool isPublic, bool alarmOn, float minRange,
            float maxRange, double longtitude, double latitude, bool switchOn);
        Task<string> RegisterNewSensor(string userId, string icbSensorId, string name, string description,
            int pollingInterval, bool isPublic, bool alarmOn, float minRange,
            float maxRange, double longtitude, double latitude, bool switchOn);

        Task<IEnumerable<Sensor>> GetAll();
        Task UpdateRange(IList<Sensor> sensorsToUpdate);

        Task<IEnumerable<Sensor>> GetAllForUpdate();
    }
}
