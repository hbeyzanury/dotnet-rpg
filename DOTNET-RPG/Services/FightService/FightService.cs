using AutoMapper;
using DOTNET_RPG.Data;
using DOTNET_RPG.Dtos.Fight;
using DOTNET_RPG.Models;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DOTNET_RPG.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        
        public FightService(DataContext context,IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public FightService(DataContext context)
        {
            _context = context;
        }
        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
        {
            var response = new ServiceResponse<AttackResultDto>();
            try
            {
                var attacker = await _context.Characters
                    .Include(c => c.Weapon)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);

                var opponent = await _context.Characters
                .FirstOrDefaultAsync(c => c.Id == request.OpponentId);
                
                int damage = DoWeaponAttack(attacker, opponent);

                if (opponent.HitPoints <= 0)
                    response.Message = $"{opponent.Name} has been defeated";

                await _context.SaveChangesAsync();
                response.data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    AttackerHitPoint = attacker.HitPoints,
                    Opponent = opponent.Name,
                    OpponentHitPoints = opponent.HitPoints,
                    Damage = damage
                };
            }

            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;

        }

        private int DoWeaponAttack(Character attacker, Character opponent)
        {  
            
            int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strenght));
            damage -= new Random().Next(opponent.Defense);

            if (damage > 0)
                opponent.HitPoints -= damage;
            return damage;
           
        }

        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
        {
            var response = new ServiceResponse<AttackResultDto>();
            try
            {
                var attacker = await _context.Characters
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);

                var opponent = await _context.Characters
                .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                var skill = attacker.Skills.FirstOrDefault(s => s.Id == request.SkillId);
                if (skill == null)
                {
                    response.Success = false;
                    response.Message = $"{attacker.Name} doesn't know this skill.";
                    return response;
                }

                int damage = DoSkillAttack(attacker, opponent, skill);

                if (opponent.HitPoints <= 0)
                    response.Message = $"{opponent.Name} has been defeated";

                await _context.SaveChangesAsync();
                response.data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    AttackerHitPoint = attacker.HitPoints,
                    Opponent = opponent.Name,
                    OpponentHitPoints = opponent.HitPoints,
                    Damage = damage
                };
            }

            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;

        }

        private static int DoSkillAttack(Character attacker, Character opponent, Skill skill)
        {
            int damage = skill.Damage + (new Random().Next(attacker.Intelligence));
            damage -= new Random().Next(opponent.Defense);

            if (damage > 0)
                opponent.HitPoints -= damage;
            return damage;
        }

        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
        {
            var response = new ServiceResponse<FightResultDto>
            {
                data = new FightResultDto()
            };

            try
            {
                var characters = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .Where(c => request.CharacterIds.Contains(c.Id)).ToListAsync();
                bool defeated = false;
                while (!defeated)
                {
                    foreach(var attacker in characters)
                    {
                        var opponents = characters.Where(c => c.Id != attacker.Id).ToList();
                        var opponent = opponents[new Random().Next(opponents.Count)];

                        int damage = 0;
                        string attackUsed = string.Empty;

                        bool useWeapon = new Random().Next(2) == 0;
                        if (useWeapon)
                        {
                            attackUsed = attacker.Weapon.Name;
                            damage = DoWeaponAttack(attacker, opponent);
                        }
                        else
                        {
                            var skill = attacker.Skills[new Random().Next(attacker.Skills.Count)];
                            attackUsed = skill.Name;
                            damage = DoSkillAttack(attacker, opponent, skill);
                        }

                        response.data.Log
                            .Add($"{attacker.Name} attacks {opponent.Name} using {attackUsed} with {(damage >= 0 ? damage : 0)} damage");
                        if (opponent.HitPoints <= 0)
                        {
                            defeated = true;
                            attacker.Victories++;
                            opponent.Defeats++;

                            response.data.Log.Add($"{opponent.Name} has been defeated!");
                            response.data.Log.Add($"{attacker.Name} wins with {attacker.HitPoints} HP left!");
                            break;

                        }
                    }
                    
                }
                characters.ForEach(c =>
                {
                    c.Fights++;
                    c.HitPoints = 100;
                });

                await _context.SaveChangesAsync();
            }


            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<HighScoreDto>>> GetHighScore()
        {
            var characters = await _context.Characters
                .Where(c => c.Fights > 0)
                .OrderByDescending(c => c.Victories)
                .ThenBy(c => c.Defeats)
                .ToListAsync();
            var response = new ServiceResponse<List<HighScoreDto>>
            {
                data = characters.Select(c => _mapper.Map<HighScoreDto>(c)).ToList()

            };
            return response;
        }
    }
}
