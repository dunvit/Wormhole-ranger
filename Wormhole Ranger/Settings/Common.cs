using Wormhole_Ranger.Tools;

namespace Wormhole_Ranger.Settings
{
    public class Common
    {
        public string EveOnlineTitle => ConfigurationTools.GetConfigOptionalStringValue("EveOnlineTitle", "EVE -");

        public string StatisticVisitorsCounterPage => ConfigurationTools.GetConfigOptionalStringValue("Statistic.VisitorsCounterPage", "EVE -");

        public string EsiAddress => ConfigurationTools.GetConfigOptionalStringValue("EsiAddress");
    }
}
