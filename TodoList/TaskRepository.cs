using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TodoList
{
    // Responsible for all methods concerning read and write data to and from textfiles
    public class TaskRepository
    {
        public readonly string DataFilePath = @"..\..\..\TaskJson.data";        
        public readonly string MaxIdPath = @"..\..\..\MaxId.txt";


        // Read maxId from MaxId.txt text-file and return it.
        public int ReadMaxId(List<ProjectTask> tasks)
        {
            if (File.Exists(MaxIdPath))
            {
                try
                {
                    var path = MaxIdPath;
                    string text = File.ReadAllText(path);      
                    text = text.Trim();
                    int res;

                    bool ok = int.TryParse(text, out res);   // Parse to integer
                    if (ok)
                    {
                        return res;
                    }
                    else
                    {
                        Console.WriteLine("The MaxId file cannot be parsed as a number");
                        return 0;             // There is a MaxId-file but the characters can't be parsed to a number 
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Check The paths of the 2 files. Check files for illegal characters");
                    return 0;
                }
            }
            else   // MaxId does not exist and needs to be recreated with a newvalue of maxid 
            {
                if (tasks.Count > 0)
                {
                    int m = tasks.Max(item => item.Id);
                    WriteMaxId(m);
                    return m;
                }
                else
                {
                    WriteMaxId(0);
                    return 0;
                }
            }
        }


        // Saves new value of maxId.
        // Overwrites old value of MaxId.txt
        // If File does not exist it creates a new one 
        public bool WriteMaxId(int maxId)
        {
            try
            {
                var path = MaxIdPath;
                File.WriteAllText(path, maxId.ToString());
                return true;
            }
            catch (Exception)
            {
                return false;
            }                     
        }


        // Saves the Taskdata to TaskJson.data by serializing the List to json. 
        // If saving ok : returns true , if not returns false . Whole txt-file will be re-written.
        public bool SaveTasksToFile(List<ProjectTask> tasks)
        {
            try
            {
                var path = DataFilePath;
                string json = JsonSerializer.Serialize(tasks);
                File.WriteAllText(path, json);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }                
        }


        // Reads the json-data and recreates the List with JsonFileReader.Read
        public List<ProjectTask> ReadTasksFromFile()
        {
            var path = DataFilePath;
            List<ProjectTask> item = JsonFileReader.Read<List<ProjectTask>>(path);
            return item;
        }
    }

    public class JsonFileReader
    {
        // Returns list with type T =  List<ProjectTask>
        // after deserializing the json-objects in the text-file
        public static T Read<T>(string filePath)
        {
            string text = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(text);
        }
    }
}
