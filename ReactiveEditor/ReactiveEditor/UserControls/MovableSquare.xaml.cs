using ReactiveEditor.ViewModels;

namespace ReactiveEditor.UserControls
{
    /// <summary>
    /// Interaction logic for MovableRect.xaml
    /// </summary>
    public partial class MovableSquare : MovableControl<SquareVM>
    {
        public MovableSquare()
        {
            InitializeComponent();
        }
    }
}