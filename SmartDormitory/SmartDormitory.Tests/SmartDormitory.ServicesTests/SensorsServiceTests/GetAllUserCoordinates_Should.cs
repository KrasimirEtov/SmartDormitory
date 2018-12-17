using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services;
using SmartDormitory.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.SensorsServiceTests
{
	[TestClass]
	public class GetAllUserCoordinates_Should
	{

		private DbContextOptions<SmartDormitoryContext> contextOptions;
		private Mock<IMeasureTypeService> measureTypeServiceMock = new Mock<IMeasureTypeService>();

		[TestMethod]
		public async Task Return_Valid_Sensor_Enumerable()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
			.UseInMemoryDatabase(databaseName: "GetAllUserCoordinates_Private_Return_Valid_Sensor_Enumerable")
				.Options;
			var userId = Guid.NewGuid().ToString();
			var sensor = SetupFakeSensor();
			var sensor2 = SetupFakeSensor();
			sensor2.IsPublic = true;
			sensor.UserId = userId;
			sensor2.UserId = userId;

			using (var actContext = new SmartDormitoryContext(contextOptions))
			{
				await actContext.Sensors.AddRangeAsync(new List<Sensor>() { sensor, sensor2 });
				await actContext.SaveChangesAsync();
			}

			// Act && Assert
			using (var assertContext = new SmartDormitoryContext(contextOptions))
			{
				var sut = new SensorsService(assertContext, measureTypeServiceMock.Object);
				var sensors = await sut.GetAllUserCoordinates(userId);
				Assert.AreEqual(2, sensors.Count());
			}
		}

		private Sensor SetupFakeSensor()
		{
			var sensor = new Sensor()
			{
				Coordinates = new Coordinates()
				{
					Latitude = 50.02,
					Longitude = 40.02
				},
				CreatedOn = DateTime.Now,
				CurrentValue = 20,
				Description = "description",
				Id = Guid.NewGuid().ToString(),
				IsPublic = false,
				IsDeleted = false,
				Name = "name",
				UserId = Guid.NewGuid().ToString(),
				IcbSensor = new IcbSensor()
				{
					Description = "icb description",
					Id = Guid.NewGuid().ToString(),
					MaxRangeValue = 100,
					MinRangeValue = 1,
					PollingInterval = 50,
					Tag = "djoni",
					MeasureType = new MeasureType()
					{
						Id = Guid.NewGuid().ToString(),
						MeasureUnit = "gradusi",
						CreatedOn = DateTime.Now,
						SuitableSensorType = "temperaturno"
					}
				}
			};
			return sensor;
		}
	}
}