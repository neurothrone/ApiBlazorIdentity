using BlazorIdentity.Shared.Models;

namespace BlazorIdentity.Shared.Services;

public interface ISkillService
{
    Task<IEnumerable<Skill>> GetSkillsFromApiAsync();
    Task<IEnumerable<Skill>> GetSkillsFromServerAsync();
}