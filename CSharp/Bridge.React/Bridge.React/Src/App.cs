
using Bridge.React.Logotron.API;

namespace Bridge.React.Logotron
{
    public sealed class App
    {
        public static void Main()
        {

            LogotronApp.ChargerLogotron();

            var container = Bridge.Html5.Document.GetElementById("main");
            container.ClassList.Remove("chargement");
            var logotron = new LogotronApp(
                new LogotronApp.Props { MessageApi = new MessageApi() } 
            );

            React.Render(logotron, container);
        }
    }
}