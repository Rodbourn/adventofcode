namespace AdventOfCode2023
{
    internal class Day001
    {
        public static string Run(string input)
        {
            input = input.ToLower();

            var answer = input.Split("\n")
                .Select(line => {
                    var lineDictionary = new Dictionary<int, int>();
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (char.IsDigit(line[i]))
                        {
                            lineDictionary[i] = line[i] - '0';
                        }
                        else if (line.Substring(i).StartsWith("zero"))
                        {
                            lineDictionary[i] = 0;
                        }
                        else if (line.Substring(i).StartsWith("one"))
                        {
                            lineDictionary[i] = 1;
                        }
                        else if (line.Substring(i).StartsWith("two"))
                        {
                            lineDictionary[i] = 2;
                        }
                        else if (line.Substring(i).StartsWith("three"))
                        {
                            lineDictionary[i] = 3;
                        }
                        else if (line.Substring(i).StartsWith("four"))
                        {
                            lineDictionary[i] = 4;
                        }
                        else if (line.Substring(i).StartsWith("five"))
                        {
                            lineDictionary[i] = 5;
                        }
                        else if (line.Substring(i).StartsWith("six"))
                        {
                            lineDictionary[i] = 6;
                        }
                        else if (line.Substring(i).StartsWith("seven"))
                        {
                            lineDictionary[i] = 7;
                        }
                        else if (line.Substring(i).StartsWith("eight"))
                        {
                            lineDictionary[i] = 8;
                        }
                        else if (line.Substring(i).StartsWith("nine"))
                        {
                            lineDictionary[i] = 9;
                        }
                        else
                        {
                            lineDictionary[i] = -1;
                        }
                    }
                    var digits = lineDictionary.Where(kvp => kvp.Value >= 0).OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).DefaultIfEmpty(0);
                    var number = digits.First() * 10 + digits.Last();
                    return number;
                })
                .Sum();

            return answer.ToString();
        }
    }
}
