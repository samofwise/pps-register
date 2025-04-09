using Microsoft.EntityFrameworkCore;
using PPSRegister.Data;
using PPSRegister.Data.Models;

namespace PPSRegister.Api.Services;

public interface IPersonalPropertySecurityService
{
  Task<List<PersonalPropertySecurity>> GetPersonalPropertySecurities(int clientId);
  Task DeleteAll(int clientId);
}

public class PersonalPropertySecurityService(PPSRegisterDbContext _context) : IPersonalPropertySecurityService
{
  public async Task<List<PersonalPropertySecurity>> GetPersonalPropertySecurities(int clientId)
  {
    return await _context.PersonalPropertySecurities
        .Where(p => p.ClientId == clientId)
        .ToListAsync();
  }

  public async Task DeleteAll(int clientId)
  {
    var uploads = await _context.PersonalPropertySecurityUploads.Where(u => u.ClientId == clientId).ToListAsync();
    _context.PersonalPropertySecurityUploads.RemoveRange(uploads);

    var pps = await _context.PersonalPropertySecurities.Where(p => p.ClientId == clientId).ToListAsync();
    _context.PersonalPropertySecurities.RemoveRange(pps);
    await _context.SaveChangesAsync();
  }
}
