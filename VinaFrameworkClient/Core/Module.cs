using System;
using System.Threading.Tasks;

using CitizenFX.Core;

namespace VinaFrameworkClient.Core
{
    /// <summary>
    /// Extend your module class with this class and add it to your client constructor.
    /// </summary>
    public abstract class Module
    {
        /// <summary>
        /// This current module name.
        /// </summary>
        protected string Name { get; }

        /// <summary>
        /// Read-only reference to the client instance.
        /// </summary>
        protected BaseClient client { get; }

        /// <summary>
        /// Extend your module class with this class and add it to your client constructor.
        /// </summary>
        /// <param name="client"></param>
        public Module(BaseClient client)
        {
            Name = this.GetType().Name;
            this.client = client;
            client.AddInternalTick(initialize);
            Log($"Instance created!");
        }

        private async Task initialize()
        {
            Log($"Initializing...");

            client.RemoveInternalTick(initialize);

            await BaseClient.Delay(0);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msecs"></param>
        public static Task Delay(int msecs)
        {
            return BaseClient.Delay(msecs);
        }

        /// <summary>
        /// Log a message from this module.
        /// </summary>
        /// <param name="message">The message to log.</param>
        protected void Log(string message)
        {
            Debug.WriteLine($"{DateTime.Now.ToLongTimeString()} [INFO] {client.ResourceName.ToUpper()} > {Name.ToUpper()}: {message}");
        }

        /// <summary>
        /// Log an exception from this module.
        /// </summary>
        /// <param name="exception">The Exception to log.</param>
        /// <param name="prefix">Some text to add before the log message.</param>
        protected void LogError(Exception exception, string prefix = "")
        {
            string pre = (prefix != "") ? $" {prefix}" : "";
            Debug.WriteLine($"{DateTime.Now.ToLongTimeString()} [ERROR] {client.ResourceName.ToUpper()} > {Name.ToUpper()}{pre}: {exception.Message}\n{exception.StackTrace}");
        }
    }
}
