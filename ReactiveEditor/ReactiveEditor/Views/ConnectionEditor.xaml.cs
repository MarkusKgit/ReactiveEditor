using ReactiveEditor.ViewModels;
using ReactiveUI;

namespace ReactiveEditor.Views
{
    public class ConnectionEditorNonGeneric : ReactiveUserControl<ConnectionVM> { }

    /// <summary>
    /// Interaction logic for ConnectionEditor.xaml
    /// </summary>
    public partial class ConnectionEditor : ConnectionEditorNonGeneric
    {
        public ConnectionEditor()
        {
            InitializeComponent();
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);
        }
    }
}