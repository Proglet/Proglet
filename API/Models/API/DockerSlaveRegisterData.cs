namespace API.Models.API
{
    /// <summary>
    /// Model with data used by slavemanagers to register themselves
    /// </summary>
    public class DockerSlaveRegisterData
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "<Pending>")]
        public string Url { get; set; }
        public string Auth { get; set; }
    }
}