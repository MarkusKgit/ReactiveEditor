using ReactiveUI;
using System;

namespace ReactiveEditor.ViewModels
{
    public class SquareVM : RectangleVM
    {
        public SquareVM()
        {
            this.WhenAnyValue(x => x.Width).Subscribe(w => this.Height = w);
            this.WhenAnyValue(x => x.Height).Subscribe(h => this.Width = h);
        }
    }
}