using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElementalPastGame.Common;
using ElementalPastGame.Components;

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
        internal GameTextBox textBox;

        public BattleGameStateHandler()
        {
            this.state = BattleState.Start;
            int textBoxHeight = 125;
            textBox = new GameTextBox("This is a test text box with more text. Battle is starting.", 0, CommonConstants.GAME_DIMENSION - textBoxHeight - 4, CommonConstants.GAME_DIMENSION, textBoxHeight);
        }

        public void HandleKeyInputs(List<Keys> keyCodes)
        {
            switch (this.state)
            {
                case BattleState.Start:
                    this.HandleStartSequence();
                    break;
                case BattleState.MoveSelection:
                    break;
                case BattleState.MoveResolution:
                    break;
                case BattleState.End:
                    break;
            }

            if (gameStateHandlerDelegate != null) {
                gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, textBox.getRenderingModel());
                gameStateHandlerDelegate.IGameStateHandlerNeedsRedraw(this);
            }

        }
    }
}
