using Algorithmia;
using CTITopicModelingAPI.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace CTITopicModelingAPI.Controllers
{
    public class TopicModelingController : ApiController
    {

        [HttpPost]
        public Response Post(Request request)
        {
            string responseText = "";
           
            try
            {

                if (request.QueryResult != null)
                {
                    //Check the Intent Name
                    switch (request.QueryResult.Intent.DisplayName.ToLower())
                    {
                        case "searchtext":

                            if (request.QueryResult.Parameters["any"].ToString() != "")
                            {                       
                                string userRequest = request.QueryResult.Parameters["any"].ToString();
                                responseText = GetDocuments(userRequest);
                            }
                            else {
                                responseText = "Input is invalid";
                            }
                            
                            break;

                    }
                }

                return new Response() { fulfillmentText = responseText, source = $"API.AI" };
            }

            catch (Exception ex)
            {
                return new Response() { fulfillmentText = "Api Error", source = $"API.AI" };
            }
        } 




        public static string GetDocuments(string userRequest)
        {

            string responseText = "";
            try
            {
                int? topicIndex = null;
                StringBuilder searchresult = new StringBuilder();

                List<string> filenames = new List<string>();
                string docsList = "[";
                foreach (string file in Directory.GetFiles(HttpContext.Current.Server.MapPath("~/Files")))
                {
                    filenames.Add(Path.GetFileName(file));
                    docsList = docsList + "\"" + File.ReadAllText(file, System.Text.Encoding.Default) + "\"" + ",";
                }
                docsList = docsList + "]";

                // LDA Input custom settings
                var input = "{"
                + "  \"docsList\":" + docsList + ","
                + " \"customSettings\": {"
                + "         \"numTopics\":" + 10 + ","
                + "         \"numIterations\":" + 300 + ","
                + "         \"numWords\":" + 10
                + "         }"
                + "}";

                var client = new Client("simF0eQKLdDbLZQSe6FMv0fjiES1");
                var algorithm = client.algo("nlp/LDA/1.0.0");
                var topicsResponse = algorithm.pipeJson<object>(input);

                var data = topicsResponse.result.ToString().Replace("{[", "[").Replace("}]", "]");

                var topicsList = JArray.Parse(data).Children().ToList();

                for (int i = 0; i < topicsList.Count; i++)
                {
                    if (topicsList[i][userRequest] != null)
                    {
                        topicIndex = i;

                       // searchresult.Append("The count of " + userRequest + " keyword is : " + topicsList[i][userRequest] + ". \n");

                        //  searchresult.Append("The " + userRequest + " present in Topic " + (topicIndex + 1) + " are " + topicsList[Convert.ToInt32(topicIndex)] + "\n");
                        break;
                    }
                }

                if (topicIndex != null)
                {

                    // LDA Mapper : Gives the response as distrubution of topics according to input documents
                    var secondinput = "{"
                   + " \"topics\":" + topicsResponse.result + ","
                   + " \"docsList\":" + docsList
                   + "}";

                    var client1 = new Client("simF0eQKLdDbLZQSe6FMv0fjiES1");
                    var algorithm1 = client1.algo("nlp/LDAMapper/0.1.1");
                    var response1 = algorithm1.pipeJson<object>(secondinput);

                    var data1 = response1.result.ToString().Replace("{[", "[").Replace("}]", "]");

                    var topicDistributionList = JObject.Parse(data1)["topic_distribution"].Children().ToList();
                                  
                    searchresult.Append(" This " + userRequest + " keyword presented in Topic " + (topicIndex + 1) + " and distributed among documents are : ");

                    for (int i = 0; i < topicDistributionList.Count; i++)
                    {
                        searchresult.Append(filenames[i] + " : " + Math.Round((Convert.ToDecimal(topicDistributionList[i]["freq"][Convert.ToString(topicIndex)]) * 100),2) + "%" + ", ");                   
                    }

                    responseText = searchresult.ToString();
                }
                else
                {
                    responseText = "Sorry! No results are found";
                }

                return responseText;
            }
            catch (Exception ex)
            {
                responseText = ex.Message;

                return responseText;
            }

        }

    
    }
}

