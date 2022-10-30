using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using ElementalPastGame.Common;
using ElementalPastGame.Components;
using ElementalPastGame.Components.ComponentSequences;
using ElementalPastGame.Rendering;
using static System.Windows.Forms.Design.AxImporter;

namespace ElementalPastGame.GameObject.GameStateHandlers
{
    public class BattleGameStateHandler : IGameStateHandler
    {
        internal enum BattleState
        {
            Start,
            MoveSelection,
            MoveResolution,
            End,
        }
        public IGameStateHandlerDelegate? gameStateHandlerDelegate { get; set; }

        internal BattleState state;
        internal InteractableTextComponentTree? textComponents;

        public BattleGameStateHandler()
        {
            this.state = BattleState.Start;
        }

        public void HandleKeyInputs(List<Keys> keyCodes)
        {
            switch (this.state)
            {
                case BattleState.Start:
                    this.GetTextComponents().HandleKeyInputs(keyCodes);
                    break;
                case BattleState.MoveSelection:
                    break;
                case BattleState.MoveResolution:
                    break;
                case BattleState.End:
                    break;
            }
            this.Redraw();

        }

        internal InteractableTextComponentTree GetTextComponents()
        {
            if (this.textComponents != null)
            {
                return this.textComponents;
            }

            int textBoxHeight = 125;
            GameTextBox firstBox = new("You encountered an enemy.", 0, CommonConstants.GAME_DIMENSION - textBoxHeight - 4, CommonConstants.GAME_DIMENSION, textBoxHeight);
            GameTextBox secondBox = new("Prepare for battle.", 0, CommonConstants.GAME_DIMENSION - textBoxHeight - 4, CommonConstants.GAME_DIMENSION, textBoxHeight);
            TextComponentTreeTextBoxNode firstBoxNode = new TextComponentTreeTextBoxNode(firstBox);
            TextComponentTreeTextBoxNode secondBoxNode = new TextComponentTreeTextBoxNode(secondBox);

            List<String> subOptions1 = new() { "Suboption0_0", "Suboption0_1", "Suboption0_2", "Suboption0_3" };
            List<String> subOptions2 = new() { "Suboption1_0", "Suboption1_1", "Suboption1_2", "Suboption2_3" };
            List<String> subOptions3 = new() { "Suboption2_0", "Suboption2_1", "Suboption2_2", "Suboption2_3" };

            TextMenu subMenu1 = new(subOptions1, true, 40, 0);
            TextMenu subMenu2 = new(subOptions2, true, 40, 0);
            TextMenu subMenu3 = new(subOptions3, true, 40, 0);
            TextMenu menu = new(false, 0, 0);
            menu.AddOptionWithKey(subMenu1, "Option1");
            menu.AddOptionWithKey(subMenu2, "Option2");
            menu.AddOptionWithKey(subMenu3, "Option3");

            firstBoxNode.SetChild(secondBoxNode);
            secondBoxNode.SetChild(menu);
            this.textComponents = new InteractableTextComponentTree(firstBoxNode);
            return this.textComponents;
        }

        internal void Redraw()
        {
            this.UpdateBackground();
            this.UpdateForeground();

            if (this.gameStateHandlerDelegate != null)
            {
                this.gameStateHandlerDelegate.IGameStateHandlerNeedsRedraw(this);
            }
        }

        internal void UpdateBackground()
        {
            Bitmap emptyBitmap = new Bitmap(CommonConstants.GAME_DIMENSION, CommonConstants.GAME_DIMENSION);
            Graphics graphics = Graphics.FromImage(emptyBitmap);
            graphics.FillRectangle(Brushes.Black, 0, 0, CommonConstants.GAME_DIMENSION, CommonConstants.GAME_DIMENSION);

            RenderingModel backgroundRenderingModel = new()
            {
                X = 0,
                Y = 0,
                Width = CommonConstants.GAME_DIMENSION,
                Height = CommonConstants.GAME_DIMENSION,
                Bitmaps = new List<Bitmap>() { emptyBitmap }
            };

            if (this.gameStateHandlerDelegate != null)
            {
                this.gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, backgroundRenderingModel);
            }

        }

        internal void UpdateForeground()
        {
            if (this.gameStateHandlerDelegate != null)
            {
                this.gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, this.GetTextComponents().GetRenderingModel());
            }
        }
    }
}
