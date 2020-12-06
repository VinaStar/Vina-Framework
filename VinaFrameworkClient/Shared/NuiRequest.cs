namespace VinaFrameworkClient.Shared
{
    public class NuiRequest
    {
        public string action { get; set; }
        public dynamic data { get; set; }

        public NuiRequest() { }
        public NuiRequest(string action)
        {
            this.action = action;
        }
        public NuiRequest(string action, dynamic data)
        {
            this.action = action;
            this.data = data;
        }
    }
}
