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

                    bool ok = int.TryParse(text, out res);
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

        // Overwrites old value
        // If File does not exist it creates a new one 
        public void WriteMaxId(int maxId)
        {
            var path = MaxIdPath;
            File.WriteAllText(path, maxId.ToString());            
        }


        public void SaveTasksToFile(List<ProjectTask> tasks)
        {
            var path = DataFilePath;
            string json = JsonSerializer.Serialize(tasks);
            File.WriteAllText(path, json);
        }

        public List<ProjectTask> ReadTasksFromFile()
        {
            var path = DataFilePath;
            List<ProjectTask> item = JsonFileReader.Read<List<ProjectTask>>(path);
            return item;
        }
    }
}
