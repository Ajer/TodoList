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


        public int GetMaxId(List<ProjectTask> tasks)
        {
            if (tasks.Count > 0)
            {
                int mId = tasks.Max(item => item.Id);
                return mId;
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
            //JsonFileReader js = new JsonFileReader();
            List<ProjectTask> item = JsonFileReader.Read<List<ProjectTask>>(DataFilePath);
            return item;
        }
    }
}
