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


        public string ReadDataFromUser(string userAction)
        {
            Console.Write(userAction + ": ");    //exempel  userAction = "Enter a Category"
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

        public void WriteHeader(List<ProjectTask> tasks)
        {
            int done = tasks.FindAll(item => item.Status == TaskStatus.Done).Count();
            int notDone = tasks.Count - done;  // "notstarted" and "started"

            Console.WriteLine();
            Console.WriteLine("Welcome to Todoly you have " + notDone.ToString() + " tasks todo and " + done.ToString() + " tasks are done.");
            Console.WriteLine("Pick an option:");
        }



        public void WriteMenu()
        {
            Console.WriteLine();
            Console.WriteLine("(1) Show task List (by date, by project or by title)");
            Console.WriteLine("(2) Add New Task");
            Console.WriteLine("(3) Edit Task (update, mark as done, remove)");
            Console.WriteLine("(4) Save and Quit");
        }



        private void SuccessMessage(string action)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("The Task was successfully " + action);   // action= "added" / "edited" etc
            Console.ResetColor();
            Console.WriteLine("-----------------------------------------");
        }


        public void AddTask(List<ProjectTask> tasks, ref int mId)
        {
            ReadAllDataForAddTask(tasks, ref mId);
        }


        private void RemoveTask(List<ProjectTask> tasks, int id)
        {
            bool dataDeleteOk = false;
            string dataDelete = "";

            Console.WriteLine("");

            while (!dataDeleteOk)
            {
                dataDelete = ReadDataFromUser("Are You sure you want to delete the task with id = " + id + "? Yes 'Y'/'y' . Abort 'A'/'a'");
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
                        TaskRepository.SaveTasksToFile(tasks);   // List has been changed. Save new List to file.
                        SuccessMessage("removed");             // The maxId stays the same even if we have erased this id here
                    }
                }
                catch (Exception e)
                {
                }
            }
        }



        private void EditTask(List<ProjectTask> tasks, int id)
        {

            string dataEditParameter = "";
            bool dataEditParameterOk = false;

            Console.WriteLine("");

            while (!dataEditParameterOk)
            {
                dataEditParameter = ReadDataFromUser("Do you want to edit Due Date 'dd' , project 'p' , Title 't' or Status 's' ");
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
                        string projName = ReadDataFromUser("Give a new value for projectname");
                        pT.Project.Name = projName;
                    }
                    else if (dataEditParameter == "t")
                    {
                        string taskTitle = ReadDataFromUser("Give a new value for taskTitle");
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
                    TaskRepository.SaveTasksToFile(tasks);   // List has been changed. Save new List to file.
                    SuccessMessage("edited");
                }
                catch (Exception e)
                {

                    Console.WriteLine("Something went wrong when editing tasks." + e.Message);
                }

            }

        }




        // Reads user input for a task: title, projectname, duedate
        // status will be set to 'started'
        public void ReadAllDataForAddTask(List<ProjectTask> tasks, ref int mId)
        {

            string dataTitle = "";
            string dataProjName = "";
            //string dataStatus = "";  // Behövs inte : sätt till "NotStarted"
            string dataDueDate = "";


            while (true)
            {
                bool dataTitleOk = false;

                Console.WriteLine();
                Console.WriteLine("Write q to quit");

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

                tasks.Add(t);

                //write to file
                TaskRepository.SaveTasksToFile(tasks); // "AutoSave" when change of Task-List is made
                TaskRepository.WriteMaxId(mId);    // Also update record of maxId on file since we dont wanna reuse id's

                SuccessMessage("added");
            }
        }


        public string UserSortsList()
        {
            string dataSort = "";
            bool dataSortOk = false;

            Console.WriteLine();
            Console.WriteLine("Write q to quit");

            while (!dataSortOk)
            {
                dataSort = ReadDataFromUser("Write 'd' to show/sort Tasks by Date ,'p' by Project, " +
                  "'t' by title and 's' by status");
                dataSort = dataSort.ToLower();

                if (dataSort == "d" || dataSort == "p" || dataSort == "t" || dataSort == "s" || dataSort == "q")
                {
                    dataSortOk = true;
                }
            }
            return dataSort;
        }


        public void ChangeList(List<ProjectTask> tasks)
        {

            Console.WriteLine();


            while (true)
            {
                string dataId = "";
                bool dataIdOk = false;

                int id = 0;

                while (!dataIdOk)
                {
                    Console.WriteLine("Write q to quit");
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



        //public string ReadSearchTask()
        //{
        //    string? s = "";
        //    string str = "";
        //    while (true)
        //    {
        //        Console.Write("Enter a product to search for: ");
        //        s = Console.ReadLine();
        //        str = s.Trim().ToLower();
        //        if (!string.IsNullOrEmpty(s) || str.Equals("q"))
        //        {
        //            break;
        //        }
        //    }
        //    return str;
        //}



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



        public void PrintAllTasks(List<ProjectTask> tasks, string sort, string search = "")
        {

            List<ProjectTask> sorted = GetSortedTasks(tasks, sort);

            bool srch = (search != "") ? true : false;
            bool found = false;

            ListHeader(sort);        // show the sort-method by quotation-mark

            if (srch)
            {

                //FirstOrDefault() returns the default value of the Data Type used if nothing is found,
                //in case of Reference type like classes or objects it is NULL.

                //ProjectTask? searchP = sorted.FirstOrDefault(item => item.TaskTitle.ToLower().Equals(search));
                //if (searchP != null)     // searched for task found
                //{
                //    found = true;
                //}
                //else
                //{
                //    found = false;
                //}
            }

            if (srch && found)
            {

                //foreach (var prod in defaultSorted)
                //{

                //    if (prod.ProductName.ToLower().Equals(search))
                //    {
                //        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                //        Console.WriteLine(prod.Category.PadRight(20) + prod.ProductName.PadRight(20) + prod.Price);
                //        Console.ResetColor();
                //    }
                //    else
                //    {
                //        Console.WriteLine(prod.Category.PadRight(20) + prod.ProductName.PadRight(20) + prod.Price);
                //    }
                //}
            }
            else if (srch && !found)
            {
                //Console.WriteLine("The Item was not found");

                //foreach (var prod in defaultSorted) // Show List
                //{
                //    Console.WriteLine(prod.Category.PadRight(20) + prod.ProductName.PadRight(20) + prod.Price);
                //}

            }
            else   // normal-display of List  (without search)
            {

                foreach (var task in sorted) // Show List
                {
                    string dt = task.DueDate.ToString("yyyy-MM-dd");
                    string status = (task.Status == TaskStatus.NotStarted) ? "Not Started" : task.Status.ToString();

                    Console.WriteLine(task.Id.ToString().PadRight(7) + task.TaskTitle.PadRight(25) + task.Project.Name.PadRight(23) + status.PadRight(15) + dt);
                }
            }
            Console.WriteLine();
            Console.WriteLine("---------------------------------------------");
        }
    }


    //public class JsonFileReader
    //{
    //    // Returns class T , for instance the list T =  List<ProjectTask>
    //    public static T Read<T>(string filePath)
    //    {
    //        string text = File.ReadAllText(filePath);
    //        return JsonSerializer.Deserialize<T>(text);
    //    }
    //}
}
