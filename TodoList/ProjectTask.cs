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

        public ProjectTask(string taskTitle, string projectName,TaskStatus status, DateTime dueDate)
        {
            TaskTitle = taskTitle;
            ProjectName = projectName;
            Status = status;
            DueDate = dueDate;
        }

        public string TaskTitle { get; set; }

        public string ProjectName { get; set; }

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
