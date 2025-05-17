namespace pis2
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var view = new Views.MainFormView();
            var controller = new Controllers.MainFormController(view, "Host=localhost;Username=postgres;Password=admin;Database=pis");

            Application.Run(view);
        }
    }
}