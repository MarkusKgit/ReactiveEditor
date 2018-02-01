using ReactiveEditor.ViewModels;

namespace ReactiveEditor.UserControls
{
    //Since XAML can't handle generic UserControl define a non-generic class
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