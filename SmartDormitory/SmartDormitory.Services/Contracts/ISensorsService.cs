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
		Task RegisterNewSensor(string ownerId, string icbSensorId, string Name, string Description,
			int userPollingInterval, bool isPublic, bool alarmOn, float AlarmMinRange, float AlarmMaxRange,
			double longtitude, double latitude);
	}
}
