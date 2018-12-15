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
	public class GetSensorById_Should
	{
		private DbContextOptions<SmartDormitoryContext> contextOptions;
		private Mock<IMeasureTypeService> measureTypeServiceMock = new Mock<IMeasureTypeService>();

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

		[TestMethod]
		public async Task Return_Sensor_When_Id_Is_Found()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
			.UseInMemoryDatabase(databaseName: "Return_Sensor_When_Id_Is_Found")
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

				var result = await sut.GetSensorById(sensor.Id);
				Assert.AreEqual(result.Id, sensor.Id);
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
