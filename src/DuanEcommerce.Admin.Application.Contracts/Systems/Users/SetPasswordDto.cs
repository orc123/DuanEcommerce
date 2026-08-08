using System;
using System.Collections.Generic;
using System.Text;

namespace DuanEcommerce.Admin.Users;

public class SetPasswordDto
{
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}
