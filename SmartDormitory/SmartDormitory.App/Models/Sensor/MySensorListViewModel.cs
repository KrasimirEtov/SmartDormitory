using SmartDormitory.App.Infrastructure.Extensions;
using SmartDormitory.Services.Models.Sensors;
using System;

namespace SmartDormitory.App.Models.Sensor
{
    public class MySensorListViewModel
    {
        public MySensorListViewModel()
        {

        }

        public MySensorListViewModel(UserSensorListModel model)
        {
            this.Id = model.Id;
            this.Name = model.Name;
            this.SensorType = model.SensorType;
            this.MeasureUnit = model.MeasureUnit;
            this.CreatedOn = model.CreatedOn;
            this.IsPublic = model.IsPublic;
            this.AlarmOn = model.AlarmOn;
            this.ImagePath = model.MeasureUnit.GetImagePathByMeasureUnit();
            this.PollingInterval = model.PollingInterval;
			this.Value = model.Value;
			this.LastUpdateOn = model.LastUpdateOn;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string SensorType { get; set; }

        public string MeasureUnit { get; set; }

        public int PollingInterval { get; set; }

        public string ImagePath { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool AlarmOn { get; set; }

        public bool IsPublic { get; set; }

		public string Value { get; set; }
		public DateTime LastUpdateOn { get; set; }
	}
}
