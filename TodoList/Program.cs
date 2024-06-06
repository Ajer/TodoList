using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList;

void Main()
{

    TaskRepository tr = new TaskRepository();
    TaskListUtilities tu = new TaskListUtilities(tr);

    List<ProjectTask> tasks;


    if (File.Exists(tr.DataFilePath))   // TaskJson.data exists
    {
        try
        {
            tasks = tr.ReadTasksFromFile();  // Load values
        }
        catch (Exception e)
        {
            tasks = null;
            Console.WriteLine("An error has ocurred while reading data from file: " + e.Message);
            Environment.Exit(0);
        }
    }
    else
    {
        tasks = new List<ProjectTask>();   // TaskJson.data does not exist yet
    }


    int maxIndex = tr.GetMaxIndex(tasks);


    tu.WriteHeader();

    string choice = "";
    
    while(true)   //while (choice.Trim().ToLower() != "q")
    {
        tu.WriteMenu();
        choice = Console.ReadLine();
        if (choice.Trim().ToLower() == "1")
        {   

            tu.PrintAllTasks(tasks);
        
        }
        else if (choice.Trim().ToLower() == "2")    // 2 - Add new Task
        {
          
            tu.AddTask(tasks, ref maxIndex);
            
        }
        else if (choice.Trim().ToLower() == "3")     // 3 - Edit Task (update, mark as done, remove)
        {
            
        }
        else if (choice.Trim().ToLower() == "4")     // 4 = Save and Quit
        {
            tr.SaveTasksToFile(tasks);

            break;
        }
    }
}

Main();

