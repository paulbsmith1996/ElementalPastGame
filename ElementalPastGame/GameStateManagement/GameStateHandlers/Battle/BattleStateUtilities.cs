using ElementalPastGame.Common;
using ElementalPastGame.GameObject.Entities;
using ElementalPastGame.TileManagement.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameStateManagement.GameStateHandlers.Battle
{
    public class BattleStateUtilities
    {

        internal List<EntityBattleData> allies { get; set; }
        internal List<EntityBattleData> enemies { get; set; }
        internal Bitmap? background;
        Random rng = Random.Shared;
        internal static double HIGHEST_ROLL = 0.1;
        public BattleStateUtilities(List<EntityBattleData> allies, List<EntityBattleData> enemies)
        {
            this.allies = allies;
            this.enemies = enemies;
        }

        // Damage calculators

        internal int ComputePhysicalDamage(EntityBattleData initiator, EntityBattleData target, BattleMoves.PhysicalAttackMove move)
        {
            double multiplier = (double)initiator.strength / target.physicalResistance;
            double roll = rng.NextDouble() * BattleStateUtilities.HIGHEST_ROLL;
            int baseDamage = initiator.activeEquipment.weapon.baseDamage;
            return (int)((multiplier + roll) * baseDamage);
        }

        // Position calculators

        internal float GetBackgroundYForLineUpIndex(int lineUpIndex, bool isEnemy)
        {
            int lineUpCount = isEnemy ? enemies.Count : allies.Count;
            return ((float)lineUpIndex + 1) * BattleStateConstants.BACKGROUND_HEIGHT / (lineUpCount + 1) + BattleStateConstants.BACKGROUND_Y;
        }

        internal Point ComputeRenderLocationForLineUpIndex(int lineUpIndex, bool isEnemy)
        {
            float gameObjectY = GetBackgroundYForLineUpIndex(lineUpIndex, isEnemy);

            double perspectiveFactor = Math.Pow(ComputeSqueezeFactor(gameObjectY), 1.1);
            int entityWidth = (int)(CommonConstants.BATTLE_PARTICIPANT_DIMENSION * perspectiveFactor);
            int entityHeight = (int)(CommonConstants.BATTLE_PARTICIPANT_DIMENSION * perspectiveFactor);

            int gameObjectX = ComputeBackgroundXForPoint(new() { X = BattleStateConstants.GAME_OBJECT_HORIZONTAL_PADDING, Y = gameObjectY }, isEnemy);

            int xOnBackground;
            bool isEvenRank = lineUpIndex % 2 == 0;
            bool isOnLeftSide = gameObjectX < BattleStateConstants.PERSPECTIVE.X;

            if (isEvenRank && isOnLeftSide)
            {
                xOnBackground = gameObjectX;
            }
            else if (isEvenRank && !isOnLeftSide)
            {
                xOnBackground = gameObjectX - entityWidth;
            }
            else if (!isEvenRank && isOnLeftSide)
            {
                xOnBackground = gameObjectX + BattleStateConstants.GAME_OBJECT_HORIZONTAL_PADDING / 2;
            }
            else
            {
                xOnBackground = gameObjectX - entityWidth - BattleStateConstants.GAME_OBJECT_HORIZONTAL_PADDING / 2;
            }

            return new Point() { X = xOnBackground, Y = (int)gameObjectY - entityHeight };
        }

        internal int ComputeBackgroundXForPoint(PointF point, bool isEnemy)
        {
            float squeezeFactor = ComputeSqueezeFactor(point.Y);

            float newX;
            int midX = BattleStateConstants.PERSPECTIVE.X;
            if (point.X <= midX)
            {
                newX = midX - (midX - point.X) * squeezeFactor;
            }
            else
            {
                newX = midX + (point.X - midX) * squeezeFactor;
            }


            return isEnemy ? CommonConstants.GAME_DIMENSION - (int)newX : (int)newX;
        }

        internal float ComputeSqueezeFactor(float y)
        {
            float normalizedY = y - BattleStateConstants.BACKGROUND_Y + BattleStateConstants.PERSPECTIVE.Y;
            float normalizedHeight = BattleStateConstants.BACKGROUND_HEIGHT + BattleStateConstants.PERSPECTIVE.Y;
            return normalizedY / normalizedHeight;
        }

        internal Bitmap GetBackground()
        {
            if (this.background != null)
            {
                return this.background;
            }

            this.background = TextureFactory.TessalatedTexture(TextureMapping.Mapping[TextureMapping.Dirt], BattleStateConstants.BACKGROUND_WIDTH, BattleStateConstants.BACKGROUND_HEIGHT, BattleStateConstants.PERSPECTIVE);
            return this.background;
        }
    }
}
