using ElementalPastGame.GameObject;
using ElementalPastGame.KeyInput;
using ElementalPastGame.Rendering;

namespace ElementalPastGame
{
    public partial class GameForm : Form, IPictureBoxManagerObserver
    {
        IPictureBoxManager pictureBoxManager = PictureBoxManager.GetInstance();
        IKeyEventPublisher eventPublisher = KeyEventPublisher.GetInstance();
        List<Keys> pressedKeys = new List<Keys>();
        PictureBox pictureBox;
        public GameForm()
        {
            InitializeComponent();
            pictureBox = new PictureBox();
            pictureBox.Size = new Size(2000, 2000);
            pictureBox.Location = new Point(10, 10);
            this.pictureBoxManager.AddIPictureBoxManagerObserver(this);
            this.Controls.Add(pictureBox);
            //pictureBoxManager.AddIPictureBoxManagerObserver(this);
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

        // IPictureBoxManagerObserver
        //public void IPictureBoxManagerDidAddPictureBox(IPictureBoxManager pictureBoxManager, PictureBox pictureBox)
        //{
        //    Console.WriteLine("Tried to add " + pictureBox);
        //    this.Controls.Add(pictureBox);
        //}

        //public void IPictureBoxManagerDidRemovePictureBox(IPictureBoxManager pictureBoxManager, PictureBox pictureBox)
        //{
        //    this.Controls.Remove(pictureBox);
        //}

        //public void IPictureBoxManagerDidUpdatePictureBox(IPictureBoxManager pictureBoxManager, PictureBox pictureBox)
        //{
        //    // TODO: Figure out if we actually need this callback
        //    this.Controls.Add(pictureBox);
        //}

        // Game run loop

        private void GameTick(object sender, EventArgs e)
        {
            this.eventPublisher.PublishPressedKeys(this.pressedKeys);
            //this.pictureBox.Image = this.pictureBoxManager.ActiveScene;
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