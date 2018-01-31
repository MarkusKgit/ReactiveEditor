using ReactiveEditor.Helpers;
using ReactiveEditor.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReactiveEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewFor<MainWindowVM>
    {
        public MainWindow()
        {
            InitializeComponent();
            this.ViewModel = new MainWindowVM();
            this.DataContext = ViewModel;
            var rKeyPressed = this.Events().KeyDown.Where(e => e.Key == Key.R);
            this.WhenActivated(d =>
            {
                this.WhenAnyValue(x => x.drawArea.ActualWidth).Subscribe(w => ViewModel.DrawAreaWidth = w);
                this.WhenAnyValue(x => x.drawArea.ActualHeight).Subscribe(h => ViewModel.DrawAreaHeight = h);
                d.Invoke(this.Events().MouseLeftButtonDown.ToUnit().InvokeCommand(ViewModel.DeselectAllCommand));
                d.Invoke(this.Events().KeyDown.Where(e => e.Key == Key.Delete).ToUnit().InvokeCommand(ViewModel.DeleteSelectedCommand));
                d.Invoke(rKeyPressed.ToUnit().InvokeCommand(ViewModel.RotateSelectedCommand));
                d.Invoke(rKeyPressed.Subscribe(e => { e.Handled = true; }));
            });
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MainWindowVM)value; }
        }

        public MainWindowVM ViewModel
        {
            get { return (MainWindowVM)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
                    DependencyProperty.Register("ViewModel", typeof(MainWindowVM), typeof(MainWindow));
    }
}