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
        protected virtual void OnModuleInitialized() { }
        private async Task initialize()
        {
            script.Log($"Initializing...");

            script.RemoveInternalTick(initialize);
            
            await BaseServer.Delay(0);

            try
            {
                OnModuleInitialized();
                script.Log($"Initialized!");
            }
            catch (Exception exception)
            {
                script.LogError(exception, " in OnModuleInitialized");
            }
        }

        /// <summary>
        /// Overridable method that run when a player is connecting to the server.
        /// </summary>
        /// <param name="player">The player that is connecting.</param>
        protected virtual void OnPlayerConnecting(Player player) { }
        internal void onPlayerConnecting(Player player)
        {
            try
            {
                OnPlayerConnecting(player);
            }
            catch (Exception exception)
            {
                script.LogError(exception, " in OnPlayerConnecting");
            }
        }

        /// <summary>
        /// Overridable method that run when a player has left the server.
        /// </summary>
        /// <param name="player">The player that left.</param>
        /// <param name="reason">The reason that player left.</param>
        protected virtual void OnPlayerDropped(Player player, string reason) { }
        internal void onPlayerDropped(Player player, string reason)
        {
            try
            {
                OnPlayerDropped(player, reason);
            }
            catch (Exception exception)
            {
                script.LogError(exception, " in OnPlayerDropped");
            }
        }

        /// <summary>
        /// Overridable method that run when the client has initialized. (After loading)
        /// </summary>
        /// <param name="player">The player client that has initialized.</param>
        protected virtual void OnPlayerClientInitialized(Player player) { }
        internal void onPlayerClientInitialized(Player player)
        {
            try
            {
                OnPlayerClientInitialized(player);
            }
            catch (Exception exception)
            {
                script.LogError(exception, " in OnPlayerClientInitialized");
            }
        }

        #endregion
    }
}
