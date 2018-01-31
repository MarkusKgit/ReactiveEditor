using ReactiveEditor.ViewModels;

namespace ReactiveEditor.UserControls
{
    public class MovableCircleInternal : MovableControl<CircleVM>
    {
    }

    /// <summary>
    /// Interaction logic for MovableCircle.xaml
    /// </summary>
    public partial class MovableCircle : MovableCircleInternal
    {
        public MovableCircle()
        {
            InitializeComponent();
        }
    }
}