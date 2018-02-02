using ReactiveEditor.ViewModels;
using ReactiveUI;

namespace ReactiveEditor.Views
{
    public class TriangleEditorNonGeneric : ReactiveUserControl<TriangleVM> { }

    /// <summary>
    /// Interaction logic for TriangleEditor.xaml
    /// </summary>
    public partial class TriangleEditor : TriangleEditorNonGeneric
    {
        public TriangleEditor()
        {
            InitializeComponent();
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);
        }
    }
}