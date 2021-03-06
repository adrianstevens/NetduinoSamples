using System.Threading;

namespace XMasPlayer
{
    public class Program
    {
        public static void Main ()
        {
            var app = new App();
            app.Run();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}

namespace System.Diagnostics
{
    public enum DebuggerBrowsableState
    {
        Never = 0,
        Collapsed = 2,
        RootHidden = 3
    }
}