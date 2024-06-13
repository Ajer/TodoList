using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList;

void Main()
{

    TaskRepository tr = new TaskRepository();
    
    List<ProjectTask> tasks;

    int maxId = 0;

    if (File.Exists(tr.DataFilePath))   // TaskJson.data exists
    {
        try
        {
            tasks = tr.ReadTasksFromFile();    // Load task-data
            maxId = tr.ReadMaxId(tasks);      // load maxId
        }
        catch (Exception e)
        {
            tasks = null;
            Console.WriteLine("An error has ocurred while reading data from file. Check both files for path and illegal characters. " + e.Message);
            Environment.Exit(0);
        }

       // maxId = tr.ReadMaxId();   // Read maxId.txt . The maxId should be equal or bigger than the highest id in TaskJson.data
    }
    else
    {
        tasks = new List<ProjectTask>();   // TaskJson.data does not exist yet. (Will be created when the first task is created).
        tr.WriteMaxId(0);   // Set maxId = 0 in MaxId.txt
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
            tu.UserChoiceHeader("Show List Of Tasks");
            string sort = tu.UserSortsList();
            if (sort!="q")
            {
                
                tu.PrintAllTasks(tasks,sort);
            }            
        }
        else if (choice.Trim().ToLower() == "2")    // 2 - Add new Task
        {
            tu.UserChoiceHeader("Add a new Task");
            tu.AddTask(tasks, ref maxId);   

        }
        else if (choice.Trim().ToLower() == "3")     // 3 - Edit Task (update, mark as done, remove)
        {
            tu.UserChoiceHeader("Edit Task / Remove Task");
            tu.PrintAllTasks(tasks, "d");  // p
            tu.ChangeTask(tasks);

        }
        else if (choice.Trim().ToLower() == "4")     // 4 = Quit  (All saved in TaskListUtilities)
        {
            //try
            //{ 
            //    bool ok = tr.SaveTasksToFile(tasks);

            //    if (!ok)
            //    {
            //        tu.FailMessage("Saving and Closing the program");
            //        Console.WriteLine("Please check MaxId.txt and TaskJson.data");
            //    }
            //    break;
            //}
            //catch (Exception)
            //{
            //    tu.FailMessage("Saving and Closing the program");
            //    Console.WriteLine("Please check MaxId.txt and TaskJson.data");
            //}

            break;
        }
    }
}

Main();

