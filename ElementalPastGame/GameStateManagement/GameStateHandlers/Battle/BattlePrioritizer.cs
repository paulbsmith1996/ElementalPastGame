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
        public static int ENTITY_QUEUE_LENGTH = 6;
        // This list will always contain the following ENTITY_QUEUE_LENGTH entities to act. 
        internal Queue<EntityDataModel> nextOrderedEntityModels = new();
        internal Dictionary<EntityDataModel, int> priorityMap = new();

        public BattlePrioritizer(List<EntityDataModel> entityDataModels)
        {
            this.InitializePriorities(entityDataModels);
            this.ComputeNextEntitiesForRange(ENTITY_QUEUE_LENGTH);
        }

        public EntityDataModel PopNextEntityAndEnqueue()
        {
            this.EnqueueNextEntity();
            return this.nextOrderedEntityModels.Dequeue();
        }

        internal void InitializePriorities(List<EntityDataModel> entityDataModels)
        {
            foreach (EntityDataModel dataModel in entityDataModels)
            {
                this.priorityMap.Add(dataModel, dataModel.agility);
            }
        }

        internal void ComputeNextEntitiesForRange(int range)
        {
            for (int rangeIndex = 0; rangeIndex < range; rangeIndex++)
            {
                this.EnqueueNextEntity();
            }
        }

        internal void EnqueueNextEntity()
        {
            List<EntityDataModel> entityDataModels = this.priorityMap.Keys.ToList();
            EntityDataModel nextEntity = entityDataModels.First();
            int maxPriority = this.priorityMap[nextEntity];
            foreach (EntityDataModel dataModel in entityDataModels)
            {
                int priority = this.priorityMap[dataModel];
                if (priority > maxPriority)
                {
                    nextEntity = dataModel;
                    maxPriority = priority;
                }
            }

            this.RecomputePrioritiesWithNextEntity(nextEntity);

            this.nextOrderedEntityModels.Enqueue(nextEntity);
        }

        internal void RecomputePrioritiesWithNextEntity(EntityDataModel nextEntity)
        {
            foreach (EntityDataModel dataModel in this.priorityMap.Keys.ToList())
            {
                if (dataModel.Equals(nextEntity))
                {
                    this.priorityMap[dataModel] = 0;
                }
                else
                {
                    this.priorityMap[dataModel] += dataModel.agility;
                }
            }
        }
    }
}
