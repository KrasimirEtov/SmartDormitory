using Microsoft.EntityFrameworkCore;
using SmartDormitory.App.Data;
using SmartDormitory.Services.Abstract;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Models.MeasureTypes;
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

        public async Task<IEnumerable<MeasureTypeServiceModel>> GetAll()
                 => await this.Context
                              .MeasureTypes
                              .Where(mt => !mt.IsDeleted)
                              .OrderBy(mt => mt.SuitableSensorType)
                              .Select(mt => new MeasureTypeServiceModel
                              {
                                  MeasureUnit = mt.MeasureUnit,
                                  SuitableSensorType = mt.SuitableSensorType,
                                  Id = mt.Id
                              })
                              .ToListAsync();

        public async Task<int> TotalCount()
                => await this.Context
                             .MeasureTypes
                             .Where(mt => !mt.IsDeleted)
                             .CountAsync();
    }
}
