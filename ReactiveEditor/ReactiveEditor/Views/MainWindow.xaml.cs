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
        private readonly Key[] disabledKeys = { Key.Left, Key.Right, Key.Up, Key.Down, Key.PageUp, Key.PageDown, Key.Home, Key.End };

        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainWindowVM();
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);

            var rKeyPressed = this.Events().KeyDown.Where(e => e.Key == Key.R);
            var cKeyPressed = this.Events().KeyDown.Where(e => e.Key == Key.C);

            //Set up disposable subscriptions
            this.WhenActivated(d =>
            {
                //Notify the ListView that an item has changed to update the info
                d.Invoke(
                    ViewModel
                    .SelectedVisuals
                    .ItemChanged
                    .Throttle(TimeSpan.FromMilliseconds(100))
                    .ObserveOn(RxApp.MainThreadScheduler)
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
                    .TouchDown
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
                    cKeyPressed
                    .ToUnit()
                    .InvokeCommand(ViewModel.ConnectCommand)
                    );
                d.Invoke(
                    rKeyPressed
                    .ToUnit()
                    .InvokeCommand(ViewModel.RotateSelectedCommand
                    ));
                //By default r and c and arrow keys change focused element -> disable this behaviour by setting handled
                d.Invoke(
                    rKeyPressed
                    .Merge(cKeyPressed)
                    .Merge(this.Events().PreviewKeyDown.Where(x => disabledKeys.Contains(x.Key)))
                    .Subscribe(e => { e.Handled = true; })
                    );
                //Edit element on double click
                d.Invoke(
                    drawArea.Events()
                    .MouseDoubleClick
                    .Select(_ => ViewModel.SelectedVisuals.FirstOrDefault())
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