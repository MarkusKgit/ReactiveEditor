using ReactiveUI;
using System;

namespace ReactiveEditor.ViewModels
{
    public class SquareVM : RectangleVM
    {
        public SquareVM()
        {
            //Since this is supposed to be a square always make sure height and width are equal
            this.WhenAnyValue(x => x.Width).Subscribe(w => this.Height = w);
            this.WhenAnyValue(x => x.Height).Subscribe(h => this.Width = h);
        }
    }
}