// PresentationPlayLogging
// PresentationPlayLogging.Config
// <author>Kevin Hartman</author>
// $Rev::                                   
// 

namespace Luminator.PresentationPlayLogging.Config.Interfaces
{
    /// <summary>The PresentationPlayLoggingConfig interface.</summary>
    public interface IPresentationPlayLoggingConfig
    {
        PresentationPlayLoggingServerConfig ServerConfig { get; set; }

        PresentationPlayLoggingClientConfig ClientConfig { get; set; }
    }
}