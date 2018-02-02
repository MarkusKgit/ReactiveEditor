using ReactiveEditor.ViewModels;
using ReactiveUI;
using Splat;
using System.Windows;

namespace ReactiveEditor.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IViewLocator viewLocator;

        public NavigationService(IViewLocator viewLocator = null)
        {
            this.viewLocator = viewLocator ?? Locator.CurrentMutable.GetService<IViewLocator>();
        }

        public void ShowModalEditView(EditVM editVM)
        {
            var editWindow = viewLocator.ResolveView(editVM);
            editWindow.ViewModel = editVM;
            var result = (editWindow as Window).ShowDialog();
            if (result.HasValue && result.Value)
            {
                editVM.ApplyChanges();
            }
        }
    }
}