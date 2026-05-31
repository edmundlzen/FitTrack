namespace FitTrack.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        try
        {
            return new Window(new MainPage()) { Title = "FitTrack" };
        }
        catch (Exception ex)
        {
            WriteErrorLog("CreateWindow", ex);
            throw;
        }
    }

    internal static void WriteErrorLog(string context, Exception ex)
    {
        try
        {
            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "fittrack_error.txt");
            File.AppendAllText(path,
                $"[{DateTime.UtcNow:O}] {context}\n{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}\n\n");
        }
        catch { }
    }
}
