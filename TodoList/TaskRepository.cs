using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TodoList
{
    public class TaskRepository
    {
        public readonly string DataFilePath = @"..\..\..\TaskJson.data";

        public readonly string MaxIdPath = @"..\..\..\MaxId.txt";


        //public int GetMaxId(List<ProjectTask> tasks)
        //{
        //    if (tasks.Count > 0)
        //    {
        //        int mId = tasks.Max(item => item.Id);
        //        return mId;
        //    }
        //    return 0;
        //}


        // Read maxId from MaxId.txt text-file
        public int ReadMaxId()
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
                }
                catch(Exception e)
                {
                    return 0;
                }
            }
            return 0;
        }


        // Saves new value of maxId.
        // Overwrites old value of MaxId.txt
        // If File does not exist it creates a new one 
        public void WriteMaxId(int maxId)
        {
            var path = MaxIdPath;
            File.WriteAllText(path, maxId.ToString());            
        }


        // Saves the Taskdata to TaskJson.data by serializing the List to json
        // Overwrites old value
        public void SaveTasksToFile(List<ProjectTask> tasks)
        {
            var path = DataFilePath;
            string json = JsonSerializer.Serialize(tasks);
            File.WriteAllText(path, json);
        }


        // Reads the json-data and recreates the List with help-class JsonFileReader
        public List<ProjectTask> ReadTasksFromFile()
        {
            var path = DataFilePath;
            List<ProjectTask> item = JsonFileReader.Read<List<ProjectTask>>(path);
            return item;
        }
    }
}
