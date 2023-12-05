using System;

namespace IndicoV2.IntegrationTests.Utils.Configs
{
    internal class IndicoConfigs
    {
        public int WorkflowId => 10473;
        public int DatasetId => 11848;

        public int ModelGroupId => 8321;

        public static int ParseEnvVar(string varName)
        {
            var rawValue = Environment.GetEnvironmentVariable(varName);
            if (!string.IsNullOrEmpty(rawValue))
            {
                int.TryParse(rawValue, out var _intId);
                return _intId;
            }
            return 0;
        }
    }
}
