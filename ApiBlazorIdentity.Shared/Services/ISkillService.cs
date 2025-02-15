using ApiBlazorIdentity.Shared.Models;

namespace ApiBlazorIdentity.Shared.Services;

public interface ISkillService
{
    Task<IEnumerable<Skill>> GetSkillsFromApiAsync();
    Task<IEnumerable<Skill>> GetSkillsFromServerAsync();
}