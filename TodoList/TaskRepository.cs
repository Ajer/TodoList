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


        public int GetMaxIndex(List<ProjectTask> tasks)
        {
            if (tasks.Count > 0)
            {
                int ind = tasks.Max(item => item.Id);
                return ind;
            }
            return 0;
        }


        public void SaveTasksToFile(List<ProjectTask> tasks)
        {
            string json = JsonSerializer.Serialize(tasks);
            File.WriteAllText(DataFilePath, json);
        }

        public List<ProjectTask> ReadTasksFromFile()
        { 
            List<ProjectTask> item = JsonFileReader.Read<List<ProjectTask>>(DataFilePath);
            return item;
        }
    }
}
