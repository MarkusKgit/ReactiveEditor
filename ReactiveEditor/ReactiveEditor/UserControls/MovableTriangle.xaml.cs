using ReactiveEditor.ViewModels;

namespace ReactiveEditor.UserControls
{
    //Since XAML can't handle generic UserControl define a non-generic class
    public class MovableTriangleInternal : MovableControl<TriangleVM>
    {
    }

    /// <summary>
    /// Interaction logic for MovableTriangle.xaml
    /// </summary>
    public partial class MovableTriangle : MovableTriangleInternal
    {
        public MovableTriangle()
        {
            InitializeComponent();
        }
    }
}