namespace WebApp.Domain;

public class ApplicationRolePermission
{
    public Guid RoleId { get; set; }
    public int PermissionId { get; set; }

    public ApplicationRole Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}