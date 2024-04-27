using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Domain;

namespace WebApp.Authorization;

internal class PermissionService : IPermissionService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public PermissionService(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<HashSet<string>> GetPermissionsAsync(Guid userId)
    {
        ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());

        ICollection<string> roles = await _userManager.GetRolesAsync(user);
        ICollection<string> permissions = await _dbContext.Set<ApplicationRole>()
            .Include(r => r.Permissions)
            .Where(x => roles.Contains(x.Name))
            .SelectMany(x => x.Permissions)
            .Select(x => x.Name).ToArrayAsync();

        return permissions.ToHashSet();
    }
}
