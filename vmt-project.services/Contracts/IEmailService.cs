﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Models.Entities;
using vmt_project.models.Request.Email;

namespace vmt_project.services.Contracts
{
    public interface IEmailService
    {
        Task SendEmail(SendEmailRequest request);
        Task SendEmailForgetPassword(User user, string token);
    }
}
