using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services;
using SmartDormitory.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.SensorsServiceTests
{
	[TestClass]
	public class TotalSensors_Should
	{
		private DbContextOptions<SmartDormitoryContext> contextOptions;
		private Mock<IMeasureTypeService> measureTypeServiceMock = new Mock<IMeasureTypeService>();

		[TestMethod]
		public async Task Return_Valid_Sensors_Count()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
			.UseInMemoryDatabase(databaseName: "Return_Valid_Sensors_Count")
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

				var usersCount = await sut.TotalSensors();

				Assert.AreEqual(1, usersCount);
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
