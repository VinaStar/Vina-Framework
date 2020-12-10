using System;
using System.Threading.Tasks;

using CitizenFX.Core;

namespace VinaFrameworkClient.Core
{
    /// <summary>
    /// Extend all your modules with the Module class and add them to your Client class constructor.
    /// </summary>
    public abstract class Module
    {
        /// <summary>
        /// Extend all your modules with the Module class and add them to your Client class constructor.
        /// </summary>
        /// <param name="client">A BaseClient class</param>
        public Module(BaseClient client)
        {
            Name = this.GetType().Name;
            this.client = client;
            BaseClient.RegisterScript(script = new ModuleScript(this));
            script.AddInternalTick(initialize);
            script.Log($"Instance created!");
        }

        #region VARIABLES

        /// <summary>
        /// Current module name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Read-only reference to the client instance.
        /// </summary>
        protected BaseClient client { get; }

        /// <summary>
        /// Read-only reference to the module script instance.
        /// </summary>
        protected ModuleScript script { get; }

        #endregion
        #region BASE EVENTS

        /// <summary>
        /// Overridable method that run on first tick only. You can get other module from here.
        /// </summary>
        protected virtual async void OnModuleInitialized() { await BaseClient.Delay(0); }
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

            await BaseClient.Delay(0);
        }

        /// <summary>
        /// Overridable method that run when a resource is starting.
        /// </summary>
        /// <param name="resourceName">The resource name that is starting.</param>
        protected virtual async void OnResourceStarting(string resourceName) { await BaseClient.Delay(0); }
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

            await BaseClient.Delay(0);
        }

        /// <summary>
        /// Overridable method that run when a resource has started.
        /// </summary>
        /// <param name="resourceName">The resource name that started.</param>
        protected virtual async void OnResourceStart(string resourceName) { await BaseClient.Delay(0); }
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

            await BaseClient.Delay(0);
        }

        /// <summary>
        /// Overridable method that run when a resource has stopped.
        /// </summary>
        /// <param name="resourceName">The resource name that stopped.</param>
        protected virtual async void OnResourceStop(string resourceName) { await BaseClient.Delay(0); }
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

            await BaseClient.Delay(0);
        }

        /// <summary>
        /// Overridable method that run when a player has died.
        /// Only work if your client has UseDeadWatcher enabled.
        /// </summary>
        /// <param name="player">The player  died.</param>
        protected virtual async void OnPlayerDied(Player player) { await BaseClient.Delay(0); }
        internal async void onPlayerDied(Player player)
        {
            try
            {
                OnPlayerDied(player);
            }
            catch (Exception exception)
            {
                script.LogError(exception, " in OnPlayerDied");
            }

            await BaseClient.Delay(0);
        }

        /// <summary>
        /// Overridable method that run when a player was dead and is now alive again.
        /// Only work if your client has UseDeadWatcher enabled.
        /// </summary>
        /// <param name="player">The player resurected.</param>
        protected virtual async void OnPlayerResurect(Player player) { await BaseClient.Delay(0); }
        internal async void onPlayerResurect(Player player)
        {
            try
            {
                OnPlayerResurect(player);
            }
            catch (Exception exception)
            {
                script.LogError(exception, " in OnPlayerResurect");
            }

            await BaseClient.Delay(0);
        }

        /// <summary>
        /// An event that is triggered when the game triggers an internal network event.
        /// </summary>
        /// <param name="name">The name of the triggered event.</param>
        /// <param name="data">The type-specific event data.</param>
        protected virtual async void OnGameEventTriggered(string name, int[] data) { await BaseClient.Delay(0); }
        internal async void onGameEventTriggered(string name, int[] data)
        {
            try
            {
                OnGameEventTriggered(name, data);
            }
            catch (Exception exception)
            {
                script.LogError(exception, " in OnGameEventTriggered");
            }

            await BaseClient.Delay(0);
        }

        /// <summary>
        /// An event that is triggered when a ped is being created by the game population system. The event can be canceled to stop creating the ped.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        /// <param name="z">The Z position.</param>
        /// <param name="modelHash">The intended model.</param>
        /// <param name="overrideCalls">{ setModel(int|string); setPosition(x, y, z) }</param>
        protected virtual async void OnPopulationPedCreating(float x, float y, float z, uint modelHash, dynamic overrideCalls) { await BaseClient.Delay(0); }
        internal async void onPopulationPedCreating(float x, float y, float z, uint modelHash, dynamic overrideCalls)
        {
            try
            {
                OnPopulationPedCreating(x, y, z, modelHash, overrideCalls);
            }
            catch (Exception exception)
            {
                script.LogError(exception, " in OnPopulationPedCreating");
            }

            await BaseClient.Delay(0);
        }

        #endregion
    }
}
