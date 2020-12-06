using System;
using System.Threading.Tasks;

using Newtonsoft.Json;

using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace VinaFrameworkClient.Core
{
    /// <summary>
    /// ModuleScript class contains the script related methods for a Module.
    /// </summary>
    public sealed class ModuleScript : BaseScript
    {
        /// <summary>
        /// ModuleScript class contains the script related methods for a Module.
        /// </summary>
        public ModuleScript(Module module) : base()
        {
            this.module = module;
        }

        #region VARIABLES

        private Module module;

        #endregion
        #region METHODS

        /// <summary>
        /// Add an event.
        /// </summary>
        /// <param name="eventName">Event name to add.</param>
        /// <param name="action">Event delegate to add.</param>
        public void AddEvent(string eventName, Delegate action)
        {
            EventHandlers[eventName] += action;
            Log($"Event {eventName} added!");
        }

        /// <summary>
        /// Remove an event.
        /// </summary>
        /// <param name="eventName">Event name to remove.</param>
        /// <param name="action">Event delegate to remove.</param>
        public void RemoveEvent(string eventName, Delegate action)
        {
            EventHandlers[eventName] -= action;
            Log($"Event {eventName} removed!");
        }

        /// <summary>
        /// Add a tick.
        /// </summary>
        /// <param name="action">Tick delegate to add.</param>
        public void AddTick(Func<Task> action)
        {
            Tick += action;
            Log($"Added Tick {action.Method.Name}!");
        }

        /// <summary>
        /// Remove a tick.
        /// </summary>
        /// <param name="action">Tick delegate to remove.</param>
        public void RemoveTick(Func<Task> action)
        {
            Tick -= action;
            Log($"Removed Tick {action.Method.Name}!");
        }

        internal void AddInternalTick(Func<Task> action)
        {
            Tick += action;
        }
        internal void RemoveInternalTick(Func<Task> action)
        {
            Tick -= action;
        }

        /// <summary>
        /// Get the ExportDictionary.
        /// </summary>
        /// <returns>Return the ExportDictionary.</returns>
        public ExportDictionary GetExports()
        {
            return Exports;
        }

        /// <summary>
        /// Get a resource Export.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns>Return a dynamic export.</returns>
        public dynamic GetExport(string resourceName)
        {
            return Exports[resourceName];
        }

        /// <summary>
        /// Export a delegate method.
        /// </summary>
        /// <param name="name">Name of the Export.</param>
        /// <param name="method">Delegate of the Export.</param>
        public void SetExport(string name, Delegate method)
        {
            Exports.Add(name, method);
            Log($"Export {name} set!");
        }

        /// <summary>
        /// Send a message to this resource Nui.
        /// </summary>
        /// <param name="nuiRequest">The nui request object.</param>
        public void SendNuiActionData(NuiRequest nuiRequest)
        {
            try
            {
                string serializedQuery = JsonConvert.SerializeObject(nuiRequest, Formatting.Indented);

                API.SendNuiMessage(serializedQuery);
            }
            catch (Exception exception)
            {
                LogError(exception, " in SendNuiActionData");
            }
        }

        /// <summary>
        /// Send a message to this resource Nui.
        /// </summary>
        /// <param name="action">The action name.</param>
        /// <param name="data">Some data will be sent as a string.</param>
        public void SendNuiActionData(string action, dynamic data = null)
        {
            NuiRequest request = new NuiRequest(action, data);
            SendNuiActionData(request);
        }

        /// <summary>
        /// Log a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Log(string message)
        {
            Debug.WriteLine($"{DateTime.Now.ToLongTimeString()} [INFO] {BaseClient.ResourceName.ToUpper()} > {module.Name}: {message}");
        }

        /// <summary>
        /// Log an exception.
        /// </summary>
        /// <param name="exception">The Exception to log.</param>
        /// <param name="prefix">Some text to add before the log message.</param>
        public void LogError(Exception exception, string prefix = "")
        {
            string pre = (prefix != "") ? prefix : "";
            Debug.WriteLine($"{DateTime.Now.ToLongTimeString()} [ERROR] {BaseClient.ResourceName.ToUpper()} > {module.Name}{pre}: {exception.Message}\n{exception.StackTrace}");
        }

        #endregion
    }
}
