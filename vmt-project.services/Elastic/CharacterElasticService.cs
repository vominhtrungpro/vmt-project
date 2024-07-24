﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Models.Entities;
using vmt_project.services.Elastic.Base;

namespace vmt_project.services.Elastic
{
    public class CharacterElasticService : ICharacterElasticService
    {
        private readonly IGenericElasticService<Character> _genericElasticService;
        public CharacterElasticService(IGenericElasticService<Character> genericElasticService)
        {
            _genericElasticService = genericElasticService;
        }
        public async Task<Character> GetCharacterById(string id)
        {
            var character = await _genericElasticService.GetDocumentAsync(id);
            return character;
        }
    }
}
