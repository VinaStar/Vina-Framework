﻿using System;
using System.Threading.Tasks;

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
        protected virtual void OnModuleInitialized()
        {
            script.Log($"Initialized!");
        }
        private async Task initialize()
        {
            script.Log($"Initializing...");

            script.RemoveInternalTick(initialize);

            await BaseClient.Delay(0);

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

        #endregion
    }
}
