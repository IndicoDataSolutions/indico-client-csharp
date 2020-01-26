using System.Text;
using GraphQL.Common.Response;

namespace Indico.Exception
{
    [System.Serializable]
    public class GraphQLException : System.Exception
    {
        public GraphQLException() { }
        public GraphQLException(GraphQLError[] errors) : base(SerializeGraphQLErrors(errors)) { }
        public GraphQLException(GraphQLError[] errors, System.Exception inner) : base(SerializeGraphQLErrors(errors), inner) { }
        protected GraphQLException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        static string SerializeGraphQLErrors(GraphQLError[] errors)
        {
            StringBuilder builder = new StringBuilder();
            int i = 0;
            builder.AppendLine();
            foreach (GraphQLError err in errors)
            {
                builder.AppendLine($"{++i} : {err.Message}");
            }
            return builder.ToString();
        }
    }
}