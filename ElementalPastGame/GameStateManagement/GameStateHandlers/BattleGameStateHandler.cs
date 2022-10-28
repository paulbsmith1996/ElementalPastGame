using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using ElementalPastGame.Common;
using ElementalPastGame.Components;
using ElementalPastGame.Components.ComponentSequences;

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
        internal InteractableTextComponentTree textComponents;

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

            if (gameStateHandlerDelegate != null) {
                gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, this.GetTextComponents().GetRenderingModel());
                gameStateHandlerDelegate.IGameStateHandlerNeedsRedraw(this);
            }

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

            firstBoxNode.SetChild(secondBoxNode);
            this.textComponents = new InteractableTextComponentTree(firstBoxNode);
            return this.textComponents;
        }
    }
}
