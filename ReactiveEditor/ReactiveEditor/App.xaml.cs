using ReactiveEditor.Services;
using ReactiveEditor.UserControls;
using ReactiveEditor.ViewModels;
using ReactiveEditor.Views;
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

            Locator.CurrentMutable.InitializeSplat();
            Locator.CurrentMutable.InitializeReactiveUI();

            Locator.CurrentMutable.RegisterLazySingleton(() => new MyViewLocator(), typeof(IViewLocator));
            Locator.CurrentMutable.RegisterLazySingleton(() => new NavigationService(), typeof(INavigationService));

            Locator.CurrentMutable.Register(() => new EditHost(), typeof(IViewFor<EditVM>));

            Locator.CurrentMutable.Register(() => new MovableSquare(), typeof(IViewFor<SquareVM>));
            Locator.CurrentMutable.Register(() => new SquareEditor(), typeof(IViewFor<SquareVM>), "EditView");

            Locator.CurrentMutable.Register(() => new MovableTriangle(), typeof(IViewFor<TriangleVM>));
            Locator.CurrentMutable.Register(() => new TriangleEditor(), typeof(IViewFor<TriangleVM>), "EditView");

            Locator.CurrentMutable.Register(() => new MovableCircle(), typeof(IViewFor<CircleVM>));
            Locator.CurrentMutable.Register(() => new CircleEditor(), typeof(IViewFor<CircleVM>), "EditView");
        }
    }
}