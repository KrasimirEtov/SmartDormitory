using Microsoft.EntityFrameworkCore;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services.Abstract;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Models.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.Services
{
	public class SensorsService : BaseService, ISensorsService
	{
		public SensorsService(SmartDormitoryContext context) : base(context)
		{
		}

		// for test purposes google map
		public void SeedSomeSensorsForMaps()
		{
			var sensor1 = new Sensor { Name = "Gerena sensors", Description = "suhata reka bla bla", Coordinates = new Coordinates { Latitude = 42.7034, Longitude = 23.36614 } };
			var sensor2 = new Sensor { Name = "Telerik academy", Description = "steven temp sensor", Coordinates = new Coordinates { Latitude = 42.6522, Longitude = 23.37397 } };
			var sensor3 = new Sensor { Name = "Letishte sofia", Description = "noise sensor terminal 1", Coordinates = new Coordinates { Latitude = 42.687333, Longitude = 23.405452 } };
			var sensor4 = new Sensor { Name = "Borisova gr", Description = "some sensor test 123", Coordinates = new Coordinates { Latitude = 42.679320, Longitude = 23.339460 } };
			this.Context.Sensors.Add(sensor1);
			this.Context.Sensors.Add(sensor2);
			this.Context.Sensors.Add(sensor3);
			this.Context.Sensors.Add(sensor4);
			this.Context.SaveChanges();
		}

		public async Task<IEnumerable<MapSensorServiceModel>> GetAllPublicSensorsCoordinates()
		{
			return await this.Context
				.Sensors
				.Select(s => new MapSensorServiceModel
				{
					Id = s.Id,
					Name = s.Name,
					Description = s.Description,
					Coordinates = new Coordinates
					{
						Latitude = s.Coordinates.Latitude,
						Longitude = s.Coordinates.Longitude
					},
				})
				.ToListAsync();
		}

		public async Task RegisterNewSensor(string ownerId, string icbSensorId, string name, string description,
			int userPollingInterval, bool isPublic, bool alarmOn, float alarmMinRange, float alarmMaxRange,
			double longtitude, double latitude)
		{
			if (await this.Context.IcbSensors.AnyAsync(s => s.Id == icbSensorId))
			{
				var sensor = new Sensor()
				{
					AlarmMaxRangeValue = alarmMaxRange,
					AlarmMinRangeValue = alarmMinRange,
					AlarmOn = alarmOn,
					Description = description,
					Name = name,
					OwnerId = ownerId,
					IsPublic = isPublic,
					UserPollingInterval = userPollingInterval,
					IcbSensorId = icbSensorId,
					Coordinates = new Coordinates
					{
						Latitude = latitude,
						Longitude = longtitude
					},
					CreatedOn = DateTime.Now
				};
				await this.Context.Sensors.AddAsync(sensor);
				await this.Context.SaveChangesAsync();
			}
		}
	}
}
