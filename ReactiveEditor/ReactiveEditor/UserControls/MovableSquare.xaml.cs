using ReactiveEditor.ViewModels;

namespace ReactiveEditor.UserControls
{
    public class MovableSquareInternal : MovableControl<SquareVM>
    {
    }

    /// <summary>
    /// Interaction logic for MovableRect.xaml
    /// </summary>
    public partial class MovableSquare : MovableSquareInternal
    {
        public MovableSquare()
        {
            InitializeComponent();
        }
    }
}