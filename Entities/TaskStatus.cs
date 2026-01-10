namespace TaskManagerAPI.Entities
{
  

    /// Tracks progression from creation to completion.
    public enum TaskStatus
    {
        
        ///// Task has been created but work has not yet started.
        Pending = 0,


       
        //// Task is currently being worked on.
        InProgress = 1,

       
        ////// Task has been finished and all work is done.
        Completed = 2
    }
}