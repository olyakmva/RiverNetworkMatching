
namespace KeyPointApp.Controls
{
    public partial class ParamControl : UserControl
    {
        public double BendDistance
        {
            get
            {
                return double.Parse(distanceNumUpDown.Text);
            }
        }
    
        public ParamControl()
        {
            InitializeComponent();
        }
    }
}
