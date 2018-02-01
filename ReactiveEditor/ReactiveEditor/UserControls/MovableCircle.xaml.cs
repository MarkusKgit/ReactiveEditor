using ReactiveEditor.ViewModels;

namespace ReactiveEditor.UserControls
{
    public class MovableCircleNonGeneric : MovableControl<CircleVM> { }

    /// <summary>
    /// Interaction logic for MovableCircle.xaml
    /// </summary>
    public partial class MovableCircle : MovableCircleNonGeneric
    {
        public MovableCircle()
        {
            InitializeComponent();
        }
    }
}