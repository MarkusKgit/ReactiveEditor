using ReactiveEditor.Helpers;
using ReactiveEditor.ViewModels;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReactiveEditor.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewFor<MainWindowVM>
    {
        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainWindowVM();
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);

            var rKeyPressed = this.Events().KeyDown.Where(e => e.Key == Key.R);

            //Set up disposable subscriptions
            this.WhenActivated(d =>
            {
                //Notify the ListView that an item has changed to update the info
                d.Invoke(
                    ViewModel
                    .SelectedMovables
                    .ItemChanged
                    .Subscribe(_ => this.SelectedInfoView.Items.Refresh())
                    );
                //Report Width and Height to the Viewmodel
                d.Invoke(
                    this.WhenAnyValue(x => x.drawArea.ActualWidth)
                    .Subscribe(w => ViewModel.DrawAreaWidth = w)
                    );
                d.Invoke(
                    this.WhenAnyValue(x => x.drawArea.ActualHeight)
                    .Subscribe(h => ViewModel.DrawAreaHeight = h)
                    );
                //Hook up events
                d.Invoke(
                    this.Events()
                    .MouseLeftButtonDown
                    .ToUnit()
                    .InvokeCommand(ViewModel.DeselectAllCommand)
                    );
                d.Invoke(
                    this.Events()
                    .KeyDown
                    .Where(e => e.Key == Key.Delete)
                    .ToUnit()
                    .InvokeCommand(ViewModel.DeleteSelectedCommand)
                    );
                d.Invoke(
                    rKeyPressed
                    .ToUnit()
                    .InvokeCommand(ViewModel.RotateSelectedCommand
                    ));
                //By default r changes focused element -> disable this behaviour by setting handled
                d.Invoke(
                    rKeyPressed
                    .Subscribe(e => { e.Handled = true; })
                    );
                //Edit element on double click
                d.Invoke(
                    drawArea.Events()
                    .MouseDoubleClick
                    .Select(_ => ViewModel.SelectedMovables.FirstOrDefault())
                    .InvokeCommand(ViewModel.EditCommand)
                    );
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