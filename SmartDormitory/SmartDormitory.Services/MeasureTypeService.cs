using Microsoft.EntityFrameworkCore;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services.Abstract;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Exceptions;
using SmartDormitory.Services.Models.MeasureTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.Services
{
	public class MeasureTypeService : BaseService, IMeasureTypeService
	{
		public MeasureTypeService(SmartDormitoryContext context) : base(context)
		{
		}

		public async Task<bool> Exists(string id)
			=> await this.Context
				   .MeasureTypes
				   .AnyAsync(mt => !mt.IsDeleted && mt.Id == id);

		public async Task<IEnumerable<MeasureTypeServiceModel>> GetAllNotDeleted()
				 => await this.Context
							  .MeasureTypes
							  .Where(mt => !mt.IsDeleted)
							  .OrderBy(mt => mt.SuitableSensorType)
							  .Select(mt => new MeasureTypeServiceModel
							  {
								  MeasureUnit = mt.MeasureUnit,
								  SuitableSensorType = mt.SuitableSensorType,
								  Id = mt.Id,
								  IsDeleted = mt.IsDeleted
							  })
							  .ToListAsync();

		public async Task<IEnumerable<MeasureTypeServiceModel>> GetAllDeleted()
				 => await this.Context
							  .MeasureTypes
							  .OrderBy(mt => mt.IsDeleted)
							  .Select(mt => new MeasureTypeServiceModel
							  {
								  MeasureUnit = mt.MeasureUnit,
								  SuitableSensorType = mt.SuitableSensorType,
								  Id = mt.Id,
								  IsDeleted = mt.IsDeleted
							  })
							  .ToListAsync();

		public async Task<int> TotalCount()
				=> await this.Context
							 .MeasureTypes
							 .Where(mt => !mt.IsDeleted)
							 .CountAsync();

		public async Task<MeasureType> GetMeasureType(string measureUnit, string sensorType)
		{
			var measureType = await this.Context.MeasureTypes
				.Where(mt => mt.MeasureUnit == measureUnit && mt.SuitableSensorType == sensorType)
				.FirstOrDefaultAsync();
			return measureType;
		}

		public async Task Create(string measureUnit, string sensorType)
		{
			var type = await GetMeasureType(measureUnit, sensorType);
			if (type != null)
			{
				if (type.IsDeleted)
				{
					type.IsDeleted = false;
					await this.Context.SaveChangesAsync();
				}
				else
				{
					throw new EntityAlreadyExistsException($"\nMeasure type is already present in the database.");
				}
			}
			else
			{
				type = new MeasureType()
				{
					CreatedOn = DateTime.Now,
					MeasureUnit = measureUnit,
					SuitableSensorType = sensorType
				};
				await this.Context.MeasureTypes.AddAsync(type);
				await this.Context.SaveChangesAsync();
			}
		}

		public async Task<MeasureType> GetType(string typeId)
		{
			return await this.Context.MeasureTypes
				.Where(mt => mt.Id == typeId)
				.FirstOrDefaultAsync();
		}

		public async Task DeleteType(string typeId)
		{
			var type = await GetType(typeId);
			if (type is null)
			{
				throw new EntityDoesntExistException($"\nMeasure Type doesn't exists!");
			}

			type.IsDeleted = !type.IsDeleted ? true : false;

			this.Context.MeasureTypes.Update(type);
			await this.Context.SaveChangesAsync();
		}
	}
}
