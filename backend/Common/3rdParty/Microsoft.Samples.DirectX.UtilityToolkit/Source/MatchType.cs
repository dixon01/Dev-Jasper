namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    /// <summary>
    /// Used when finding valid device settings
    /// </summary>
    public enum MatchType
    {
        IgnoreInput, // Use the closest valid value to a default 
        PreserveInput, // Use input without change, but may cause no valid device to be found
        ClosestToInput // Use the closest valid value to the input 
    }
}