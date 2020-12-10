namespace VinaFrameworkClient.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class NuiRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string action { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public dynamic data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public NuiRequest() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public NuiRequest(string action)
        {
            this.action = action;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="data"></param>
        public NuiRequest(string action, dynamic data)
        {
            this.action = action;
            this.data = data;
        }
    }
}
