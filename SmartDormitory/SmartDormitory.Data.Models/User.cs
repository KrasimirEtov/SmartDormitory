using Microsoft.AspNetCore.Identity;
using SmartDormitory.Data.Models.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartDormitory.Data.Models
{
    public class User : IdentityUser, IAuditable, IDeletable
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ICollection<Sensor> Sensors { get; set; } = new HashSet<Sensor>();

        public bool IsDeleted { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? DeletedOn { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? CreatedOn { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ModifiedOn { get; set; }
    }
}
