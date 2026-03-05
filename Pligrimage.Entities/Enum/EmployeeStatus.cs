using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pligrimage.Entities.Enum
{
    public enum EmployeeStatus
    {
        [Display(Name = "موظف")]
        Employee = 0,
        [Display(Name = "متقاعد")]
        Pension=2
    }
}
