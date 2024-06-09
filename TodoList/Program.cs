using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList;

void Main()
{

    TaskRepository tr = new TaskRepository();
   
    List<ProjectTask> tasks;

    int maxId;

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

        maxId = tr.ReadMaxId();  // Read maxId.txt
    }
    else
    {
        tasks = new List<ProjectTask>();   // TaskJson.data does not exist yet
        tr.WriteMaxId(0);   // create a txt-file with maxId = 0
        maxId = 0;
    }


    TaskListUtilities tu = new TaskListUtilities(tr);

   
    tu.WriteHeader(tasks);

    string choice = "";
    
    while(true)
    {
        tu.WriteMenu();
        choice = Console.ReadLine();

        if (choice.Trim().ToLower() == "1")       // 1 - Show List by Sort
        {
            string sort = tu.UserSortsList();
            if (sort!="q")
            {
                tu.PrintAllTasks(tasks,sort);
            }            
        }
        else if (choice.Trim().ToLower() == "2")    // 2 - Add new Task
        {          

            tu.AddTask(tasks, ref maxId);   

        }
        else if (choice.Trim().ToLower() == "3")     // 3 - Edit Task (update, mark as done, remove)
        {
            tu.PrintAllTasks(tasks, "p");
            tu.ChangeList(tasks);

        }
        else if (choice.Trim().ToLower() == "4")     // 4 = Save and Quit
        {
            tr.SaveTasksToFile(tasks);

            break;
        }
    }
}

Main();

