using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TodoList
{
    public class TaskListUtilities
    {

        public TaskRepository TaskRepository { get; set; }



        //public List<Project> Projects { get; set; }


        public TaskListUtilities(TaskRepository tr)
        {
            //Projects = new List<Project>();

            TaskRepository = tr;
        }


        // HelpClass which let the user input an answer-string to a question
        // The answer-string is returned
        private string ReadDataFromUser(string userAction)
        {
            Console.Write(userAction + ": ");    //example  userAction = "Enter a ProjectName"
            string? data = Console.ReadLine();

            if (data != null)
            {
                if (data.Trim().ToLower() != "q")   // data Ok
                {
                    return data.Trim();    // data is returned trimmed
                }
                else if (data.Trim().ToLower() == "q")
                {
                    return "q";
                }
            }
            return "";
        }


        // Writes the Welcome-note when the program starts
        public void WriteWelcomeHeader(List<ProjectTask> tasks)
        {
            int done = tasks.FindAll(item => item.Status == TaskStatus.Done).Count();  // done
            int notDone = tasks.Count - done;   // "notstarted" and "started"

            string nd_tsk = (notDone != 1) ? "tasks" : "task";
            string d_tsk = (done != 1) ? "tasks" : "task";

            Console.WriteLine();
            Console.WriteLine("Welcome to Todoly you have " + notDone.ToString() + " " + nd_tsk + " todo and " + done.ToString() + " " + d_tsk+ " are done.");
            Console.WriteLine("Pick an option:");

        }


        // Writes the 4 main choices for the user
        public void WriteMenu()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
           
            Console.WriteLine();
            Console.WriteLine("(1) Show task List (by date, by project, by title or by status)");
            Console.WriteLine("(2) Add New Task");
            Console.WriteLine("(3) Edit Task (update, mark as done, remove)");
            Console.WriteLine("(4) Save and Quit");

            Console.ResetColor();
        }


        // Message when an action is completed
        public void SuccessMessage(string action)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("The Task was successfully " + action);   // action= "added" / "edited" etc
            Console.ResetColor();
            Console.WriteLine("-------------------------------------");
        }

        // Errorhandling message
        public void FailMessage(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Something went wrong when " + msg + ". Contact dev if problem persist");   // msg= "" / "edited" etc
            Console.ResetColor();
            Console.WriteLine("-------------------------------------");
        }


        // Lets the user decide if he wants to goahead with the removal of a Task with a certain id
        // If 'Y/y': goes ahead with removal . if 'A/a' or 'Q/q': the process is aborted  
        private void RemoveTask(List<ProjectTask> tasks, int id)
        {
            bool dataDeleteOk = false;
            string dataDelete = "";

            Console.WriteLine("");

            while (!dataDeleteOk)
            {
                dataDelete = ReadDataFromUser("Are You sure you want to Delete the task with id = " + id + "?  YES 'Y'/'y' .ABORT 'A'/'a'");
                dataDelete = dataDelete.ToLower();

                if (dataDelete == "y" || dataDelete == "a" || dataDelete == "q")
                {
                    dataDeleteOk = true;
                }
            }
            if (dataDelete != "a" && dataDelete != "q")   // Do the Edit
            {

                try
                {
                    ProjectTask pT = tasks.Find(item => item.Id == id);    //get task to remove. We know id exists here from ChangeLIst

                    bool ok = tasks.Remove(pT);
                    if (ok)
                    {
                        bool ok2 = TaskRepository.SaveTasksToFile(tasks);   // List has been changed. Save new List to file.
                        if (ok2)
                        {
                            SuccessMessage("removed");     // The maxId stays the same even if we have erased this id here. (Simulate DB)                   
                        }
                        else
                        {
                            FailMessage("removing a task");
                        }
                    }
                    else
                    {
                        FailMessage("removing a task");
                    }
                }
                catch (Exception e)
                {
                    FailMessage("removing a task");
                    Console.WriteLine(e.Message);
                }
            }
        }


        // Lets the user decide what param to edit and then does the editing
        private void EditTask(List<ProjectTask> tasks, int id)
        {

            string dataEditParameter = "";
            bool dataEditParameterOk = false;

            Console.WriteLine("");

            while (!dataEditParameterOk)
            {
                dataEditParameter = ReadDataFromUser("Do you want to edit Due Date 'dd' , Project 'p' , Title 't' or Status 's' ");
                dataEditParameter = dataEditParameter.ToLower();

                if (dataEditParameter == "dd" || dataEditParameter == "p" || dataEditParameter == "t" || dataEditParameter == "s" || dataEditParameter == "q")
                {
                    dataEditParameterOk = true;
                }
            }

            if (dataEditParameter.ToLower() != "q")   // Do the Edit
            {

                try
                {
                    ProjectTask pT = tasks.Find(item => item.Id == id);    //get task to edit, we know id exists here from ChangeLIst

                    if (dataEditParameter == "dd")
                    {
                        string dataDueDate = "";
                        bool dateTimeOk = false;

                        while (!dateTimeOk)
                        {
                            dataDueDate = ReadDataFromUser("Enter a new value for DueDate in format YYYY-MM-DD");
                            if (ValidateDate(dataDueDate))
                            {
                                dateTimeOk = true;
                            }
                        }
                        DateTime dueDt = Convert.ToDateTime(dataDueDate);
                        pT.DueDate = dueDt;

                    }
                    else if (dataEditParameter == "p")
                    {
                        string projName = "";
                        bool projNameOk = false;

                        while (!projNameOk)
                        {
                            projName = ReadDataFromUser("Give a new value for projectName");
                            if (projName!="")
                            {
                                projNameOk = true;
                            }
                        }

                        pT.Project.Name = projName;  // we have a non-blank-value
                    }
                    else if (dataEditParameter == "t")
                    {

                        string taskTitle = "";
                        bool taskTitleOk = false;

                        while (!taskTitleOk)
                        {
                            taskTitle = ReadDataFromUser("Give a new value for taskTitle");
                            if (taskTitle!= "")
                            {
                                taskTitleOk = true;
                            }
                        }

                        pT.TaskTitle = taskTitle;
                    }
                    else   // "s"
                    {
                        bool statusOk = false;
                        string status = "";

                        while (!statusOk)
                        {
                            status = ReadDataFromUser("Give new value for status. Not Started 'ns', Started 's' or Done 'd'");
                            status = status.ToLower();

                            if (status == "ns" || status == "s" || status == "d")
                            {
                                statusOk = true;
                            }
                        }
                        if (status == "ns")
                        {
                            pT.Status = TaskStatus.NotStarted;
                        }
                        else if (status == "s")
                        {
                            pT.Status = TaskStatus.Started;
                        }
                        else if (status == "d")
                        {
                            pT.Status = TaskStatus.Done;
                        }
                    }

                    // write to file
                    bool ok = TaskRepository.SaveTasksToFile(tasks);   // List has been changed. Save new List to file.
                    if (ok)
                    {
                        SuccessMessage("edited");
                    }
                    else
                    {
                        FailMessage("editing a task");
                    }
                }
                catch (Exception e)
                {
                    FailMessage("editing a task");
                    Console.WriteLine(e.Message);
                }

            }

        }


        // Start method accessible from program-class for adding a new task.
        public void AddTask(List<ProjectTask> tasks, ref int mId)
        {
            ReadAllDataForAddTask(tasks, ref mId);
        }



        // Reads user input for a task: title, projectname, duedate
        // Status will be set to 'NotStarted'
        private void ReadAllDataForAddTask(List<ProjectTask> tasks, ref int mId)
        {

            string dataTitle = "";
            string dataProjName = "";
            //string dataStatus = "";      // Initiate to TaskStatus.NotStarted
            string dataDueDate = "";


            while (true)
            {
                bool dataTitleOk = false;

                Console.WriteLine();
                //Console.WriteLine("Write q to quit");
                QuitCue();

                while (!dataTitleOk)
                {
                    dataTitle = ReadDataFromUser("Enter a Task Title");
                    if (dataTitle != "")  // either "q" or other not empty data makes user break out of this loop
                    {
                        dataTitleOk = true;
                    }
                }
                if (dataTitle.ToLower() == "q")
                {
                    break;
                }

                bool projNameOk = false;
                while (!projNameOk)
                {
                    dataProjName = ReadDataFromUser("Enter a Project Name");
                    if (dataProjName != "")
                    {
                        projNameOk = true;
                    }
                }
                if (dataProjName.ToLower() == "q")
                {
                    break;
                }

                bool dateTimeOk = false;
                while (!dateTimeOk)
                {
                    dataDueDate = ReadDataFromUser("Enter a DueDate in format YYYY-MM-DD");
                    if (ValidateDate(dataDueDate) || dataDueDate.ToLower() == "q")
                    {
                        dateTimeOk = true;
                    }
                }
                if (dataDueDate.ToLower() == "q")
                {
                    break;
                }

                // All datas here because no break has been performed

                DateTime dueDt = Convert.ToDateTime(dataDueDate);

                Project project = new Project(dataProjName);

                mId = mId + 1;    // create new Id for task
                ProjectTask t = new ProjectTask(mId, dataTitle, project, TaskStatus.NotStarted, dueDt);

               
                try
                {
                    tasks.Add(t);

                    //write to file
                    bool okSttf = TaskRepository.SaveTasksToFile(tasks); // "AutoSave" when change of Task-List is made
                    bool okWmi = TaskRepository.WriteMaxId(mId);    // Also update record of maxId on file since we added task. (we dont wanna reuse id's)


                    if (okSttf && okWmi)
                    {
                        SuccessMessage("added");
                    }
                    else
                    {
                        FailMessage("adding a task");
                    }
                }
                catch (Exception e)
                {
                    FailMessage("adding a task");
                    Console.WriteLine(e.Message);
                }               
            }
        }


        // Informs the user of the possibility to quit
        private void QuitCue()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Write q to quit");
            Console.ResetColor();
        }


         // Lets the user input the preferred sorting
        // Returns the string associated with the particular sort: 'd' ,'p' ,'t','s'
        public string UserSortsList()
        {
            string dataSort = "";
            bool dataSortOk = false;

            Console.WriteLine();

            //Console.WriteLine("Write q to quit");
            QuitCue();

            //Console.WriteLine();

            while (!dataSortOk)
            {
                dataSort = ReadDataFromUser("Write 'd' to sort by Date ,'p' by Project, " +
                  "'t' by Tasktitle or 's' by Status");
                dataSort = dataSort.ToLower();

                if (dataSort == "d" || dataSort == "p" || dataSort == "t" || dataSort == "s" || dataSort == "q")
                {
                    dataSortOk = true;
                }
            }
            return dataSort;
        }


        // Start method accessible from program-class for editing / deleting a task
        public void ChangeTask(List<ProjectTask> tasks)
        {

            Console.WriteLine();


            while (true)
            {
                string dataId = "";
                bool dataIdOk = false;

                int id = 0;

                while (!dataIdOk)
                {
                    Console.WriteLine();
                    //Console.WriteLine("Write q to quit");

                    QuitCue();
                   
                    dataId = ReadDataFromUser("Write the Id-number for the task you want to change");
                    dataId = dataId.ToLower();

                    if (dataId != "q")
                    {

                        bool intOk = int.TryParse(dataId, out id);

                        if (intOk)
                        {
                            int index = tasks.FindIndex(item => item.Id == id);

                            if (index >= 0)    // id exists
                            {
                                dataIdOk = true;
                            }
                            else  // -1
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("The ID was not found");
                                Console.ResetColor();
                            }
                        }
                        // else : not recognized as id
                    }
                    else
                    {
                        dataIdOk = true;
                    }
                }

                if (dataId == "q")
                {
                    break;
                }


                string dataEditOrRemove = "";
                bool dataEditOrRemoveOk = false;

                while (!dataEditOrRemoveOk)
                {
                    dataEditOrRemove = ReadDataFromUser("Do you want to Edit 'E' or Remove 'X' the task");
                    dataEditOrRemove = dataEditOrRemove.ToLower();

                    if (dataEditOrRemove == "e" || dataEditOrRemove == "x" || dataEditOrRemove == "q")
                    {
                        dataEditOrRemoveOk = true;
                    }
                }
                if (dataEditOrRemove == "q")
                {
                    break;
                }

                // here dataEditOrRemove is either "e"  or "x" 
                if (dataEditOrRemove == "e")   // edit task
                {

                    EditTask(tasks, id);

                }
                else   // remove task  "x"
                {
                    RemoveTask(tasks, id);

                    break;
                }
            }
        }



        // Checks if a datetime-string of format "yyyy-MM-dd" is a valid date.
        // For instance: YYYY-04-30 is valid but YYYY-04-31 is not
        private bool ValidateDate(string str)
        {
            DateTime dt;
            string[] formats = { "yyyy-MM-dd" };
            if (DateTime.TryParseExact(str, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt))
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        // Sort by ascending Title
        private List<ProjectTask> TitleSort(List<ProjectTask> tasks)
        {
            return tasks.OrderBy(item => item.TaskTitle).ToList();
        }

        // Sort by ascending ProjectName
        private List<ProjectTask> ProjectSort(List<ProjectTask> tasks)
        {
            return tasks.OrderBy(item => item.Project.Name).ToList();
        }

        // Sort by ascending date
        private List<ProjectTask> DateSort(List<ProjectTask> tasks)
        {
            return tasks.OrderBy(item => item.DueDate).ToList();
        }

        // Sort by alphabetical status:  Done,NotStarted,Started
        private List<ProjectTask> StatusSort(List<ProjectTask> tasks)
        {
            return tasks.OrderBy(item => item.Status.ToString()).ToList();
        }

        // Help-Method to PrintAllTasks who returns the sort wanted 
        private List<ProjectTask> GetSortedTasks(List<ProjectTask> tasks, string sort)
        {
            if (sort == "p")
            {
                return ProjectSort(tasks);
            }
            else if (sort == "d")
            {
                return DateSort(tasks);
            }
            else if (sort == "s")
            {
                return StatusSort(tasks);
            }
            else       //   sort=="t"
            {
                return TitleSort(tasks);
            }
        }

        // Writes the Header of the main-alternative 1-4
        public void UserChoiceHeader(string choice)
        {
            //Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine();
            Console.WriteLine(choice);

            //Console.ResetColor();
        }


        // Writes the headers for the different task-params
        private void ListHeader(string sort)
        {
            string taskString = "Task";
            string projString = "Project";
            string dueString = "DueDate";
            string statusString = "Status";

            if (sort == "p")
            {
                projString = "Project'";
            }
            else if (sort == "d")
            {
                dueString = "DueDate'";
            }
            else if (sort == "t")
            {
                taskString = "Task'";
            }
            else if (sort == "s")
            {
                statusString = "Status'";
            }

            Console.WriteLine();
            Console.WriteLine("Id".PadRight(7) + taskString.PadRight(25) + projString.PadRight(23) + statusString.ToString().PadRight(15) + dueString.ToString());
            Console.WriteLine("---".PadRight(7) + "----".PadRight(25) + "-------".PadRight(23) + "------".ToString().PadRight(15) + "-------".ToString());
        }


        // Prints the List according to the sort
        public void PrintAllTasks(List<ProjectTask> tasks, string sort)
        {

            List<ProjectTask> sorted = GetSortedTasks(tasks, sort);

            ListHeader(sort);        // show the sort-method by quotation-mark

            if (tasks.Count() > 0)
            {
                int cmpTime = 10;    //  days
                foreach (var task in sorted) // Show List
                {
                    string dt = task.DueDate.ToString("yyyy-MM-dd");
                    string status = (task.Status == TaskStatus.NotStarted) ? "Not Started" : task.Status.ToString();

                    //int t1 = GetTimeSpanInDays(task.DueDate);

                    //if (t1 <= 10 && t1 >= 0)   // tasks within 10 days before duedate (incl duedate) become red. "Red window of dates" 
                    //{                           // Older and Younger tasks remain white. 
                    //    Console.ForegroundColor = ConsoleColor.Red;
                    //}

                    if (task.Status == TaskStatus.Done)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }


                    Console.WriteLine(task.Id.ToString().PadRight(7) + task.TaskTitle.PadRight(25) + task.Project.Name.PadRight(23) + status.PadRight(15) + dt);

                    Console.ResetColor();
                }
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine(" No items here yet");
            }
            
            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------------------------------------");
        }

        //private int GetTimeSpanInDays(DateTime dt)
        //{
        //    TimeSpan ts = dt - DateTime.Now;
        //    return ts.Days;
        //}
    }
}
