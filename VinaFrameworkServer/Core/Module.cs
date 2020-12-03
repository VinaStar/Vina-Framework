using System;
using System.Threading.Tasks;

using CitizenFX.Core;

namespace VinaFrameworkServer.Core
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Module
    {
        /// <summary>
        /// This current module name.
        /// </summary>
        protected string Name { get; }

        /// <summary>
        /// Read-only reference to the server instance.
        /// </summary>
        protected BaseServer server { get; }

        /// <summary>
        /// Extend your module class with this class and add it to your server constructor.
        /// </summary>
        /// <param name="server"></param>
        public Module(BaseServer server)
        {
            Name = this.GetType().Name;
            this.server = server;
            server.AddInternalTick(initialize);
            Log($"Instance created!");
        }

        private async Task initialize()
        {
            Log($"Initializing...");

            server.RemoveInternalTick(initialize);
            
            await BaseServer.Delay(0);

            try
            {
                OnModuleInitialized();
            }
            catch (Exception exception)
            {
                LogError(exception, " in OnModuleInitialized");
            }
        }

        /// <summary>
        /// Overridable method that run on first tick only. You can get other module from here.
        /// </summary>
        protected virtual void OnModuleInitialized()
        {
            Log($"Initialized!");
        }

        internal void onPlayerConnecting(Player player)
        {
            OnPlayerConnecting(player);
        }

        /// <summary>
        /// Overridable method that run when a player is connecting to the server.
        /// </summary>
        /// <param name="player">The player that is connecting.</param>
        protected abstract void OnPlayerConnecting(Player player);

        internal void onPlayerDropped(Player player, string reason)
        {
            OnPlayerDropped(player, reason);
        }

        /// <summary>
        /// Overridable method that run when a player has left the server.
        /// </summary>
        /// <param name="player">The player that left.</param>
        /// <param name="reason">The reason that player left.</param>
        protected abstract void OnPlayerDropped(Player player, string reason);

        internal void onPlayerClientInitialized(Player player)
        {
            OnPlayerClientInitialized(player);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        protected abstract void OnPlayerClientInitialized(Player player);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msecs"></param>
        public static Task Delay(int msecs)
        {
            return BaseServer.Delay(msecs);
        }

        /// <summary>
        /// Log a message from this module.
        /// </summary>
        /// <param name="message">The message to log.</param>
        protected void Log(string message)
        {
            Debug.WriteLine($"{DateTime.Now.ToLongTimeString()} [INFO] {server.ResourceName.ToUpper()} > {Name.ToUpper()}: {message}");
        }

        /// <summary>
        /// Log an exception from this module.
        /// </summary>
        /// <param name="exception">The Exception to log.</param>
        /// <param name="prefix">Some text to add before the log message.</param>
        protected void LogError(Exception exception, string prefix = "")
        {
            string pre = (prefix != "") ? $" {prefix}" : "";
            Debug.WriteLine($"{DateTime.Now.ToLongTimeString()} [ERROR] {server.ResourceName.ToUpper()} > {Name.ToUpper()}{pre}: {exception.Message}\n{exception.StackTrace}");
        }
    }
}
