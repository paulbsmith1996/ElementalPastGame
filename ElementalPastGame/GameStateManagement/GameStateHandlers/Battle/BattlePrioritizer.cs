using ElementalPastGame.GameObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameStateManagement.GameStateHandlers.Battle
{
    public class BattlePrioritizer
    {
        internal List<EntityDataModel> entityDataModels;
        internal Dictionary<EntityDataModel, int> priorityMap = new();

        public BattlePrioritizer(List<EntityDataModel> entityDataModels)
        {
            this.entityDataModels = entityDataModels;

            this.InitializePriorities();
        }

        public void InitializePriorities()
        {
            foreach (EntityDataModel dataModel in this.entityDataModels)
            {
                this.priorityMap.Add(dataModel, dataModel.agility);
            }
        }

        public EntityDataModel getNextDataModel()
        {
            EntityDataModel nextDataModel = this.entityDataModels.First();
            int maxPriority = this.priorityMap[nextDataModel];
            foreach (EntityDataModel dataModel in this.entityDataModels)
            {
                int priority = this.priorityMap[dataModel];
                if (priority > maxPriority)
                {
                    nextDataModel = dataModel;
                    maxPriority = priority;
                }
            }

            return nextDataModel;
        }
    }
}
