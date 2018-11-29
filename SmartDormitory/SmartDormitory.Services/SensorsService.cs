using Microsoft.EntityFrameworkCore;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services.Abstract;
using SmartDormitory.Services.Contracts;
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
            var sensor1 = new Sensor { Name = "Test1", Description = "Test1", Coordinates = new Coordinates { Latitude = 90, Longitude = 100 } };
            var sensor2 = new Sensor { Name = "Test2", Description = "Test1", Coordinates = new Coordinates { Latitude = 80, Longitude = 105 } };
            var sensor3 = new Sensor { Name = "Test3", Description = "Test1", Coordinates = new Coordinates { Latitude = 85, Longitude = 110 } };
            var sensor4 = new Sensor { Name = "Test4", Description = "Test1", Coordinates = new Coordinates { Latitude = 79, Longitude = 111 } };
            this.Context.Sensors.Add(sensor1);
            this.Context.Sensors.Add(sensor2);
            this.Context.Sensors.Add(sensor3);
            this.Context.Sensors.Add(sensor4);
            this.Context.SaveChanges();
        }

        public async Task<IEnumerable<Coordinates>> GetAllPublicSensorsCoordinates()
        {
            return await this.Context
                .Sensors
                .Select(s => new Coordinates
                {
                    Latitude = s.Coordinates.Latitude,
                    Longitude = s.Coordinates.Longitude
                })
                .ToListAsync();
        }
    }
}
