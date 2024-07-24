using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Contracts;
using vmt_project.dal.Models.Entities;
using vmt_project.services.Contracts;

namespace vmt_project.services.Implementations
{
    public class CronjobService : ICronjobService
    {
        private readonly UserManager<User> _userManager;
        private readonly ICharacterRepository _characterRepository;
        public CronjobService(UserManager<User> userManager,ICharacterRepository characterRepository)
        {
            _userManager = userManager;
            _characterRepository = characterRepository;
        }
        public int CountUser()
        {
            return _userManager.Users.Count();
        }
        public async Task<List<Character>> ElasticSearchCharacter(DateTime time)
        {
            var characters = await _characterRepository.FindBy(m => m.ModifiedOn > time).ToListAsync();

            return characters;
        }
    }
}
