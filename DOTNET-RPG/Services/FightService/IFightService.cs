using DOTNET_RPG.Dtos.Fight;
using DOTNET_RPG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DOTNET_RPG.Services.FightService
{
    public interface IFightService
    {
        public Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request);

        public Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request);

        public Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request);

        public Task<ServiceResponse<List<HighScoreDto>>> GetHighScore();
    }
}
