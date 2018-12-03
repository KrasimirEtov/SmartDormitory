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
		Task<string> RegisterNewSensor(string ownerId, string icbSensorId, string name, string description,
			int userPollingInterval, bool isPublic, bool alarmOn, float alarmMinRange, float alarmMaxRange,
			double longtitude, double latitude);
		Task<Sensor> GetSensorById(string sensorId);
	}
}
