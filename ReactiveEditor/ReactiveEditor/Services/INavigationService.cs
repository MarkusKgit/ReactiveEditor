using ReactiveEditor.ViewModels;

namespace ReactiveEditor.Services
{
    public interface INavigationService
    {
        void ShowModalEditView(EditVM editVM);
    }
}