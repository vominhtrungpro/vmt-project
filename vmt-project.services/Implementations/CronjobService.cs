using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Models.Entities;
using vmt_project.services.Contracts;

namespace vmt_project.services.Implementations
{
    public class CronjobService : ICronjobService
    {
        private readonly UserManager<User> _userManager;
        public CronjobService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public int CountUser()
        {
            return _userManager.Users.Count();
        }
    }
}
