using AutoMapper;
using DOTNET_RPG.Dtos.Character;
using DOTNET_RPG.Dtos.Fight;
using DOTNET_RPG.Dtos.Skill;
using DOTNET_RPG.Dtos.Weapon;
using DOTNET_RPG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DOTNET_RPG
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDto>();
            CreateMap<AddCharacterDto, Character>();
            CreateMap<Weapon, GetWeaponDto>();
            CreateMap<Skill, GetSkillDto>();
            CreateMap<Character, HighScoreDto>();
        }
    }
}
