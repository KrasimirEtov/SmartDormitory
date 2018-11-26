using SmartDormitory.App.Data;

namespace SmartDormitory.Services.Abstract
{
    public abstract class BaseService
    {
        private readonly SmartDormitoryContext context;

        public BaseService(SmartDormitoryContext context)
        {
            this.context = context;
        }

        public SmartDormitoryContext Context => this.context;
    }
}
