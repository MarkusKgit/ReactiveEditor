using ReactiveUI;
using ReactiveUI.Legacy;
using System.Collections.Generic;

namespace ReactiveEditor.ViewModels
{
    public interface IConnection : IVisual
    {
        IMovable FirstMovable { get; set; }
        IMovable SecondMovable { get; set; }
        ReactiveList<IMovable> InterPoints { get; set; }
    }
}