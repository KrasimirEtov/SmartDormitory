﻿using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.SensorsServiceTests
{
	[TestClass]
	public class GetUserSensors_Should
	{
		private DbContextOptions<SmartDormitoryContext> contextOptions;
		private Mock<IMeasureTypeService> measureTypeServiceMock = new Mock<IMeasureTypeService>();

		[TestMethod]
		public async Task Throw_EntityDoesntExistException_When_User_Does_Not_Exist()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
			.UseInMemoryDatabase(databaseName: "GetUsersSensors_Throw_EntityDoesntExistException_When_User_Does_Not_Exist")
				.Options;

			string userId = Guid.NewGuid().ToString();

			// Act && Assert// Arrange

			using (var assertContext = new SmartDormitoryContext(contextOptions))
			{
				var sut = new SensorsService(assertContext,
				measureTypeServiceMock.Object);

				await Assert.ThrowsExceptionAsync<EntityDoesntExistException>(
					() => sut.GetUserSensors(userId));
			}
		}

		[TestMethod]
		public async Task Return_Valid_Sensor_Enumerable_When_Passed_Parameters_Are_Default()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
			.UseInMemoryDatabase
			(databaseName: "GetUserSensorsReturn_Valid_Sensor_Enumerable_When_Passed_Parameters_Are_Default")
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
				var sensors = await sut.GetUserSensors(sensor.UserId);
				Assert.AreEqual(1, sensors.Count());
			}
		}

		[TestMethod]
		public async Task Return_Valid_Sensor_Enumerable_When_SearchTerm_Is_Not_Default()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
			.UseInMemoryDatabase
			(databaseName: "GetUserSensorsReturn_Return_Valid_Sensor_Enumerable_When_SearchTerm_Is_Not_Default")
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
				var sensors = await sut.GetUserSensors(sensor.UserId, "description", "all", -1, -1);
				Assert.AreEqual(1, sensors.Count());
			}
		}

		[TestMethod]
		public async Task Return_Valid_Sensor_Enumerable_When_MeasureType_Is_Not_Default()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
			.UseInMemoryDatabase
			(databaseName: "GetUserSensorsReturn_Return_Valid_Sensor_Enumerable_When_MeasureType_Is_Not_Default")
				.Options;

			var sensor = SetupFakeSensor();

			using (var actContext = new SmartDormitoryContext(contextOptions))
			{
				await actContext.Sensors.AddAsync(sensor);
				await actContext.SaveChangesAsync();
			}
			measureTypeServiceMock
				.Setup(x => x.Exists(It.IsAny<string>()))
				.ReturnsAsync(true);

			// Act && Assert
			using (var assertContext = new SmartDormitoryContext(contextOptions))
			{
				var sut = new SensorsService(assertContext, measureTypeServiceMock.Object);
				var sensors = await sut.GetUserSensors(sensor.UserId, "", "temperature", -1, -1);
				Assert.AreEqual(1, sensors.Count());
			}
		}

		[TestMethod]
		public async Task Return_Valid_Sensor_Enumerable_When_IsPublic_Is_Not_Default()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
			.UseInMemoryDatabase
			(databaseName: "GetUserSensorsReturn_Valid_Sensor_Enumerable_When_IsPublic_Is_Not_Default")
				.Options;

			var sensor = SetupFakeSensor();

			using (var actContext = new SmartDormitoryContext(contextOptions))
			{
				await actContext.Sensors.AddAsync(sensor);
				await actContext.SaveChangesAsync();
			}
			measureTypeServiceMock
				.Setup(x => x.Exists(It.IsAny<string>()))
				.ReturnsAsync(true);

			// Act && Assert
			using (var assertContext = new SmartDormitoryContext(contextOptions))
			{
				var sut = new SensorsService(assertContext, measureTypeServiceMock.Object);

				var sensors = await sut.GetUserSensors(sensor.UserId, "", "all", 1, -1);
				Assert.AreEqual(1, sensors.Count());
			}
		}

		[TestMethod]
		public async Task Return_Valid_Sensor_Enumerable_When_AlarmOn_Is_Not_Default()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
			.UseInMemoryDatabase
			(databaseName: "GetUserSensorsReturn_Valid_Sensor_Enumerable_When_AlarmOn_Is_Not_Default")
				.Options;

			var sensor = SetupFakeSensor();

			using (var actContext = new SmartDormitoryContext(contextOptions))
			{
				await actContext.Sensors.AddAsync(sensor);
				await actContext.SaveChangesAsync();
			}
			measureTypeServiceMock
				.Setup(x => x.Exists(It.IsAny<string>()))
				.ReturnsAsync(true);

			// Act && Assert
			using (var assertContext = new SmartDormitoryContext(contextOptions))
			{
				var sut = new SensorsService(assertContext, measureTypeServiceMock.Object);
				var sensors = await sut.GetUserSensors(sensor.UserId, "", "all", -1, 1);
				Assert.AreEqual(1, sensors.Count());
			}
		}

		[TestMethod]
		public async Task Return_Valid_Sensor_Enumerable_When_MeasureType_Is_Boolean_True()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
			.UseInMemoryDatabase
			(databaseName: "GetUserSensorsReturn_Return_Valid_Sensor_Enumerable_When_MeasureType_Is_Boolean_True")
				.Options;

			var sensor = SetupFakeSensor();
			sensor.IcbSensor.MeasureType.SuitableSensorType = "Boolean switch (door/occupancy/etc)";
			sensor.CurrentValue = 1;

			using (var actContext = new SmartDormitoryContext(contextOptions))
			{
				await actContext.Sensors.AddAsync(sensor);
				await actContext.SaveChangesAsync();
			}
			measureTypeServiceMock
				.Setup(x => x.Exists(It.IsAny<string>()))
				.ReturnsAsync(true);

			// Act && Assert
			using (var assertContext = new SmartDormitoryContext(contextOptions))
			{
				var sut = new SensorsService(assertContext, measureTypeServiceMock.Object);
				var sensors = await sut.GetUserSensors(sensor.UserId, "",
					"all", -1, -1);
				Assert.AreEqual(1, sensors.Count());
			}
		}

		[TestMethod]
		public async Task Return_Valid_Sensor_Enumerable_When_MeasureType_Is_Boolean_False()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
			.UseInMemoryDatabase
			(databaseName: "GetUserSensorsReturn_Return_Valid_Sensor_Enumerable_When_MeasureType_Is_Boolean_False")
				.Options;

			var sensor = SetupFakeSensor();
			sensor.IcbSensor.MeasureType.SuitableSensorType = "Boolean switch (door/occupancy/etc)";
			sensor.CurrentValue = 0;

			using (var actContext = new SmartDormitoryContext(contextOptions))
			{
				await actContext.Sensors.AddAsync(sensor);
				await actContext.SaveChangesAsync();
			}
			measureTypeServiceMock
				.Setup(x => x.Exists(It.IsAny<string>()))
				.ReturnsAsync(true);

			// Act && Assert
			using (var assertContext = new SmartDormitoryContext(contextOptions))
			{
				var sut = new SensorsService(assertContext, measureTypeServiceMock.Object);
				var sensors = await sut.GetUserSensors(sensor.UserId, "",
					"all", -1, -1);
				Assert.AreEqual(1, sensors.Count());
			}
		}

		private Sensor SetupFakeSensor()
		{
			string measureTypeId = "temperature";
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
				AlarmOn = true,
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
						Id = measureTypeId,
						MeasureUnit = "gradusi",
						CreatedOn = DateTime.Now,
						SuitableSensorType = "temperaturno"
					},
					MeasureTypeId = measureTypeId
				},
				User = new User()
				{
					AgreedGDPR = true,
					IsDeleted = false,
					UserName = "testera",
					Email = "tester4eto",
					Id = Guid.NewGuid().ToString()
				}
			};
			return sensor;
		}
	}
}
