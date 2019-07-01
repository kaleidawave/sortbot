using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace sortbot
{
    class Program
    {
        const string vowels = "aeiou";
        static async Task Main(string[] args)
        {
            SortBot sortbot = new SortBot();

            Console.WriteLine("Username:");
            string username = Console.ReadLine();
            int stage = 1;
            bool finished = false;

            var start = await sortbot.StartExam(username);
            string question = start.nextSet;

            while (!finished)
            {
                Exam exam = await sortbot.ReadQuestion(question);
                Console.WriteLine($"Stage: {stage}, {exam.name} - {exam.message}");

                List<dynamic> listToSort = new List<dynamic>(exam.question);

                switch (stage)
                {
                    case 1:
                    case 2:
                    case 3:
                        listToSort.Sort();
                        break;
                    case 4:
                        listToSort = listToSort.OrderBy(x => (x as string).Length).Reverse().ToList();
                        break;
                    case 5:
                        listToSort = listToSort.OrderBy(x => (x as string).ToCharArray().Where(c => vowels.Contains(c)).Count()).ToList();
                        break;
                    case 6:
                        listToSort = listToSort.OrderBy(x => (x as string).ToCharArray().Where(c => !vowels.Contains(c)).Count()).ToList();
                        break;
                    case 7:
                        listToSort = listToSort.OrderBy(x => (x as string).Split(' ').Length).ToList();
                        break;

                }
                Solution solution = new Solution(listToSort);
                ExamResults results = await sortbot.AnswerQuestion(exam.setPath, solution);

                Console.WriteLine($"Solved, {results.message} \n");

                if (!(results.certificate is null))
                {
                    Certificate certificate = await sortbot.RetreiveCertificate(results);
                    Console.WriteLine(certificate.ToString());
                    finished = true;
                }
                else
                {
                    question = results.nextSet;
                    stage++;
                }
            }
        }
    }
}
