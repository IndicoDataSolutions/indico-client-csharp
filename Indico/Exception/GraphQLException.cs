using System;
using System.Text;
using GraphQL;

namespace Indico.Exception
{
    [Serializable]
    public class GraphQLException : System.Exception
    {
        public GraphQLException() { }

        public GraphQLException(string message) : base(message) { }

        public GraphQLException(GraphQLError[] errors) : base(SerializeGraphQLErrors(errors)) { }

        public GraphQLException(GraphQLError[] errors, System.Exception inner) : base(SerializeGraphQLErrors(errors), inner) { }

        protected GraphQLException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        private static string SerializeGraphQLErrors(GraphQLError[] errors)
        {
            var builder = new StringBuilder();
            int i = 0;
            builder.AppendLine();
            foreach (var err in errors)
            {
                builder.AppendLine($"{++i} : {err.Message}");
            }
            return builder.ToString();
        }
    }
}