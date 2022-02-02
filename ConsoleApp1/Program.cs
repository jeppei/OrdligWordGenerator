using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp1 {
    class Program {
        static void Main(string[] args) {

            IEnumerable<string> wordsAll = File.ReadAllText("C:\\Users\\jeslu16\\source\\repos\\ConsoleApp1\\ConsoleApp1\\WordsAll.txt").Split(' ').Select(w => w.ToLower());

            Console.WriteLine($"WordsAll contains {wordsAll.Count()} words");
            List<string> wordsUnique = GetUniqueWords(wordsAll);

            Console.WriteLine($"There are {wordsUnique.Count} unique words");

            List<string> results = new List<string>();
            for (int i = 0; i < 32; i++) {
                int[] binary = Convert.ToString(i, 2).PadLeft(5, '0').ToArray().Select(c => int.Parse(c.ToString())).ToArray();
                string result = GetBestWords(wordsUnique, binary);
                
                if (!results.Contains(result)) {
                    results.Add(result);
                    Console.WriteLine(result);
                }
            }
            Console.WriteLine("Finished!");
        }

        private static string GetBestWords(List<string> wordsUnique, int[] bestWordIndex) {
            List<string> bestWords = new List<string>();
            for (int i = 0; i < 5; i++) {
                wordsUnique = FindBestWord(wordsUnique, bestWordIndex[i], out string bestWord);
                bestWords.Add(bestWord);
            }
            return string.Join(", ", bestWords);
        }

        private static List<string> GetUniqueWords(IEnumerable<string> wordsAll) {
            List<string> wordsUnique = new List<string>();
            foreach (string word in wordsAll) {

                Dictionary<char, bool> charExists = new Dictionary<char, bool>();
                if (word.Length != 5) goto end_of_loop;

                foreach (char c in word) {
                    if (charExists.ContainsKey(c) && charExists[c] == true) goto end_of_loop;
                    charExists.Add(c, true);
                }
                wordsUnique.Add(word);
            end_of_loop: { };
            }
            return wordsUnique;
        }

        private static List<string> FindBestWord(List<string> words, int bestWordIndex, out string bestWord) {
            List<string> newWordList = new List<string>();
            ToppWords topWords = new ToppWords();

            using (StreamWriter writetext = new StreamWriter("C:\\Users\\jeslu16\\source\\repos\\ConsoleApp1\\ConsoleApp1\\WordsUnique.txt")) {

                foreach (string word1 in words) {
                    int count = 0;
                    foreach (string word2 in words) {
                        foreach (char c in word1) {
                            if (word2.Contains(c)) goto end_of_loop;
                        }
                        count++;
                    end_of_loop: { };
                    }
                    writetext.WriteLine(word1 + " " + count);
                    topWords.Add(word1, count);
                }
            }

            bestWord = "(none)";
            if (topWords.Count != 0) {
                bestWord = topWords[Math.Min(bestWordIndex, topWords.Count-1)];

                foreach (string word in words) {
                    foreach (char c in word) {
                        if (bestWord.Contains(c)) goto end_of_loop;
                    }
                    newWordList.Add(word);
                end_of_loop: { };
                }
            }

            //Console.WriteLine($"{words.Count} words left. Best words are: {topWords}");
            return newWordList;
        }

        internal class ToppWords: List<string>{
            readonly Dictionary<string, int> Score = new Dictionary<string, int>();

            internal void Add(string word, int score) {

                if (Contains(word)) return;
                for (int i = 0; i < Count; i++) {
                    if (score > Score[this[i]]) {
                        base.Insert(i, word);
                        Score[word] = score;
                        break;
                    }
                }

                if (Count > 10) RemoveAt(10);

                if (Count == 0) {
                    base.Add(word);
                    Score[word] = score;
                }
            }

            public override string ToString() => string.Join(", ", this.Select(w => $"{w}({Score[w]})"));
        }
    }
}