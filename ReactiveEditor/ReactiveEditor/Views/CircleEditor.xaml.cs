using ReactiveEditor.ViewModels;
using ReactiveUI;

namespace ReactiveEditor.Views
{
    public class CircleEditorNonGeneric : ReactiveUserControl<CircleVM> { }

    /// <summary>
    /// Interaction logic for CircleEditor.xaml
    /// </summary>
    public partial class CircleEditor : CircleEditorNonGeneric
    {
        public CircleEditor()
        {
            InitializeComponent();
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);
        }
    }
}