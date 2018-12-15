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
	public class DeleteSensor_Should
	{
		private DbContextOptions<SmartDormitoryContext> contextOptions;
		private Mock<IMeasureTypeService> measureTypeServiceMock = new Mock<IMeasureTypeService>();

		[TestMethod]
		public async Task Throw_EntityDoesntExistException_When_Sensor_Does_Not_Exist()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
			.UseInMemoryDatabase(databaseName: "Throw_EntityDoesntExistException_When_Sensor_Does_Not_Exist")
				.Options;

			string sensorId = Guid.NewGuid().ToString();

			// Act && Assert// Arrange
			
			using (var assertContext = new SmartDormitoryContext(contextOptions))
			{
				var sut = new SensorsService(assertContext,
				measureTypeServiceMock.Object);

				await Assert.ThrowsExceptionAsync<EntityDoesntExistException>(
					() => sut.DeleteSensor(sensorId));
			}
		}

		[TestMethod]
		public async Task Remove_Sensor_When_Exists_And_Is_Not_Deleted()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
			.UseInMemoryDatabase(databaseName: "Remove_Sensor_When_Exists_And_Is_Not_Deleted")
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
				var sut = new SensorsService(assertContext,
								measureTypeServiceMock.Object);

				await sut.DeleteSensor(sensor.Id);
				var result = await assertContext.Sensors.FirstOrDefaultAsync(s => s.Id == sensor.Id);
				Assert.IsTrue(result.IsDeleted);
			}
		}

		[TestMethod]
		public async Task Restore_Sensor_When_Exists_And_Is_Deleted()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
			.UseInMemoryDatabase(databaseName: "Restore_Sensor_When_Exists_And_Is_Deleted")
				.Options;

			var sensor = SetupFakeSensor();
			sensor.IsDeleted = true;

			using (var actContext = new SmartDormitoryContext(contextOptions))
			{
				await actContext.Sensors.AddAsync(sensor);
				await actContext.SaveChangesAsync();
			}

			// Act && Assert
			using (var assertContext = new SmartDormitoryContext(contextOptions))
			{
				var sut = new SensorsService(assertContext,
								measureTypeServiceMock.Object);

				await sut.DeleteSensor(sensor.Id);
				var result = await assertContext.Sensors.FirstOrDefaultAsync(s => s.Id == sensor.Id);
				Assert.IsFalse(result.IsDeleted);
			}
		}

		[TestMethod]
		public async Task Throw_ArugmentException_When_Passed_Id_Is_Null()
		{
			// Arrange
			var contextMock = new Mock<SmartDormitoryContext>();

			var sut = new SensorsService(contextMock.Object,
				measureTypeServiceMock.Object);

			// Act & Assert
			await Assert.ThrowsExceptionAsync<ArgumentNullException>(
				() => sut.GetSensorById(null));
		}

		[TestMethod]
		public async Task Throw_ArugmentException_When_Passed_Invalid_Guid()
		{
			// Arrange
			var contextMock = new Mock<SmartDormitoryContext>();

			var sut = new SensorsService(contextMock.Object,
				measureTypeServiceMock.Object);

			// Act & Assert
			await Assert.ThrowsExceptionAsync<ArgumentException>(
				() => sut.GetSensorById("InvalidGuid"));
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
