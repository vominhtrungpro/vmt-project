﻿using Confluent.Kafka;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Nest;
using NetCore.Infrastructure.Common.Helpers;
using NetCore.Infrastructure.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Contracts;
using vmt_project.dal.Models.Entities;
using vmt_project.models.DTO.Character;
using vmt_project.models.Request.Character;
using vmt_project.models.Request.Generic;
using vmt_project.models.Response.Character;
using vmt_project.services.Contracts;
using static vmt_project.common.Constants.ResponseMessages;
using Filter = vmt_project.models.Request.Generic.Filter;

namespace vmt_project.services.Implementations
{
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly ElasticClient _elasticClient;
        public CharacterService(ICharacterRepository characterRepository,ElasticClient elasticClient)
        {
            _characterRepository = characterRepository;
            _elasticClient = elasticClient;
        }
        public async Task<AppActionResult> Create(CreateCharacterRequest request)
        {
            var result = new AppActionResult();
            try
            {
                var character = new Character
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name
                };
                character.SetCreatedInfo("Admin");
                character.SetModifiedInfo("Admin");
                _characterRepository.Add(character);
            }
            catch (Exception ex)
            {

                return result.BuildError(ex.Message);
            }
            return result.BuildResult(SUCCESS_CREATE_CHARACTER);
        }
        public async Task<AppActionResultData<List<CharacterDto>>> List()
        {
            var result = new AppActionResultData<List<CharacterDto>>();
            try
            {
                var list = _characterRepository
                    .GetAll()
                    .OrderByDescending(m => m.CreatedOn)
                    .Select(m => new CharacterDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                    })
                    .ToList();

                return result.BuildResult(list);
            }
            catch (Exception ex)
            {
                return result.BuildError(ex.Message);
            }
        }
        public async Task<AppActionResultData<SearchCharacterResult>> Search(SearchCharacterRequest request)
        {
            var result = new AppActionResultData<SearchCharacterResult>();
            try
            {
                long pageSize = request.PageSize.HasValue ? request.PageSize.Value : 10; 
                long pageIndex = request.PageIndex.HasValue ? request.PageIndex.Value : 1;

                var searchDescriptor = new SearchDescriptor<Character>()
                    .From((int)((pageIndex - 1) * pageSize))
                    .Size((int)pageSize);

                searchDescriptor = Filters(searchDescriptor, request.Filters);

                var searchResponse = await _elasticClient.SearchAsync<Character>(searchDescriptor);

                if (!searchResponse.IsValid)
                {
                    return result.BuildError(searchResponse.OriginalException.ToString());
                }

                var characters = searchResponse.Documents.Select(s => new CharacterDto { 
                    Id = s.Id,
                    Name = s.Name,
                }).ToList();
                var numOfRecords = searchResponse.Total;

                var searchResult = new SearchCharacterResult
                {
                    NumOfRecords = numOfRecords,
                    NumOfPages = SearchHelper.CalculateNumOfPages((int)numOfRecords, (int)pageSize),
                    PageIndex = pageIndex,
                    Data = characters,
                };
                return result.BuildResult(searchResult);

            }
            catch (Exception ex)
            {
                return result.BuildError(ex.ToString());
            }
        }
        private SearchDescriptor<Character> Filters(SearchDescriptor<Character> searchDescriptor, IList<Filter> filters)
        {
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    switch (filter.FieldName)
                    {
                        case "Name":
                            searchDescriptor = searchDescriptor.Query(q => q
                                .Bool(b => b
                                    .Filter(f => f
                                        .QueryString(qs => qs
                                            .DefaultField(f => f.Name)
                                            .Query($"*{filter.Value}*")
                                        )
                                    )
                                )
                            );
                        break;
                    }
                }
            }
            else
            {
                searchDescriptor = searchDescriptor.Query(q => q.MatchAll());
            }

            return searchDescriptor;
        }

    }
}
