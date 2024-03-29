﻿using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.DataSets;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.Configs;
using IndicoV2.IntegrationTests.Utils.DataHelpers;
using IndicoV2.Workflows;
using IndicoV2.Workflows.Models;
using NUnit.Framework;
using Unity;

namespace IndicoV2.IntegrationTests.Workflows
{
    public class WorkflowClientTests
    {
        private IDataSetClient _dataSetClient;
        private DataHelper _dataHelper;
        private IndicoConfigs _indicoConfigs;
        private IWorkflowsClient _workflowsClient;
        private int _workflowId;
        private int _dataSetId;

        [SetUp]
        public async Task SetUp()
        {
            var container = new IndicoTestContainerBuilder().Build();
            _dataSetClient = container.Resolve<IDataSetClient>();
            _indicoConfigs = new IndicoConfigs();
            var _rawDataSetId = _indicoConfigs.DatasetId;
            if (_rawDataSetId == 0)
            {
                var dataset = await _dataHelper.DataSets().GetAny();
                _dataSetId = dataset.Id;
                var workflows = await _workflowsClient.ListAsync(_dataSetId, default);
                _workflowId = workflows.First().Id;
            }
            else
            {
                _dataSetId = _rawDataSetId;
                int _rawWorkflowID = _indicoConfigs.WorkflowId;
                if (_rawWorkflowID == 0)
                {
                    var workflows = await _workflowsClient.ListAsync(_dataSetId, default);
                    _workflowId = workflows.First().Id;
                }
                else
                {
                    _workflowId = _rawWorkflowID;
                }
            }
            _workflowsClient = container.Resolve<IWorkflowsClient>();
            _dataHelper = container.Resolve<DataHelper>();
        }

        [Test]
        public async Task AddData_ShouldReturnResult()
        {

            await _dataSetClient.AddFilesAsync(_dataSetId, new[] { _dataHelper.Files().GetSampleFilePath() }, default);
            var result = await _workflowsClient.AddDataAsync(_workflowId, default);

            result.Should().NotBeNull();
        }

        [Test]
        public async Task ListWorkflows_ShouldReturnResult()
        {
            var result = await _workflowsClient.ListAsync(_dataSetId);
            result.Should().NotBeNull();
            foreach (Workflow wf in result)
            {
                wf.Id.Should().BeGreaterThan(0);
                wf.Name.Should().NotBeNull();
            }
        }

        [Test]
        public async Task GetWorkflow_ShouldReturnResult()
        {
            var result = await _workflowsClient.GetWorkflowAsync(_workflowId);
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().NotBeNull();
        }
    }
}
