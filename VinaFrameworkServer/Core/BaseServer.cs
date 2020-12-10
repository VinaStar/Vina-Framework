using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CitizenFX.Core;
using CitizenFX.Core.Native;

using Newtonsoft.Json;

using VinaFrameworkClient.Shared;

namespace VinaFrameworkServer.Core
{
    /// <summary>
    /// Extend your Server class with BaseServer class and add your modules into the constructor.
    /// </summary>
    public abstract class BaseServer : BaseScript
    {
        /// <summary>
        /// Extend your Server class with BaseServer class and add your modules into the constructor.
        /// </summary>
        public BaseServer()
        {
            ResourceName = API.GetCurrentResourceName();

            Debug.WriteLine("============================================================");
            Log($"Instanciating...");

            modules = new List<Module>();

            EventHandlers["onResourceStarting"] += new Action<string>(onResourceStarting);
            EventHandlers["onResourceStart"] += new Action<string>(onResourceStart);
            EventHandlers["onResourceStop"] += new Action<string>(onResourceStop);
            EventHandlers["playerConnecting"] += new Action<Player, string, dynamic, dynamic>(onPlayerConnecting);
            EventHandlers["playerJoining"] += new Action<Player>(onPlayerJoining);
            EventHandlers["playerDropped"] += new Action<Player, string>(onPlayerDropped);
            EventHandlers["playerEnteredScope"] += new Action<dynamic>(onPlayerEnteredScope);
            EventHandlers["playerLeftScope"] += new Action<dynamic>(onPlayerLeftScope);
            EventHandlers["entityCreating"] += new Action<int>(onEntityCreating);
            EventHandlers["entityCreated"] += new Action<int>(onEntityCreated);
            EventHandlers["entityRemoved"] += new Action<int>(onEntityRemoved);
            EventHandlers[$"internal:{Name}:onPlayerClientInitialized"] += new Action<Player>(onPlayerClientInitialized);

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
        /// Cleanup the garbage once per minute to keep memory usage low.
        /// </summary>
        protected bool UseGarbageCollector { get; set; } = true;

        #endregion
        #region BASE EVENTS

        private async void onResourceStarting(string resourceName)
        {
            foreach (Module module in modules)
            {
                module.onResourceStarting(resourceName);
            }

            await Delay(0);
        }

        private async void onResourceStart(string resourceName)
        {
            foreach (Module module in modules)
            {
                module.onResourceStart(resourceName);
            }

            await Delay(0);
        }

        private async void onResourceStop(string resourceName)
        {
            foreach (Module module in modules)
            {
                module.onResourceStop(resourceName);
            }

            await Delay(0);
        }

        private async void onPlayerConnecting([FromSource] Player player, string playerName, dynamic setKickReason, dynamic deferrals)
        {
            foreach (Module module in modules)
            {
                module.onPlayerConnecting(player, deferrals);
            }

            await Delay(0);
        }

        private async void onPlayerJoining([FromSource] Player player)
        {
            foreach (Module module in modules)
            {
                module.onPlayerJoining(player);
            }

            await Delay(0);
        }

        private async void onPlayerDropped([FromSource] Player player, string reason)
        {
            foreach (Module module in modules)
            {
                module.onPlayerDropped(player, reason);
            }

            await Delay(0);
        }

        private async void onPlayerClientInitialized([FromSource] Player player)
        {
            foreach (Module module in modules)
            {
                module.onPlayerClientInitialized(player);
            }

            await Delay(0);
        }

        private async void onPlayerEnteredScope(dynamic data)
        {
            foreach (Module module in modules)
            {
                module.onPlayerEnteredScope(data["for"], data["player"]);
            }

            await Delay(0);
        }

        private async void onPlayerLeftScope(dynamic data)
        {
            foreach (Module module in modules)
            {
                module.onPlayerLeftScope(data["for"], data["player"]);
            }

            await Delay(0);
        }

        private async void onEntityCreating(int entityHandle)
        {
            foreach (Module module in modules)
            {
                module.onEntityCreating(entityHandle);
            }

            await Delay(0);
        }

        private async void onEntityCreated(int entityHandle)
        {
            foreach (Module module in modules)
            {
                module.onEntityCreated(entityHandle);
            }

            await Delay(0);
        }

        private async void onEntityRemoved(int entityHandle)
        {
            foreach (Module module in modules)
            {
                module.onEntityRemoved(entityHandle);
            }

            await Delay(0);
        }

        #endregion
        #region SERVER TICKS

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

        #endregion
        #region SERVER METHODS

        /// <summary>
        /// Check if a Module has been added to the Server instance.
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
            foreach(Module _module in modules)
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
                LogError(exception, $" in {moduleType.Name} > Constructor");
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
        /// Get a player by name
        /// </summary>
        /// <param name="name">The player name to get.</param>
        /// <returns>Return a Player object or null</returns>
        public Player GetPlayerByName(string name)
        {
            foreach (Player player in Players)
            {
                if (player.Name == name)
                {
                    return player;
                }
            }
            return null;
        }

        /// <summary>
        /// Get a player by it's handle
        /// </summary>
        /// <param name="handle">The player handle to get.</param>
        /// <returns>Return a Player object or null</returns>
        public Player GetPlayerByHandle(string handle)
        {
            foreach (Player player in Players)
            {
                if (player.Handle == handle)
                {
                    return player;
                }
            }
            return null;
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
        /// Send a nui message to a player client.
        /// </summary>
        /// <param name="player">The player to send to.</param>
        /// <param name="nuiRequest">The nui request to send.</param>
        public static void SendNuiActionDataTo(Player player, NuiRequest nuiRequest)
        {
            TriggerClientEvent(player, $"internal:{ResourceName}:onServerNuiRequest", nuiRequest);
        }

        /// <summary>
        /// Send a nui message to all player clients.
        /// </summary>
        /// <param name="nuiRequest">The nui request to send to all players.</param>
        public static void SendNuiActionDataToAll(NuiRequest nuiRequest)
        {
            TriggerClientEvent($"internal:{ResourceName}:onServerNuiRequest", nuiRequest);
        }

        /// <summary>
        /// Serialize an object into a json string.
        /// </summary>
        /// <param name="obj">The object to convert into a string.</param>
        /// <param name="indented">The json format, indented or none by default.</param>
        /// <returns>The converted json string.</returns>
        public static string SerializeObject(dynamic obj, bool indented = false)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, (indented) ? Formatting.Indented : Formatting.None);
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
            Debug.WriteLine($"{DateTime.Now.ToLongTimeString()} [INFO] {BaseServer.ResourceName.ToUpper()}: {message}");
        }

        /// <summary>
        /// Log an exception.
        /// </summary>
        /// <param name="exception">The Exception to log.</param>
        /// <param name="prefix">Some text to add before the log message.</param>
        protected static void LogError(Exception exception, string prefix = "")
        {
            string pre = (prefix != "") ? prefix : "";
            Debug.WriteLine($"{DateTime.Now.ToLongTimeString()} [ERROR] {BaseServer.ResourceName.ToUpper()}{pre}: {exception.Message}\n{exception.StackTrace}");
        }

        #endregion
    }
}
