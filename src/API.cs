#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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
            var content = new StringContent(JsonConvert.SerializeObject(new { login = username }));

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

        public async Task<ExamResults> AnswerQuestion(string question, Solution solution)
        {
            var content = JsonConvert.SerializeObject(solution);

            var postContent = new StringContent(content);
            postContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(question, postContent);

            Stream dataStream = await response.Content.ReadAsStreamAsync();
            using (StreamReader streamReader = new StreamReader(dataStream))
            {
                return JsonConvert.DeserializeObject<ExamResults>(streamReader.ReadToEnd());
            }
        }

        public async Task<Certificate> RetreiveCertificate(ExamResults exam)
        {
            var response = await client.GetAsync(exam.certificate);
            Stream dataStream = await response.Content.ReadAsStreamAsync();
            using (StreamReader streamReader = new StreamReader(dataStream))
            {
                return JsonConvert.DeserializeObject<Certificate>(streamReader.ReadToEnd());
            }
        }
    }


    public struct queryResponse
    {
        public string message;
    }

    public struct startExamResponse
    {
        public string message;
        public string nextSet;
    }

    public struct Exam
    {
        public string name;
        public Solution exampleSolution;
        public string message;
        public string setPath;
        public List<dynamic> question;
    }

    public struct Solution
    {
        public Solution(List<dynamic> solution)
        {
            this.solution = solution;
        }
        public List<dynamic> solution;
        public override string ToString() => $"solution: [{String.Join(',', this.solution)}] ";
    }

    public struct ExamResults
    {
        public string result;
        public string message;
        public int elapsed;
        public string nextSet;
        public string? certificate;
    }

    public struct Certificate
    {
        public string message;
        public double elapsed;
        public DateTime completed;
        public override string ToString() => $"Certificate: \n{message} Completed at {completed.ToString("h:mm tt MM/dd/yyyy ")}";
    }
}