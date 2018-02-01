using ReactiveEditor.ViewModels;

namespace ReactiveEditor.UserControls
{
    public class MovableTriangleNonGeneric : MovableControl<TriangleVM> { }

    /// <summary>
    /// Interaction logic for MovableTriangle.xaml
    /// </summary>
    public partial class MovableTriangle : MovableTriangleNonGeneric
    {
        public MovableTriangle()
        {
            InitializeComponent();
        }
    }
}