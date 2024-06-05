using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
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


        public TaskRepository taskRepository { get; set; }

        //public List<Project> Projects { get; set; }

        public TaskListUtilities(TaskRepository tr)
        {
            //Projects = new List<Project>();

            taskRepository = tr;
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
            Console.WriteLine("Welcome to Todoly you have X tasks todo and Y tasks are done.");
            Console.WriteLine("Pick an option:");
        }

         public void WriteMenu()
         {
             Console.WriteLine("(1) Show task List (by date or project)");
             Console.WriteLine("(2) Add New Task");
             Console.WriteLine("(3) Edit Task (update, mark as done, remove)");
             Console.WriteLine("(4) Save and Quit");
         }


        
           
          public void SuccessMessage()
          {
              Console.ForegroundColor = ConsoleColor.Green;
              Console.WriteLine("The Task was successfully added");
              Console.ResetColor();
              Console.WriteLine("-----------------------------------------");
          }


          public void AddTask(List<ProjectTask> tasks)
          {
              ReadAllDataForAddTask(tasks);
          }


          // Reads user input for a task: title, projectname, duedate
          // status will be set to 'started'
          public void ReadAllDataForAddTask(List<ProjectTask> tasks)
          {

                string dataTitle = "";
                string dataProjName = "";
                //string dataStatus = "";  // Behövs inte : sätt till "started"
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

                    ProjectTask t = new ProjectTask(dataTitle,dataProjName,TaskStatus.NotStarted,dueDt);

                    tasks.Add(t);

                    //write to file
                    //

                    //SaveDataToFile(tasks);   // "AutoSave" when change of Task-List is made

                    taskRepository.SaveTasksToFile(tasks);

                    SuccessMessage();
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
          public List<ProjectTask> DefaultSort(List<ProjectTask> tasks)
          {
                return tasks.OrderBy(item => item.TaskTitle).ToList();
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
              Console.WriteLine("Task".PadRight(15) + "Project".PadRight(15) + "Status".ToString().PadRight(12)+ "DueDate".ToString());
              Console.WriteLine("----".PadRight(15) + "-------".PadRight(15) + "------".ToString().PadRight(12) + "-------".ToString());
          }

          public void PrintAllTasks(List<ProjectTask> tasks, string search = "")
          {
      
              var defaultSorted = DefaultSort(tasks);   // default sort.ThenBy(item => item.ProductName).ToList();

              bool srch = (search != "") ? true : false;
              bool found = false;

              ListHeader();

              if (srch)
              {

                  //FirstOrDefault() returns the default value of the Data Type used if nothing is found,
                  //in case of Reference type like classes or objects it is NULL.

                  ProjectTask? searchP = defaultSorted.FirstOrDefault(item => item.TaskTitle.ToLower().Equals(search));
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

                  foreach (var task in defaultSorted) // Show List
                  {
                      string dt = task.DueDate.ToString("yyyy-MM-dd");
                      Console.WriteLine(task.TaskTitle.PadRight(15) + task.ProjectName.PadRight(15) + task.Status.ToString().PadRight(12)+ dt);
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
