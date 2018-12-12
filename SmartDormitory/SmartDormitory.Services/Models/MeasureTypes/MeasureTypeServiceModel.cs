namespace SmartDormitory.Services.Models.MeasureTypes
{
    public class MeasureTypeServiceModel
    {
        public string Id { get; set; }

        public string SuitableSensorType { get; set; }

        public string MeasureUnit { get; set; }
		public bool IsDeleted { get; set; }
	}
}
