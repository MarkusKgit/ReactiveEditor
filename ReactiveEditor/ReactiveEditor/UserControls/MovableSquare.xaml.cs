using ReactiveEditor.ViewModels;

namespace ReactiveEditor.UserControls
{
    public class MovableSquareNonGeneric : MovableControl<SquareVM> { }

    /// <summary>
    /// Interaction logic for MovableRect.xaml
    /// </summary>
    public partial class MovableSquare : MovableSquareNonGeneric
    {
        public MovableSquare()
        {
            InitializeComponent();
        }
    }
}