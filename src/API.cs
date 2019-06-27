using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace sortbot
{
    public class SortBot
    {
        private const string baseURL = "https://api.noopschallenge.com";
        private static readonly HttpClient client = new HttpClient();

        public SortBot()
        {
            client.BaseAddress = new System.Uri(baseURL);
        }

        public async Task<queryResponse> Main()
        {
            var response = await client.GetAsync("/sortbot");
            Stream dataStream = await response.Content.ReadAsStreamAsync();
            using (StreamReader streamReader = new StreamReader(dataStream))
            {
                return JsonConvert.DeserializeObject<queryResponse>(streamReader.ReadToEnd());
            }
        }

        public async Task<startExamResponse> StartExam(string username)
        {   
            var content = new StringContent($@"{{
                ""login"": ""{username}""
            }}");

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync("/sortbot/exam/start", content);

            Stream dataStream = await response.Content.ReadAsStreamAsync();
            using (StreamReader streamReader = new StreamReader(dataStream))
            {
                return JsonConvert.DeserializeObject<startExamResponse>(streamReader.ReadToEnd());
            }
        }

        public async Task<Exam> ReadQuestion(string nextSet)
        {  
            var response = await client.GetAsync(nextSet);
            Stream dataStream = await response.Content.ReadAsStreamAsync();

            using (StreamReader streamReader = new StreamReader(dataStream))
            {
                return JsonConvert.DeserializeObject<Exam>(streamReader.ReadToEnd());
            }
        }

        public async Task<ExamResults> AnswerQuestion(string question, List<dynamic> Answer) 
        {
            var content = new StringContent($@"{{
                ""solution"": [{string.Join(',', Answer)}]
            }}");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(question, content);

            Stream dataStream = await response.Content.ReadAsStreamAsync();
            using (StreamReader streamReader = new StreamReader(dataStream))
            {
                return JsonConvert.DeserializeObject<ExamResults>(streamReader.ReadToEnd());
            }
        }
    }

    public struct queryResponse
    {
        public string message;
    }

    public struct startExamResponse {
        public string message;
        public string nextSet;
    }

    public struct Exam {
        public string name;
        public string message;
        public string setPath;
        public dynamic[] question;
    }

    public struct ExamResults {
        public string result;
        public string message;
        public int elapsed;
        public string nextSet;
    }
}