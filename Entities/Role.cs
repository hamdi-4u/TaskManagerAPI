namespace TaskManagerAPI.Entities
{
    /// <summary>
    /// Defines the available user roles in the system.
    /// Determines access permissions and authorization levels.
    /// </summary>
    public enum Role
    {
      
        ///// Administrator role with full system access.
        /// Can create, read, update, and delete all users and tasks.
       
        Admin,

       
        ///// Regular user role with limited access.
        /// Can only view and update status of tasks assigned to them.
       
        User
    }
}