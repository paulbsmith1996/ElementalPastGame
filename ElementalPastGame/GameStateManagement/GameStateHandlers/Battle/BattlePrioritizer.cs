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
        internal Queue<EntityBattleModel> nextOrderedEntityModels = new();
        internal Dictionary<EntityBattleModel, int> priorityMap = new();

        public BattlePrioritizer(List<EntityBattleModel> EntityBattleDatas)
        {
            this.InitializePriorities(EntityBattleDatas);
            this.ComputeNextEntitiesForRange(ENTITY_QUEUE_LENGTH);
        }

        public EntityBattleModel PopNextEntityAndEnqueue()
        {
            this.EnqueueNextEntity();
            return this.nextOrderedEntityModels.Dequeue();
        }

        internal void InitializePriorities(List<EntityBattleModel> EntityBattleDatas)
        {
            foreach (EntityBattleModel dataModel in EntityBattleDatas)
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
            List<EntityBattleModel> EntityBattleDatas = this.priorityMap.Keys.ToList();
            EntityBattleModel nextEntity = EntityBattleDatas.First();
            int maxPriority = this.priorityMap[nextEntity];
            foreach (EntityBattleModel dataModel in EntityBattleDatas)
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

        internal void RecomputePrioritiesWithNextEntity(EntityBattleModel nextEntity)
        {
            foreach (EntityBattleModel dataModel in this.priorityMap.Keys.ToList())
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
