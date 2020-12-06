using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CitizenFX.Core;
using CitizenFX.Core.Native;

using Newtonsoft.Json;

using VinaFrameworkClient.Shared;

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

            Debug.WriteLine("============================================================");
            Log($"Initializing...");

            modules = new List<Module>();
            deadPlayers = new List<Player>();

            EventHandlers[$"internal:{Name}:onServerNuiRequest"] += new Action<NuiRequest>(onServerNuiRequest);

            Tick += initialize;
            Tick += garbageCollect;
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

        /// <summary>
        /// **Experimental**
        /// Cleanup the garbage once a minutes to keep memory usage low.
        /// </summary>
        protected bool UseGarbageCollector { get; set; } = false;

        /// <summary>
        /// **Experimental**
        /// Enabled the process that watches for players who die and also resurect.
        /// Enable if you need to use OnPlayerDied and OnPlayerResurect base events in your modules.
        /// </summary>
        protected bool UsePlayerDeadResurectWatcher
        {
            get
            {
                return _usePlayerDeadResurectWatcher;
            }
            set
            {
                if (value == false)
                {
                    deadPlayers.Clear();
                }
                _usePlayerDeadResurectWatcher = value;
            }
        }
        private bool _usePlayerDeadResurectWatcher = false;
        private List<Player> deadPlayers;

        #endregion
        #region BASE EVENTS

        private void onServerNuiRequest(NuiRequest request)
        {
            SendNuiActionData(request);
        }

        #endregion
        #region SERVER TICKS

        private async Task initialize()
        {
            Tick -= initialize;
            Tick += playerDeadResurectWatcher;

            await Delay(0);

            Log($"Initialized!");
            TriggerServerEvent($"internal:{Name}:onPlayerClientInitialized");
        }

        private async Task garbageCollect()
        {
            if (!UseGarbageCollector)
            {
                await Delay(1);
                return;
            }

            await Delay(60000);

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private async Task playerDeadResurectWatcher()
        {
            await Delay(250); // run 4 times per second, experimental

            if (!UsePlayerDeadResurectWatcher) return;

            foreach (Player player in Players)
            {
                if (!deadPlayers.Contains(player) && player.IsDead)
                {
                    deadPlayers.Add(player);
                    foreach (Module module in modules)
                    {
                        module.onPlayerDied(player);
                    }
                }
                else if (deadPlayers.Contains(player) && !player.IsDead)
                {
                    deadPlayers.Remove(player);
                    foreach (Module module in modules)
                    {
                        module.onPlayerResurect(player);
                    }
                }
            }
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
        /// Send a message to this resource Nui.
        /// </summary>
        /// <param name="nuiRequest">The nui request object.</param>
        public static void SendNuiActionData(NuiRequest nuiRequest)
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
        public static void SendNuiActionData(string action, dynamic data = null)
        {
            NuiRequest request = new NuiRequest(action, data);
            SendNuiActionData(request);
        }

        /// <summary>
        /// Serialize an object into a json string.
        /// </summary>
        /// <param name="obj">The object to convert into a string.</param>
        /// <param name="formatting">The json format, indented or none by default.</param>
        /// <returns>The converted json string.</returns>
        public static string SerializeObject(dynamic obj, Formatting formatting = Formatting.None)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, formatting);
            }
            catch (Exception exception)
            {
                LogError(exception, " in SerializeObject");
            }

            return "";
        }

        /// <summary>
        /// Deserialize a json into a dynamic object.
        /// </summary>
        /// <param name="json">The json string to convert into an object.</param>
        /// <returns>The converted object.</returns>
        public static dynamic DeserializeObject(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject(json);
            }
            catch (Exception exception)
            {
                LogError(exception, " in DeserializeObject");
            }

            return null;
        }

        /// <summary>
        /// Deserialize an json into an object of a specific Type.
        /// </summary>
        /// <typeparam name="T">The type to convert the json to.</typeparam>
        /// <param name="json">The json string to convert into an object.</param>
        /// <returns>The converted object.</returns>
        public static T DeserializeObject<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception exception)
            {
                LogError(exception, $" in DeserializeObject with generic type {typeof(T).Name}");
            }

            return default(T);
        }

        /// <summary>
        /// Log a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        protected static void Log(string message)
        {
            Debug.WriteLine($"{DateTime.Now.ToLongTimeString()} [INFO] {BaseClient.ResourceName.ToUpper()}: {message}");
        }

        /// <summary>
        /// Log an exception.
        /// </summary>
        /// <param name="exception">The Exception to log.</param>
        /// <param name="prefix">Some text to add before the log message.</param>
        protected static void LogError(Exception exception, string prefix = "")
        {
            string pre = (prefix != "") ? prefix : "";
            Debug.WriteLine($"{DateTime.Now.ToLongTimeString()} [ERROR] {BaseClient.ResourceName.ToUpper()}{pre}: {exception.Message}\n{exception.StackTrace}");
        }

        #endregion
    }
}
