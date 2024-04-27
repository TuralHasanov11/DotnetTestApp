namespace WebApp.Domain;

public enum Permissions
{
    Null = 0,
    CourseView = 1
}


public sealed class Permission
{
    public Permission(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;

    public ICollection<ApplicationRole> Roles { get; } = [];
    public ICollection<ApplicationRolePermission> RolePermissions { get; } = [];
}
