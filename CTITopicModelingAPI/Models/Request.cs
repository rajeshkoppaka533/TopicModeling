using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTITopicModelingAPI.Models
{
    public class Request
    {
        public class TextList
        {
            public List<string> Text { get; set; }
        }

        public class FulfillmentMessage
        {
            public TextList Text { get; set; }
        }

        public class OutputContext
        {
            public string Name { get; set; }
            public int LifespanCount { get; set; }
            public IDictionary<string, object> Parameters { get; set; }
        }

        public class Intent
        {
            public string Name { get; set; }
            public string DisplayName { get; set; }
        }

        public class QueryResults
        {
            public string QueryText { get; set; }
            public IDictionary<string, object> Parameters { get; set; }
            public bool AllRequiredParamsPresent { get; set; }
            public string FulfillmentText { get; set; }
            public List<FulfillmentMessage> FulfillmentMessages { get; set; }
            public List<OutputContext> OutputContexts { get; set; }
            public Intent Intent { get; set; }
            public int IntentDetectionConfidence { get; set; }
            public string LanguageCode { get; set; }
        }
        public string ResponseId { get; set; }
        public string Session { get; set; }
        public QueryResults QueryResult { get; set; }
    }
}