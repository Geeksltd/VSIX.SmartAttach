namespace System
{
    public static class Similarity
    {
        public static double GetDifference(string solutionName, string poolName)
        {
            if (string.Compare(solutionName, poolName, ignoreCase: true) == 0)
                return 0;

            var solutionWords = solutionName.Split('.');
            var poolWords = poolName.Split('.');

            var totalDistance = 0;
            foreach (var solutionWord in solutionWords)
            {
                foreach (var poolWord in poolWords)
                {
                    totalDistance += Distance(solutionWord, poolWord);
                }
            }
            var substitutions = solutionWords.Length * poolName.Length;
            var normailizedDistance = totalDistance / (substitutions * 1.0);

            // length matters
            if (poolWords.Length == 0)
                return double.MaxValue;
            return normailizedDistance / poolWords.Length;
        }

        // Levenshtein algorithm:
        static int Distance(string firstArgument, string secondArgument)
        {
            int firstArgumentLength = firstArgument.Length;
            int secondArgumentLength = secondArgument.Length;

            // Step 1
            if (firstArgumentLength == 0)
                return secondArgumentLength;

            if (secondArgumentLength == 0)
                return firstArgumentLength;

            // Step 2
            int[,] distance = new int[firstArgumentLength + 1, secondArgumentLength + 1];

            for (int i = 0; i <= firstArgumentLength; i++) { distance[i, 0] = i; }
            for (int j = 0; j <= secondArgumentLength; j++) { distance[0, j] = j; }

            // Step 3
            for (int i = 1; i <= firstArgumentLength; i++)
            {
                //Step 4
                for (int j = 1; j <= secondArgumentLength; j++)
                {
                    // Step 5
                    int cost = (secondArgument[j - 1] == firstArgument[i - 1]) ? 0 : 1;

                    // Step 6
                    distance[i, j] = Math.Min(
                        Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                        distance[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return distance[firstArgumentLength, secondArgumentLength];
        }
    }
}
