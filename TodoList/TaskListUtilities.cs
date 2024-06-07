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
                     return data.Trim();
                }
                else if (data.Trim().ToLower() == "q")
                {
                     return "q";
                }
             }
             return "";
         }

         public void WriteHeader()
         {
            Console.WriteLine();
            Console.WriteLine("Welcome to Todoly you have X tasks todo and Y tasks are done.");
            Console.WriteLine("Pick an option:");
        }

         public void WriteMenu()
         {
             Console.WriteLine();
             Console.WriteLine("(1) Show task List (by date or project)");
             Console.WriteLine("(2) Add New Task");
             Console.WriteLine("(3) Edit Task (update, mark as done, remove)");
             Console.WriteLine("(4) Save and Quit");
         }


                
          public void SuccessMessage(string action)
          {
              Console.ForegroundColor = ConsoleColor.Green;
              Console.WriteLine("The Task was successfully " + action);   // action= "added" / "edited" etc
              Console.ResetColor();
              Console.WriteLine("-----------------------------------------");
          }


          public void AddTask(List<ProjectTask> tasks,ref int mIndex)
          {
              ReadAllDataForAddTask(tasks,ref mIndex);
          }


          private void RemoveTask(List<ProjectTask> tasks, int id, ref int mId)
          {
          }



          private void EditTask(List<ProjectTask> tasks, int id, ref int mId)
          {

            string dataEditParameter = "";
            bool dataEditParameterOk = false;

            Console.WriteLine("");

            while (!dataEditParameterOk)
            {
                dataEditParameter = ReadDataFromUser("Do you want to edit Due Date 'dt' , project 'p' , Title 't' or Status 's' ");            
                dataEditParameter = dataEditParameter.Trim().ToLower();

                if (dataEditParameter == "dt" || dataEditParameter == "p" || dataEditParameter == "t" || dataEditParameter == "s" || dataEditParameter == "q") 
                {
                    dataEditParameterOk = true;
                }
            }

            if (dataEditParameter.ToLower() != "q")   // Do the Edit
            {
              
                try
                {
                    ProjectTask pT = tasks.Find(item => item.Id == id);    //get task to edit

                    if (dataEditParameter == "dt")
                    {
                        Console.WriteLine("To be implemented");
                    }
                    else if (dataEditParameter == "p")
                    {
                        string projName = ReadDataFromUser("Give new value for projectname");
                        pT.Project.Name = projName;
                    }
                    else if (dataEditParameter == "t")
                    {
                        string taskTitle = ReadDataFromUser("Give new value for taskTitle");
                        pT.TaskTitle = taskTitle;
                    }
                    else   // "s"
                    {
                        bool statusOk = false;
                        string status = "";

                        while (!statusOk)
                        {
                            status = ReadDataFromUser("Give new value for status. Not Started 'ns', Started 's' or Done 'd'");
                            if (status=="ns" || status == "s" || status == "d")
                            {
                                statusOk = true;
                            }
                        }
                        if (status=="ns")
                        {
                            pT.Status = TaskStatus.NotStarted;
                        }
                        else if (status=="s")
                        {
                            pT.Status = TaskStatus.Started;
                        }
                        else if (status=="d")
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

                    Console.WriteLine("Something went wrong when editing tasks");
                }               

            }

          }




          // Reads user input for a task: title, projectname, duedate
          // status will be set to 'started'
          public void ReadAllDataForAddTask(List<ProjectTask> tasks, ref int mIndex)
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
                      if (ValidateDate(dataDueDate.Trim()) || dataDueDate.Trim().ToLower() == "q")
                      {
                           dateTimeOk = true;
                      }
                    }
                    if (dataDueDate.Trim().ToLower() == "q")
                    {
                         break;
                    }

                    // All datas here because no break has been performed

                    DateTime dueDt = Convert.ToDateTime(dataDueDate);

                    Project project = new Project(dataProjName);

                    mIndex = mIndex + 1;  // create new Id for task
                    ProjectTask t = new ProjectTask(mIndex,dataTitle,project,TaskStatus.NotStarted,dueDt);

                    tasks.Add(t);

                    //write to file
                    TaskRepository.SaveTasksToFile(tasks); // "AutoSave" when change of Task-List is made

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
                  dataSort = ReadDataFromUser("Write 'd' to show Tasks by Date ,'p' to show them by Project and " +
                    "'t' to show them by title");
                  dataSort = dataSort.Trim().ToLower();

                  if (dataSort == "d" || dataSort == "p" || dataSort == "t" || dataSort == "q")
                  {
                    dataSortOk = true;
                  }
              }
              return dataSort;
          }

        
          public void ChangeList(List<ProjectTask> tasks,ref int maxIndex)
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
                    dataId = dataId.Trim().ToLower();

                    if (dataId != "q")
                    {
                        
                        bool intOk = Int32.TryParse(dataId, out id);

                        if (intOk)
                        {
                            int index = tasks.FindIndex(item => item.Id == id);

                            if (index >= 0 )    // id exists
                            {
                                dataIdOk = true;
                            }
                            else  // -1
                            {
                                Console.WriteLine("The ID was not found");
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Not recognized");
                        }
                    }
                    else
                    {
                        dataIdOk = true;
                    }
                    
                }
                if (dataId=="q")
                {
                    break;
                }


                string dataEditOrRemove = "";
                bool dataEditOrRemoveOk = false;

                while (!dataEditOrRemoveOk)
                {
                    dataEditOrRemove = ReadDataFromUser("Do you want to Edit 'E' or Remove 'X' the task");
                    dataEditOrRemove = dataEditOrRemove.Trim().ToLower();

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

                    EditTask(tasks,id,ref maxIndex);


                }
                else   // remove task  "x"
                {
                    RemoveTask(tasks,id,ref maxIndex);
                }         
             }
          }

                 
          
          // Checks if a datetime-string of format "yyyy-MM-dd" is a valid date
          public bool ValidateDate(string str)
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
          public List<ProjectTask> TitleSort(List<ProjectTask> tasks)
          {
                return tasks.OrderBy(item => item.TaskTitle).ToList();
          }

          // Sort by ascending ProjectName
          public List<ProjectTask> ProjectSort(List<ProjectTask> tasks)
          {
             return tasks.OrderBy(item => item.Project.Name).ToList();
          }

          // Sort by ascending ProjectName
          public List<ProjectTask> DateSort(List<ProjectTask> tasks)
          {
              return tasks.OrderBy(item => item.DueDate).ToList();
          }


          public List<ProjectTask> GetSortedTasks(List<ProjectTask> tasks, string sort)
          { 
             if (sort=="p")
             {
                  return ProjectSort(tasks);
             }
             else if (sort=="d")
             {
                return DateSort(tasks);
             }
             else                  //   sort=="t"
             {
                return TitleSort(tasks);
             }
          }
          


          public string ReadSearchTask()
          {
              string? s = "";
              string str = "";
              while (true)
              {
                  Console.Write("Enter a product to search for: ");
                  s = Console.ReadLine();
                  str = s.Trim().ToLower(); 
                  if (!string.IsNullOrEmpty(s) || str.Equals("q"))
                  {
                      break;
                  }
              }
              return str;
          }



          public void ListHeader()
          {
              Console.WriteLine();
              Console.WriteLine("Id".PadRight(7) + "Task".PadRight(15) + "Project".PadRight(15) + "Status".ToString().PadRight(14)+ "DueDate".ToString());
              Console.WriteLine("---".PadRight(7) + "----".PadRight(15) + "-------".PadRight(15) + "------".ToString().PadRight(14) + "-------".ToString());
          }



          public void PrintAllTasks(List<ProjectTask> tasks, string sort, string search = "")
          {
      
              List<ProjectTask> sorted = GetSortedTasks(tasks,sort);   

              bool srch = (search != "") ? true : false;
              bool found = false;

              ListHeader();

              if (srch)
              {

                  //FirstOrDefault() returns the default value of the Data Type used if nothing is found,
                  //in case of Reference type like classes or objects it is NULL.

                  ProjectTask? searchP = sorted.FirstOrDefault(item => item.TaskTitle.ToLower().Equals(search));
                  if (searchP != null)     // searched for task found
                  {
                       found = true;
                  }
                  else
                  {
                       found = false;
                  }
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

                      Console.WriteLine(task.Id.ToString().PadRight(7) + task.TaskTitle.PadRight(15) + task.Project.Name.PadRight(15) + status.PadRight(14)+ dt);
                  }
              }
              Console.WriteLine();
              Console.WriteLine("---------------------------------------------");
        }
    }
    public class JsonFileReader
    {
        public static T Read<T>(string filePath)
        {
            string text = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(text);
        }
    }
}
