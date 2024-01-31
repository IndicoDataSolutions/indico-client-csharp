using System;

namespace IndicoV2.IntegrationTests.Utils.Configs
{
    internal class IndicoConfigs
    {
        public int WorkflowId => ParseEnvVar("INDICO_TEST_WORKFLOW_ID");

        public int DatasetId => ParseEnvVar("INDICO_TEST_DATASET_ID");

        public int DocumentDatasetId => ParseEnvVar("INDICO_TEST_DOCUMENT_DATASET_ID");
        public int CsvDatasetId => ParseEnvVar("INDICO_TEST_CSV_DATASET_ID");

        public int ModelGroupId => ParseEnvVar("INDICO_TEST_MODELGROUP_ID");

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
