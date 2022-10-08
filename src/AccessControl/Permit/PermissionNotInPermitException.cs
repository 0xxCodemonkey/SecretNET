namespace SecretNET.AccessControl;

/// <summary>
/// Class PermissionNotInPermitException.
/// Implements the <see cref="Exception" />
/// </summary>
/// <seealso cref="Exception" />
public class PermissionNotInPermitException : Exception
{
    /// <summary>
    /// Gets the permissions.
    /// </summary>
    /// <value>The permissions.</value>
    public PermissionType[] Permissions { get; private set; }

    /// <summary>
    /// Gets the permissions in contract.
    /// </summary>
    /// <value>The permissions in contract.</value>
    public string[] PermissionsInContract { get; private set; }


    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionNotInPermitException"/> class.
    /// </summary>
    /// <param name="permissions">The permissions.</param>
    /// <param name="permissionsInContract">The permissions in contract.</param>
    public PermissionNotInPermitException(PermissionType[] permissions, string[] permissionsInContract)
    {
        Permissions = permissions;
        PermissionsInContract = permissionsInContract;
    }
}
