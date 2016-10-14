namespace WpfSplashScreen.Models.Events
{
    public class ComputeEvent
    {
        public string InstanceId { get; set; }
        public bool IsComputing { get; set; }

        public ComputeEvent(bool isComputing, string instanceId)
        {
            IsComputing = isComputing;
            InstanceId = instanceId;
        }
    }
}