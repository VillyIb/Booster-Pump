using BoosterPumpConfiguration;
using Microsoft.Extensions.Options;

namespace BoosterPumpApplicationAsync
{
    // This is a template for classes where ControllerSettings are injected.

    /// <summary>
    /// ControllerService is dynamically reloaded when .json file is modified.
    /// </summary>
    public class ControllerService
    {
        public ControllerSettings ControllerSettings { get; }

        public ControllerService(IOptionsSnapshot<ControllerSettings> controllerSettings)
        {
            ControllerSettings = controllerSettings.Value;
        }

        //public bool Enabled => bool.TryParse(ControllerSettings.Enabled, out var enabled) && enabled;
        public bool Enabled => ControllerSettings.Enabled;

        //public float MinSpeedPct => float.TryParse(ControllerSettings.MinSpeedPct, out var minSpeedPct) ? minSpeedPct : 0f;
        public float MinSpeedPct => ControllerSettings.MinSpeedPct;

    }
}
