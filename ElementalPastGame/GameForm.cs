using ElementalPastGame.GameStateManagement;
using ElementalPastGame.KeyInput;
using ElementalPastGame.Rendering;
using System.Diagnostics;

namespace ElementalPastGame
{
    public partial class GameForm : Form, IPictureBoxManagerObserver
    {
        IPictureBoxManager pictureBoxManager = PictureBoxManager.GetInstance();
        IKeyEventPublisher eventPublisher = KeyEventPublisher.GetInstance();
        List<Keys> downKeys = new List<Keys>();
        List<char> pressedKeys = new List<char>();
        PictureBox pictureBox;
        internal DateTime DebugLastTickTime = DateTime.Now;
        public GameForm()
        {
            InitializeComponent();
            pictureBox = new PictureBox();
            pictureBox.Size = new Size(2000, 2000);
            pictureBox.Location = new Point(10, 10);
            this.Controls.Add(pictureBox);
            this.pictureBoxManager.AddIPictureBoxManagerObserver(this);
            // TODO: should be able to remove this line later. Just need it to start up the Player model here
            GameStateManager.getInstance();
        }

        // Key Interpretation

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (!this.downKeys.Contains(e.KeyCode)) {
                this.downKeys.Add(e.KeyCode);
            }
        }

        private void KeyIsPressed(object sender, KeyPressEventArgs e)
        {
            // No-op for keyIsPressed for the moment
            //this.eventPublisher.PublishKeyPressed(e.KeyChar);
            if (!this.pressedKeys.Contains(e.KeyChar))
            {
                this.pressedKeys.Add(e.KeyChar);
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            this.downKeys.Remove(e.KeyCode);
        }

        // Game run loop

        private void GameTick(object sender, EventArgs e)
        {
            if (this.pressedKeys.Count > 0)
            {
                foreach (char keyChar in this.pressedKeys)
                {
                    this.eventPublisher.PublishKeyPressed(keyChar);
                }
            }
            this.pressedKeys = new();
            this.eventPublisher.PublishKeysDown(this.downKeys);
        }

        private void GameForm_Leave(object sender, EventArgs e)
        {
            this.downKeys = new List<Keys>();
        }

        private void GameForm_Deactivate(object sender, EventArgs e)
        {
            this.downKeys = new List<Keys>();
        }

        public void IPictureBoxManagerNeedsRedraw()
        {
            this.pictureBox.Image = this.pictureBoxManager.ActiveScene;
        }
    }
}