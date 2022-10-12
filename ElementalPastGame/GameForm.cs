using ElementalPastGame.GameObject;
using ElementalPastGame.KeyInput;
using ElementalPastGame.Rendering;
using System.Diagnostics;

namespace ElementalPastGame
{
    public partial class GameForm : Form, IPictureBoxManagerObserver
    {
        IPictureBoxManager pictureBoxManager = PictureBoxManager.GetInstance();
        IKeyEventPublisher eventPublisher = KeyEventPublisher.GetInstance();
        List<Keys> pressedKeys = new List<Keys>();
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
            GameObjectManager.getInstance();
        }

        // Key Interpretation

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (!this.pressedKeys.Contains(e.KeyCode)) {
                this.pressedKeys.Add(e.KeyCode);
            }
        }

        private void KeyIsPressed(object sender, KeyPressEventArgs e)
        {
            // No-op for keyIsPressed for the moment
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            this.pressedKeys.Remove(e.KeyCode);
        }

        // Game run loop

        private void GameTick(object sender, EventArgs e)
        {
            this.eventPublisher.PublishPressedKeys(this.pressedKeys);
        }

        private void GameForm_Leave(object sender, EventArgs e)
        {
            this.pressedKeys = new List<Keys>();
        }

        private void GameForm_Deactivate(object sender, EventArgs e)
        {
            this.pressedKeys = new List<Keys>();
        }

        public void IPictureBoxManagerNeedsRedraw()
        {
            this.pictureBox.Image = this.pictureBoxManager.ActiveScene;
        }
    }
}