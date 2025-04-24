using Engine;

namespace SuperAdventure
{
    public partial class SuperAdventure : Form
    {
        private Player _player = new Player(10, 10, 20, 0, 1);

        public SuperAdventure()
        {
            InitializeComponent();

            lblHitPoints.Text = _player.CurrentHitPoints.ToString();
            lblGold.Text = _player.Gold.ToString();
            lblExperience.Text = _player.ExperiencePoints.ToString();
            lblLevel.Text = _player.Level.ToString();

            Location location = new Location(1, "Home", "This is your house.");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
