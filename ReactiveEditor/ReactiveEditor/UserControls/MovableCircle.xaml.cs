using ReactiveEditor.ViewModels;

namespace ReactiveEditor.UserControls
{
    //Since XAML can't handle generic UserControl define a non-generic class
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