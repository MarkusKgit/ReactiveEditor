using ReactiveEditor.ViewModels;
using ReactiveUI;
using System.Windows;

namespace ReactiveEditor.Views
{
    /// <summary>
    /// Interaction logic for EditHost.xaml
    /// </summary>
    public partial class EditHost : Window, IViewFor<EditVM>
    {
        public EditHost()
        {
            InitializeComponent();
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (EditVM)value; }
        }

        public EditVM ViewModel
        {
            get { return (EditVM)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
                    DependencyProperty.Register("ViewModel", typeof(EditVM), typeof(EditHost));

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}