﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.models.DTO.MyProfile.cs
{
    public class MyProfileDto
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public int Order { get; set; }
        public List<string> PictureUrls { get; set; }
    }
}
