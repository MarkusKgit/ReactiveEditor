using ReactiveUI;
using Splat;

namespace ReactiveEditor.Services
{
    public class MyViewLocator : IViewLocator
    {
        public IViewFor ResolveView<T>(T viewModel, string contract = null) where T : class
        {
            var type = typeof(IViewFor<>).MakeGenericType(viewModel.GetType());
            return Locator.Current.GetService(type, contract) as IViewFor;
        }
    }
}