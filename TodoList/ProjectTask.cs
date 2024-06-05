using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoList
{
    public class ProjectTask
    {
        public ProjectTask()
        {         
        }

        public ProjectTask(string taskTitle, Project project,TaskStatus status, DateTime dueDate)
        {
            TaskTitle = taskTitle;
            Project = project;     // projectName
            Status = status;
            DueDate = dueDate;
        }

        public string TaskTitle { get; set; }

        public Project Project { get; set; }

        public TaskStatus Status { get; set; }

        public DateTime DueDate { get; set; }

    }

    public enum TaskStatus
    {
        NotStarted,
        Started,
        Done
    }
}
