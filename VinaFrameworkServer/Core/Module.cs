using System;
using System.Threading.Tasks;

using CitizenFX.Core;

namespace VinaFrameworkServer.Core
{
    /// <summary>
    /// Extend all your modules with the Module class and add them to your Server class constructor.
    /// </summary>
    public abstract class Module
    {
        /// <summary>
        /// Extend all your modules with the Module class and add them to your Server class constructor.
        /// </summary>
        /// <param name="server">A BaseServer class</param>
        public Module(BaseServer server)
        {
            Name = this.GetType().Name;
            this.server = server;
            BaseServer.RegisterScript(script = new ModuleScript(this));
            script.AddInternalTick(initialize);
            script.Log($"Instance created!");
        }

        #region VARIABLES

        /// <summary>
        /// Current module name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Read-only reference to the server instance.
        /// </summary>
        protected BaseServer server { get; }

        /// <summary>
        /// Read-only reference to the module script instance.
        /// </summary>
        protected ModuleScript script { get; }

        #endregion
        #region BASE EVENTS

        /// <summary>
        /// Overridable method that run on first tick only. You can get other module from here.
        /// </summary>
        protected virtual async void OnModuleInitialized() { await BaseServer.Delay(0); }
        private async Task initialize()
        {
            script.Log($"Initializing...");

            script.RemoveInternalTick(initialize);

            try
            {
                OnModuleInitialized();
                script.Log($"Initialized!");
            }
            catch (Exception exception)
            {
                script.LogError(exception, " in OnModuleInitialized");
            }

            await BaseServer.Delay(0);
        }

        /// <summary>
        /// Overridable method that run when a resource is starting.
        /// </summary>
        /// <param name="resourceName">The resource name that is starting.</param>
        protected virtual async void OnResourceStarting(string resourceName) { await BaseServer.Delay(0); }
        internal async void onResourceStarting(string resourceName)
        {
            try
            {
                OnResourceStarting(resourceName);
            }
            catch (Exception exception)
            {
                script.LogError(exception, " in OnResourceStarting");
            }

            await BaseServer.Delay(0);
        }

        /// <summary>
        /// Overridable method that run when a resource has started.
        /// </summary>
        /// <param name="resourceName">The resource name that started.</param>
        protected virtual async void OnResourceStart(string resourceName) { await BaseServer.Delay(0); }
        internal async void onResourceStart(string resourceName)
        {
            try
            {
                OnResourceStart(resourceName);
            }
            catch (Exception exception)
            {
                script.LogError(exception, " in OnResourceStart");
            }

            await BaseServer.Delay(0);
        }

        /// <summary>
        /// Overridable method that run when a resource has stopped.
        /// </summary>
        /// <param name="resourceName">The resource name that stopped.</param>
        protected virtual async void OnResourceStop(string resourceName) { await BaseServer.Delay(0); }
        internal async void onResourceStop(string resourceName)
        {
            try
            {
                OnResourceStop(resourceName);
            }
            catch (Exception exception)
            {
                script.LogError(exception, " in OnResourceStop");
            }

            await BaseServer.Delay(0);
        }

        /// <summary>
        /// Overridable method that run when a player is connecting to the server.
        /// </summary>
        /// <param name="player">The player that is connecting.</param>
        /// <param name="deferrals">deferrals: { defer: any; done: any; presentCard: any; update: any }</param>
        protected virtual async void OnPlayerConnecting(Player player, dynamic deferrals) { await BaseServer.Delay(0); }
        internal async void onPlayerConnecting(Player player, dynamic deferrals)
        {
            try
            {
                OnPlayerConnecting(player, deferrals);
            }
            catch (Exception exception)
            {
                script.LogError(exception, " in OnPlayerConnecting");
            }

            await BaseServer.Delay(0);
        }

        /// <summary>
        /// Overridable method that run when after a player has connected to the server.
        /// </summary>
        /// <param name="player">The player that is joining.</param>
        protected virtual async void OnPlayerJoining(Player player) { await BaseServer.Delay(0); }
        internal async void onPlayerJoining(Player player)
        {
            try
            {
                OnPlayerJoining(player);
            }
            catch (Exception exception)
            {
                script.LogError(exception, " in OnPlayerJoining");
            }

            await BaseServer.Delay(0);
        }

