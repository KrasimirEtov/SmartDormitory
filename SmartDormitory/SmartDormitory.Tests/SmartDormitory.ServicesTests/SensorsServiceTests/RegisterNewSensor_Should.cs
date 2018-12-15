using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services;
using SmartDormitory.Services.Contracts;
using System;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.SensorsServiceTests
{
	[TestClass]
	public class RegisterNewSensor_Should
	{
		private DbContextOptions<SmartDormitoryContext> contextOptions;
		private Mock<IMeasureTypeService> measureTypeServiceMock = new Mock<IMeasureTypeService>();

		[TestMethod]
		public async Task Return_Empty_String_When_Icb_Sensors_Are_Empty()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
			.UseInMemoryDatabase(databaseName: "Return_Valid_Sensor_Enumerable")
				.Options;

			string expected = "";

			// Act && Assert
			using (var assertContext = new SmartDormitoryContext(contextOptions))
			{
				var sut = new SensorsService(assertContext, measureTypeServiceMock.Object);
				var result = await sut.RegisterNewSensor(It.IsAny<string>(), "icbSensorId",
					It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(),
					It.IsAny<bool>(), It.IsAny<float>(), It.IsAny<float>(), It.IsAny<double>(),
					It.IsAny<double>(), It.IsAny<bool>());
				Assert.AreEqual(expected, result);
			}
		}

		[TestMethod]
		public async Task Successfully_Register_Sensor_When_Parameters_Are_Valid()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
			.UseInMemoryDatabase(databaseName: "Successfully_Register_Sensor_When_Parameters_Are_Valid")
				.Options;

			string icbSensorId = Guid.NewGuid().ToString();
			using (var actContext = new SmartDormitoryContext(contextOptions))
			{
				await actContext.IcbSensors.AddAsync(new IcbSensor()
				{
					Id = icbSensorId
				});
				await actContext.SaveChangesAsync();
			}

			// Act && Assert
			using (var assertContext = new SmartDormitoryContext(contextOptions))
			{
				var sut = new SensorsService(assertContext, measureTypeServiceMock.Object);
				var result = await sut.RegisterNewSensor("userId", icbSensorId, "name", "desc", 50,
					true, true, 40, 50, 20.5, 40.5, true);
				var sensorsCount = await assertContext.Sensors.CountAsync();
				var sensor = await assertContext.Sensors.FirstOrDefaultAsync(x => x.Id == result);
				Assert.AreEqual(result, sensor.Id);
				Assert.AreEqual(1, sensorsCount);
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
