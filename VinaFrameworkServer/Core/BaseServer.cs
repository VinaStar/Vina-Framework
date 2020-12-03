using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace VinaFrameworkServer.Core
{
    /// <summary>
    /// Extend your Server script with this class and add modules to your constructor
    /// </summary>
    public abstract class BaseServer : BaseScript
    {
        /// <summary>
        /// This current client resource name
        /// </summary>
        public static string Name { get; private set; }

        /// <summary>
        /// This current client resource name
        /// </summary>
        public string ResourceName { get { return Name; } }

        private List<Module> modules;

        /// <summary>
        /// Extend your Server script with this class and add modules to your constructor
        /// </summary>
        public BaseServer()
        {
            Name = API.GetCurrentResourceName();
            modules = new List<Module>();
            EventHandlers["playerConnecting"] += new Action<Player>(onPlayerConnecting);
            EventHandlers["playerDropped"] += new Action<Player, string>(onPlayerDropped);
            EventHandlers[$"internal:{Name}:onPlayerClientInitialized"] += new Action<Player>(onPlayerClientInitialized);
            
            Debug.WriteLine("============================================================");
            Log($"Instanciating...");
        }

        #region Base Events

        private void onPlayerConnecting([FromSource] Player player)
        {
            Log($"PlayerConnecting {player.Name}");

            foreach (Module module in modules)
            {
                try
                {
                    module.onPlayerConnecting(player);
                }
                catch (Exception exception)
                {
                    LogError(exception, $" in {module.GetType().Name} > OnPlayerConnecting");
                }
            }
        }
        private void onPlayerDropped([FromSource] Player player, string reason)
        {
            Log($"PlayerDropped {player.Name}");

            foreach (Module module in modules)
            {
                try
                {
                    module.onPlayerDropped(player, reason);
                }
                catch (Exception exception)
                {
                    LogError(exception, $" in {module.GetType().Name} > OnPlayerDropped");
                }
            }
        }
        private void onPlayerClientInitialized([FromSource] Player player)
        {
            Log($"PlayerClientInitialized {player.Name}");

            foreach (Module module in modules)
            {
                try
                {
                    module.onPlayerClientInitialized(player);
                }
                catch (Exception exception)
                {
                    LogError(exception, $" in {module.GetType().Name} > OnPlayerClientInitialized");
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Verify if the module has been loaded.
        /// </summary>
        /// <param name="moduleType">The module class type. Ex: typeof(MyModule)</param>
        /// <returns></returns>
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
        /// Add module by referencing the type of your module class.
        /// </summary>
        /// <param name="moduleType">The module class type. Ex: typeof(MyModule)</param>
        public void AddModule(Type moduleType)
        {
            bool error = false;
            foreach(Module _module in modules)
            {
                if (_module.GetType() == moduleType)
                {
                    error = true;
                    LogError(new Exception($"Trying to add existing module {moduleType.Name}!"));
                }
            }

            if (error) return;

            try
            {
                if (!moduleType.IsSubclassOf(typeof(Module)))
                    throw new Exception($"Trying to add class {moduleType.Name} that is not a subclass of Module!");

                Module module = (Module)Activator.CreateInstance(moduleType, this);
                modules.Add(module);
            }
            catch (Exception exception)
            {
                LogError(exception, $" in {moduleType.Name} > Constructor");
            }
        }

        /// <summary>
        /// Get a module instance from anywhere, cannot be called inside a module constructor.
        /// </summary>
        /// <typeparam name="T">The module class type.</typeparam>
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
        /// Register an event.
        /// </summary>
        /// <param name="eventName">The event name to register.</param>
        /// <param name="action">The event delegate to register.</param>
        public void RegisterEvent(string eventName, Delegate action)
        {
            EventHandlers[eventName] += action;
            Log($"Event {eventName} registered!");
        }

        /// <summary>
        /// Un-register an event.
        /// </summary>
        /// <param name="eventName">The event name to un-register.</param>
        /// <param name="action">The event delegate to un-register.</param>
        public void UnregisterEvent(string eventName, Delegate action)
        {
            EventHandlers[eventName] -= action;
            Log($"Event {eventName} deregistered!");
        }

        /// <summary>
        /// Add a tick to the resource.
        /// </summary>
        /// <param name="action">The tick delegate to add.</param>
        public void AddTick(Func<Task> action)
        {
            Tick += action;
            Log($"{action.Method.DeclaringType.Name} added Tick {action.Method.Name}!");
        }

        /// <summary>
        /// Remove a tick from the resource.
        /// </summary>
        /// <param name="action">The tick delegate to remove.</param>
        public void RemoveTick(Func<Task> action)
        {
            Tick -= action;
            Log($"{action.Method.DeclaringType.Name} removed Tick {action.Method.Name}!");
        }

        /// <summary>
        /// Un-register an event.
        /// </summary>
        /// <param name="eventName">The event name to un-register.</param>
        /// <param name="action">The event delegate to un-register.</param>
        internal void AddInternalTick(Func<Task> action)
        {
            Tick += action;
        }

        /// <summary>
        /// Remove a tick from the resource.
        /// </summary>
        /// <param name="action">The tick delegate to remove.</param>
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
        /// Get a specific Export from a resource.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns>Return the dynamic export.</returns>
        public dynamic GetExport(string resourceName)
        {
            return Exports[resourceName];
        }

        /// <summary>
        /// Set an Export from this resource.
        /// </summary>
        /// <param name="name">The name of the Exported method.</param>
        /// <param name="method">The delegate of the Exported method.</param>
        public void SetExport(string name, Delegate method)
        {
            Exports.Add(name, method);
        }

        /// <summary>
        /// Log a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Log(string message)
        {
            Debug.WriteLine($"{DateTime.Now.ToLongTimeString()} [INFO] {ResourceName.ToUpper()}: {message}");
        }

        /// <summary>
        /// Log an exception.
        /// </summary>
        /// <param name="exception">The Exception to log.</param>
        /// <param name="prefix">Some text to add before the log message.</param>
        public void LogError(Exception exception, string prefix = "")
        {
            string pre = (prefix != "") ? $" {prefix}" : "";
            Debug.WriteLine($"{DateTime.Now.ToLongTimeString()} [ERROR] {ResourceName.ToUpper()}{pre}: {exception.Message}\n{exception.StackTrace}");
        }

        #endregion
    }
}
