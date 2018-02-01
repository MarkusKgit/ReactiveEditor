using ReactiveEditor.UserControls;
using ReactiveEditor.ViewModels;
using ReactiveUI;
using Splat;
using System.Windows;

namespace ReactiveEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Locator.CurrentMutable.Register(() => new MovableSquare(), typeof(IViewFor<SquareVM>));
            Locator.CurrentMutable.Register(() => new MovableTriangle(), typeof(IViewFor<TriangleVM>));
            Locator.CurrentMutable.Register(() => new MovableCircle(), typeof(IViewFor<CircleVM>));
        }
    }
}