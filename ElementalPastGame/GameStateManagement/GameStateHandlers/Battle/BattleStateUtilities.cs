using ElementalPastGame.Common;
using ElementalPastGame.GameObject.Entities;
using ElementalPastGame.Items.Equipment;
using ElementalPastGame.Items.Equipment.Weapons;
using ElementalPastGame.TileManagement.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static ElementalPastGame.Items.Equipment.Weapons.Weapon;

namespace ElementalPastGame.GameStateManagement.GameStateHandlers.Battle
{
    public class BattleStateUtilities
    {
        internal List<EntityBattleModel> allies { get; set; }
        internal List<EntityBattleModel> enemies { get; set; }
        internal Bitmap? background;

        Random rng = Random.Shared;

        internal static double HIGHEST_ROLL = 0.1;

        internal static double BASE_UNARMED_DAMAGE = 20;

        public BattleStateUtilities(List<EntityBattleModel> allies, List<EntityBattleModel> enemies)
        {
            this.allies = allies;
            this.enemies = enemies;
        }

        internal WeaponAction SelectRandomMoveForEntityBattleModel(EntityBattleModel battleModel)
        {
            ActiveEquipment? activeEquipment = battleModel.activeEquipment;
            Weapon? equippedWeapon = activeEquipment?.weapon;
            
            if (equippedWeapon == null)
            {
                return WeaponAction.Punch;
            }

            List<WeaponAction> potentialActions = equippedWeapon.multipliersForWeaponActions.Keys.ToList();
            return potentialActions.ElementAt(rng.Next(potentialActions.Count));
        }

        // Damage calculators

        internal int ComputePhysicalDamage(EntityBattleModel initiator, EntityBattleModel target, WeaponAction weaponAction)
        {
            ActiveEquipment? activeEquipment = initiator.activeEquipment;
            Weapon? equippedWeapon = activeEquipment?.weapon;

            int baseDamage = 20;
            double weaponMultiplier = 1.0;

            if (equippedWeapon != null)
            {
                baseDamage = equippedWeapon.baseDamage;
                weaponMultiplier = equippedWeapon.multipliersForWeaponActions.GetValueOrDefault(weaponAction);
                if (weaponMultiplier == 0)
                {
                    weaponMultiplier = 1.0;
                }
            }

            double strengthResistanceRatio = (float)initiator.strength / target.physicalResistance;
            double roll = (float)(rng.NextDouble() * BattleStateUtilities.HIGHEST_ROLL);
            return (int)((strengthResistanceRatio + roll + weaponMultiplier) * baseDamage);
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
