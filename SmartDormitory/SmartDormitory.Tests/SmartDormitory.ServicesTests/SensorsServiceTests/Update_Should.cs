using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Exceptions;
using System;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.SensorsServiceTests
{
	[TestClass]
	public class Update_Should
	{
		private DbContextOptions<SmartDormitoryContext> contextOptions;
		private Mock<IMeasureTypeService> measureTypeServiceMock = new Mock<IMeasureTypeService>();

		[TestMethod]
		public async Task Throw_EntityDoesntExistException_When_Sensor_Does_Not_Exist()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
			.UseInMemoryDatabase(databaseName: "Update_Throw_EntityDoesntExistException_When_Sensor_Does_Not_Exist")
				.Options;

			string sensorId = Guid.NewGuid().ToString();

			// Act && Assert

			using (var assertContext = new SmartDormitoryContext(contextOptions))
			{
				var sut = new SensorsService(assertContext,
				measureTypeServiceMock.Object);

				await Assert.ThrowsExceptionAsync<EntityDoesntExistException>(
					() => sut.Update(sensorId, It.IsAny<string>(), It.IsAny<string>(),
					It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(),
					It.IsAny<bool>(), It.IsAny<float>(), It.IsAny<float>(), It.IsAny<double>(),
					It.IsAny<double>(), It.IsAny<bool>()));
			}
		}

		[TestMethod]
		public async Task Successfully_Update_Sensor_When_Parameters_Are_Valid()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
			.UseInMemoryDatabase(databaseName: "Successfully_Update_Sensor_When_Parameters_Are_Valid")
				.Options;

			var sensor = SetupFakeSensor();
			using (var actContext = new SmartDormitoryContext(contextOptions))
			{
				await actContext.Sensors.AddAsync(sensor);
				await actContext.SaveChangesAsync();
			}

			// Act && Assert
			using (var assertContext = new SmartDormitoryContext(contextOptions))
			{
				var sut = new SensorsService(assertContext, measureTypeServiceMock.Object);
				var resultId = await sut.Update(sensor.Id, sensor.UserId, sensor.IcbSensorId,
					sensor.Name, sensor.Description, sensor.PollingInterval, sensor.IsPublic,
					sensor.AlarmOn, sensor.MinRangeValue, sensor.MaxRangeValue,
					sensor.Coordinates.Longitude, sensor.Coordinates.Latitude, sensor.SwitchOn);
				var resultSensor = await assertContext.Sensors.FirstOrDefaultAsync(s => s.Id == sensor.Id);
				Assert.AreEqual(resultSensor.ModifiedOn.Value.Day, DateTime.Now.Day);
				Assert.AreEqual(resultSensor.Id, sensor.Id);
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
				IsPublic = true,
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
