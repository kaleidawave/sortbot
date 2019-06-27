using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace sortbot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            SortBot sortbot = new SortBot();

            int count = 0;
            Console.WriteLine("Username:");
            string username = Console.ReadLine();
            var start = await sortbot.StartExam(username);
            string question = start.nextSet;

            while (question != null && count < 1000)
            {
                Exam exam = await sortbot.ReadQuestion(start.nextSet);
                List<dynamic> numbers = new List<dynamic>(exam.question);
                numbers.Sort(); // Uses default sorting
                ExamResults results = await sortbot.AnswerQuestion(exam.setPath, numbers);
                question = results.nextSet;
                Console.WriteLine($"Round {results.elapsed}, Iter: {++count}");
            }
        }
    }
}
