using ReactiveEditor.ViewModels;
using ReactiveUI;

namespace ReactiveEditor.Views
{
    public class SquareEditorNonGeneric : ReactiveUserControl<SquareVM> { }

    /// <summary>
    /// Interaction logic for SquareEditor.xaml
    /// </summary>
    public partial class SquareEditor : SquareEditorNonGeneric
    {
        public SquareEditor()
        {
            InitializeComponent();
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);
        }
    }
}