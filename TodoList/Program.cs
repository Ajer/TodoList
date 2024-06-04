using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList;

void Main()
{

    TaskListUtilities tu = new TaskListUtilities();

    List<ProjectTask> tasks;

    if (File.Exists(tu.DataFilePath))   // json.data exists
    {
        tasks = tu.ReadDataFromFile();  // Read all values and assign
    }
    else
    {
        tasks = new List<ProjectTask>();   // json.data does not exist yet
    }

    tu.WriteHeader();
    tu.WriteMenu();

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
          
            tu.AddTask(tasks);
            
        }
        else if (choice.Trim().ToLower() == "3")     // 3 - Edit Task (update, mark as done, remove)
        {
            tu.ReadDataFromFile();
            
            //Console.WriteLine("3");
        }
        else if (choice.Trim().ToLower() == "4")     // 4 = Save and Quit
        {
            tu.SaveDataToFile(tasks);

            break;
        }
    }
}

Main();

