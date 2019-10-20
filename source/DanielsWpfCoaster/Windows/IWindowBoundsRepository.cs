namespace DanielsWpfCoaster.Windows
{
    public interface IWindowBoundsRepository
    {
        WindowPlacement LoadBoundsOrDefault();
        void StoreBounds(WindowPlacement wp);
    }
}