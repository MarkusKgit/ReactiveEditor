namespace ReactiveEditor.ViewModels
{
    public interface IVisual : IDuplicatable
    {
        double Left { get; set; }
        double Top { get; set; }
        double Right { get; }
        double Bottom { get; }
        double Height { get; set; }
        double Width { get; set; }

        bool IsSelected { get; set; }
    }
}