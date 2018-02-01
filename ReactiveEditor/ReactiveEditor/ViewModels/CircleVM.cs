using ReactiveUI;
using System;

namespace ReactiveEditor.ViewModels
{
    public class CircleVM : MovableVM
    {
        public CircleVM()
        {
            //Since this is supposed to be a circle always make sure height and width are equal
            this.WhenAnyValue(x => x.Width).Subscribe(w => this.Height = w);
            this.WhenAnyValue(x => x.Height).Subscribe(h => this.Width = h);
        }
    }
}