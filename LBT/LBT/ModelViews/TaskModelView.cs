using LBT.Cache;
using LBT.Controllers;
using LBT.Models;
using System.Linq;

namespace LBT.ModelViews
{
    public sealed class TaskIndex : BaseModelView
    {
        public PeopleContactTask[] ActualTasks { get; set; }

        public PeopleContactTask[] DelayedTasks { get; set; }

        private TaskIndex()
        {
        }

        public static TaskIndex GetViewModel(BaseController baseController)
        {
            PeopleContactTask[] peopleContactTasks = PeopleContactCache.GetPeopleContactTasks(baseController.Db, baseController.UserId);
            var taskIndex = new TaskIndex
            {
                ActualTasks = peopleContactTasks.Where(pct => pct.PeopleContactTaskType == PeopleContactTaskType.Actual).ToArray(),
                DelayedTasks = peopleContactTasks.Where(pct => pct.PeopleContactTaskType == PeopleContactTaskType.Delayed).ToArray()
            };

            return taskIndex;
        }
    }
}