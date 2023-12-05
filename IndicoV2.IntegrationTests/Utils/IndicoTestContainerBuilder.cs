using System;
using System.Linq;
using IndicoV2.DataSets;
using IndicoV2.Extensions.Jobs;
using IndicoV2.Extensions.SubmissionResult;
using IndicoV2.Jobs;
using IndicoV2.Models;
using IndicoV2.Ocr;
using IndicoV2.Reporting;
using IndicoV2.Reviews;
using IndicoV2.Storage;
using IndicoV2.Submissions;
using IndicoV2.Workflows;
using Unity;
using Unity.Lifetime;

namespace IndicoV2.IntegrationTests.Utils
{
    internal class IndicoTestContainerBuilder
    {
        private readonly UnityContainer _container;

        private string BaseUrl => "https://dev-ci.us-east-2.indico-dev.indico.io";
        private string ApiToken => "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpZCI6NTAzMiwidXNlcl9pZCI6MjY1LCJ1c2VyX2VtYWlsIjoibmF0ZS5zaGltQGluZGljby5pbyIsImlhdCI6MTY5ODE1ODU2OCwiYXVkIjpbImluZGljbzpyZWZyZXNoX3Rva2VuIl19.RJHgXtT3rIad5v1ijJBX9rV7syh7Y9QCualLal3AC4I";


        public IndicoTestContainerBuilder() => _container = new UnityContainer();


        public IUnityContainer Build()
        {
            var isClientRegistered = _container.Registrations.Any(r => typeof(IndicoClient).IsAssignableFrom(r.RegisteredType));

            if (!isClientRegistered)
            {
                RegisterClientDefault();
            }

            _container.RegisterType<IIndicoClient, IndicoClient>();

            _container.RegisterFactory<IDataSetClient>(c => c.Resolve<IndicoClient>().DataSets());
            _container.RegisterFactory<IWorkflowsClient>(c => c.Resolve<IndicoClient>().Workflows());
            _container.RegisterFactory<ISubmissionsClient>(c => c.Resolve<IndicoClient>().Submissions());
            _container.RegisterFactory<IReviewsClient>(c => c.Resolve<IndicoClient>().Reviews());
            _container.RegisterFactory<IJobsClient>(c => c.Resolve<IndicoClient>().Jobs());
            _container.RegisterFactory<IJobAwaiter>(c => c.Resolve<IndicoClient>().JobAwaiter());
            _container.RegisterFactory<IStorageClient>(c => c.Resolve<IndicoClient>().Storage());
            _container.RegisterFactory<ISubmissionResultAwaiter>(c =>
                c.Resolve<IndicoClient>().GetSubmissionResultAwaiter());
            _container.RegisterFactory<IModelClient>(c => c.Resolve<IndicoClient>().Models());
            _container.RegisterFactory<IOcrClient>(c => c.Resolve<IndicoClient>().Ocr());
            _container.RegisterFactory<IUserReportingClient>(c => c.Resolve<IndicoClient>().UserReporting());

            return _container;
        }

        private void RegisterClientAutoReview() => RegisterClient(ApiToken, BaseUrl);

        private void RegisterClientDefault() => RegisterClient(ApiToken, BaseUrl);

        private void RegisterClient(string token, string uri) => _container.RegisterFactory<IndicoClient>(c => new IndicoClient(token, new Uri(uri)), new SingletonLifetimeManager());

        #region Fluent

        public IndicoTestContainerBuilder ForAutoReviewWorkflow() => WrapFluent(RegisterClientAutoReview);

        private IndicoTestContainerBuilder WrapFluent(Action action)
        {
            action();

            return this;
        }

        #endregion Fluent
    }
}
