using ReactiveEditor.ViewModels;
using ReactiveUI;

namespace ReactiveEditor.UserControls
{
    public class ConnectionControlNonGeneric : ReactiveUserControl<ConnectionVM> { }

    /// <summary>
    /// Interaction logic for ConnectionControl.xaml
    /// </summary>
    public partial class ConnectionControl : ConnectionControlNonGeneric
    {
        public ConnectionControl()
        {
            InitializeComponent();
            this.WhenAnyValue(x => x.DataContext).BindTo(this, x => x.ViewModel);
        }
    }
}