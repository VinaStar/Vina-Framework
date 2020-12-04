using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace VinaFrameworkClient.Core
{
    /// <summary>
    /// Extend your Client class with BaseClient class and add your modules into the constructor.
    /// </summary>
    public abstract class BaseClient : BaseScript
    {
        /// <summary>
        /// Extend your Client class with BaseClient class and add your modules into the constructor.
        /// </summary>
        public BaseClient()
        {
            ResourceName = API.GetCurrentResourceName();
            modules = new List<Module>();

            Debug.WriteLine("============================================================");
            Log($"Initializing...");

            Tick += initialize;
        }

        #region VARIABLES

        /// <summary>
        /// This current client resource name
        /// </summary>
        public static string ResourceName { get; private set; }

        /// <summary>
        /// This current client resource name
        /// </summary>
        public string Name { get { return ResourceName; } }

        private List<Module> modules;

        #endregion
        #region BASE EVENTS

        private async Task initialize()
        {
            Tick -= initialize;

            await Delay(0);

            Log($"Initialized!");
            TriggerServerEvent($"internal:{Name}:onPlayerClientInitialized");
        }

        #endregion
        #region CLIENT METHODS

        /// <summary>
        /// Check if a Module has been added to the Client instance.
        /// </summary>
        /// <param name="moduleType">Your module type. Ex: typeof(MyModule)</param>
        /// <returns>True if it was found.</returns>
        public bool HasModule(Type moduleType)
        {
            foreach (Module module in modules)
            {
                if (module.GetType() == moduleType)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Add a module by passing your Module type.
        /// </summary>
        /// <param name="moduleType">Your module type. Ex: typeof(MyModule)</param>
        protected void AddModule(Type moduleType)
        {
            bool error = false;
            foreach (Module _module in modules)
            {
                if (_module.GetType() == moduleType)
                {
                    error = true;
                    LogError(new Exception($"Module {moduleType.Name} was already added!"));
                }
            }

            if (error) return;

            try
            {
                if (!moduleType.IsSubclassOf(typeof(Module)))
                    throw new Exception($"Trying to add class {moduleType.Name} that is not extending Module class!");

                Module module = (Module)Activator.CreateInstance(moduleType, this);
                modules.Add(module);
            }
            catch (Exception exception)
            {
                LogError(exception, $" > {moduleType.Name} in Constructor");
            }
        }

        /// <summary>
        /// Get a module instance. Cannot be called inside a Module constructor.
        /// </summary>
        /// <typeparam name="T">Your module type. Ex: &lt;MyModule&gt;</typeparam>
        /// <returns>Return the module or throw an exception if the module is not found.</returns>
        public T GetModule<T>()
        {
            foreach (Module module in modules)
            {
                if (module.GetType() == typeof(T))
                {
                    return (T)Convert.ChangeType(module, typeof(T));
                }
            }

            throw new Exception($"Module {typeof(T)} doesn't exist!");
        }

        /// <summary>
        /// Get the player list.
        /// </summary>
        /// <returns>Return a PlayerList object.</returns>
        public PlayerList GetPlayers()
        {
            return Players;
        }

        /// <summary>
        /// Add an event.
        /// </summary>
        /// <param name="eventName">Event name to add.</param>
        /// <param name="action">Event delegate to add.</param>
        protected void AddEvent(string eventName, Delegate action)
        {
            EventHandlers[eventName] += action;
            Log($"Event {eventName} added!");
        }

        /// <summary>
        /// Remove an event.
        /// </summary>
        /// <param name="eventName">Event name to remove.</param>
        /// <param name="action">Event delegate to remove.</param>
        protected void RemoveEvent(string eventName, Delegate action)
        {
            EventHandlers[eventName] -= action;
            Log($"Event {eventName} removed!");
        }

        /// <summary>
        /// Add a tick.
        /// </summary>
        /// <param name="action">Tick delegate to add.</param>
        protected void AddTick(Func<Task> action)
        {
            Tick += action;
            Log($"Added Tick {action.Method.Name}!");
        }

        /// <summary>
        /// Remove a tick.
        /// </summary>
        /// <param name="action">Tick delegate to remove.</param>
        protected void RemoveTick(Func<Task> action)
        {
            Tick -= action;
            Log($"Removed Tick {action.Method.Name}!");
        }

        /// <summary>
        /// Get the ExportDictionary.
        /// </summary>
        /// <returns>Return the ExportDictionary.</returns>
        protected ExportDictionary GetExports()
        {
            return Exports;
        }

        /// <summary>
        /// Get a resource Export.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns>Return a dynamic export.</returns>
        protected dynamic GetExport(string resourceName)
        {
            return Exports[resourceName];
        }

        /// <summary>
        /// Export a delegate method.
        /// </summary>
        /// <param name="name">Name of the Export.</param>
        /// <param name="method">Delegate of the Export.</param>
        protected void SetExport(string name, Delegate method)
        {
            Exports.Add(name, method);
        }

        /// <summary>
        /// Log a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        protected void Log(string message)
        {
            Debug.WriteLine($"{DateTime.Now.ToLongTimeString()} [INFO] {BaseClient.ResourceName.ToUpper()}: {message}");
        }

        /// <summary>
        /// Log an exception.
        /// </summary>
        /// <param name="exception">The Exception to log.</param>
        /// <param name="prefix">Some text to add before the log message.</param>
        protected void LogError(Exception exception, string prefix = "")
        {
            string pre = (prefix != "") ? prefix : "";
            Debug.WriteLine($"{DateTime.Now.ToLongTimeString()} [ERROR] {BaseClient.ResourceName.ToUpper()}{pre}: {exception.Message}\n{exception.StackTrace}");
        }

        #endregion
    }
}