        /// <summary>
        /// Overridable method that run when a player has left the server.
        /// </summary>
        /// <param name="player">The player that left.</param>
        /// <param name="reason">The reason that player left.</param>
        protected virtual async void OnPlayerDropped(Player player, string reason) { await BaseServer.Delay(0); }
        internal async void onPlayerDropped(Player player, string reason)
        {
            try
            {
                OnPlayerDropped(player, reason);
            }
            catch (Exception exception)
            {
                script.LogError(exception, " in OnPlayerDropped");
            }

            await BaseServer.Delay(0);
        }

        /// <summary>
        /// Overridable method that run when the client has initialized. (After loading)
        /// </summary>
        /// <param name="player">The player client that has initialized.</param>
        protected virtual async void OnPlayerClientInitialized(Player player) { await BaseServer.Delay(0); }
        internal async void onPlayerClientInitialized(Player player)
        {
            try
            {
                OnPlayerClientInitialized(player);
            }
            catch (Exception exception)
            {
                script.LogError(exception, " in OnPlayerClientInitialized");
            }

            await BaseServer.Delay(0);
        }

        /// <summary>
        /// Overridable method that is triggered when a player enters another player's scope.
        /// </summary>
        /// <param name="playerHandle">The player handle for which the scope is being entered.</param>
        /// <param name="playerEnteringHandle">The player handle that is entering the scope.</param>
        protected virtual async void OnPlayerEnteredScope(string playerHandle, string playerEnteringHandle) { await BaseServer.Delay(0); }
        internal async void onPlayerEnteredScope(string playerHandle, string playerEnteringHandle)
        {
            try
            {
                OnPlayerEnteredScope(playerHandle, playerEnteringHandle);
            }
            catch (Exception exception)
            {
                script.LogError(exception, " in OnPlayerEnteredScope");
            }

            await BaseServer.Delay(0);
        }

        /// <summary>
        /// Overridable method that is triggered when a player leaves another player's scope.
        /// </summary>
        /// <param name="playerHandle">The player handle for which the scope is being left.</param>
        /// <param name="playerLeavingHandle">The player handle that is leaving the scope.</param>
        protected virtual async void OnPlayerLeftScope(string playerHandle, string playerLeavingHandle) { await BaseServer.Delay(0); }
        internal async void onPlayerLeftScope(string playerHandle, string playerLeavingHandle)
        {
            try
            {
                OnPlayerLeftScope(playerHandle, playerLeavingHandle);
            }
            catch (Exception exception)
            {
                script.LogError(exception, " in OnPlayerLeftScope");
            }

            await BaseServer.Delay(0);
        }

        /// <summary>
        /// Overridable method that is triggered when an entity is being created.
        /// </summary>
        /// <param name="entityHandle">The handle of the entity that is being created.</param>
        protected virtual async void OnEntityCreating(int entityHandle) { await BaseServer.Delay(0); }
        internal async void onEntityCreating(int entityHandle)
        {
            try
            {
                OnEntityCreating(entityHandle);
            }
            catch (Exception exception)
            {
                script.LogError(exception, " in OnEntityCreating");
            }

            await BaseServer.Delay(0);
        }

        /// <summary>
        /// Overridable method that is triggered when an entity has been created.
        /// </summary>
        /// <param name="entityHandle">The handle of the entity that was created.</param>
        protected virtual async void OnEntityCreated(int entityHandle) { await BaseServer.Delay(0); }
        internal async void onEntityCreated(int entityHandle)
        {
            try
            {
                OnEntityCreated(entityHandle);
            }
            catch (Exception exception)
            {
                script.LogError(exception, " in OnEntityCreated");
            }

            await BaseServer.Delay(0);
        }

        /// <summary>
        /// Overridable method that is triggered when an entity has been removed.
        /// </summary>
        /// <param name="entityHandle">The handle of the entity that was removed.</param>
        protected virtual async void OnEntityRemoved(int entityHandle) { await BaseServer.Delay(0); }
        internal async void onEntityRemoved(int entityHandle)
        {
            try
            {
                OnEntityRemoved(entityHandle);
            }
            catch (Exception exception)
            {
                script.LogError(exception, " in OnEntityRemoved");
            }

            await BaseServer.Delay(0);
        }

        #endregion
    }
}
