using System;
using System.Collections.Generic;

class Solution {

    static int MATRIX_DIMENSION = 3;
    static int MIN = 1;//lower bound of the values of the magic square
    static int MAX = 9;//upper bound of the values of the magic square
    static int COMBINATIONS_AMOUNT = 288; //constrained combinations amount in a 3*3 matrix
    static int NULL_COST = 0;

    //---------------------------------------------------------------------------------------------------------

    static void Main(String[] args) {
        int[][] matrix = new int[MATRIX_DIMENSION][];
        for (int i = 0; i < MATRIX_DIMENSION; i++) 
            matrix[i] = Array.ConvertAll(Console.ReadLine().Split(' '), Int32.Parse);
        List<List<List<int>>> permutations = new List<List<List<int>>>();
        permutations.AddRange(listMatrixPermutations());
        Console.WriteLine("The cost is: " + CalculateCost(matrix, permutations));

        Console.WriteLine("THE MATRIX PERMUTATIONS ARE THE FOLLOWING: ");
        for (int i = 0; i < permutations.Count; i++) {
            for (int j = 0; j < permutations[i].Count; j++) {
                for (int k = 0; k < permutations[i][j].Count; k++)
                    Console.Write(permutations[i][j][k]);
                Console.WriteLine();
            }
            Console.WriteLine("----------------------------------------------------------------");
        }
        Console.ReadKey();        
    }

    //---------------------------------------------------------------------------------------------------------

    static int CalculateCost(int[][] matrix, List<List<List<int>>> permutations) {
        List<List<int>> matrix_list = new List<List<int>>();
        for (int i = 0; i < MATRIX_DIMENSION; i++) {
            List<int> list = new List<int>();
            for (int j = 0; j < MATRIX_DIMENSION; j++) {
                list.Add(matrix[i][j]);
            }
            matrix_list.Add(list);
        }
        if (IsMagicSquare(matrix_list))
            return NULL_COST;
        return CalculateCostNoMagic(matrix, permutations);
    }

    //---------------------------------------------------------------------------------------------------------

    static void CalculateSums(int[][] matrix, ref int[] rows, ref int[] columns, ref int diagLeft, 
        ref int diagRight) {
        
        int[][] transpose = new int[MATRIX_DIMENSION][];
        for (int i = 0; i < MATRIX_DIMENSION; i++) {
            transpose[i] = new int[MATRIX_DIMENSION];
            for (int j = 0; j < MATRIX_DIMENSION; j++)
                rows[i] += matrix[i][j];
            diagRight += matrix[i][i];
        }
        for (int i = 0; i < MATRIX_DIMENSION; i++)
            for (int j = 0; j < MATRIX_DIMENSION; j++)
                transpose[i][j] = matrix[j][i];

        for (int i = 0; i < MATRIX_DIMENSION; i++) {
            for (int j = 0; j < MATRIX_DIMENSION; j++)
                columns[i] += transpose[i][j];
            diagLeft += transpose[i][MATRIX_DIMENSION - 1 - i];
        }
    }

    //---------------------------------------------------------------------------------------------------------

    static bool IsMagicSquare(List<List<int>> matrix_list) {
        int[][] matrix = new int[MATRIX_DIMENSION][];
        for (int i = 0; i < MATRIX_DIMENSION; i++) {
            matrix[i] = new int[MATRIX_DIMENSION];
            for (int j = 0; j < MATRIX_DIMENSION; j++)
                matrix[i][j] = matrix_list[i][j];
        }   
        
        int[] rows = new int[MATRIX_DIMENSION];
        int[] columns = new int[MATRIX_DIMENSION];
        int diagRight = 0;
        int diagLeft = 0;
        CalculateSums(matrix, ref rows, ref columns, ref diagLeft, ref diagRight);
        bool nomagic = false;
        if (diagLeft != diagRight)
            return nomagic;
        for (int i = 0; i <MATRIX_DIMENSION; i++) {
            if (rows[i] != diagLeft || columns[i] != diagLeft || rows[i]!=columns[i])
                return nomagic;
            for (int j = 0; j < MATRIX_DIMENSION; j++) {
                if (rows[i] != columns[j] || columns[i]!=rows[j] || rows[i] != rows[j] || columns[i] != columns[j])
                    return nomagic;
            }
        }        
        return !nomagic;
    }

    //---------------------------------------------------------------------------------------------------------

    static int MagicConstant() {
        return (MATRIX_DIMENSION * ((int)Math.Pow(MATRIX_DIMENSION, 2) + 1))/2;
    }

    //---------------------------------------------------------------------------------------------------------

    static int CalculateCostNoMagic(int[][] matrix, List<List<List<int>>> permutations) {

        List<int> costs = new List<int>();
        for (int i = 0; i < permutations.Count; i++) {
            int cost = 0;
            for (int j = 0; j < MATRIX_DIMENSION; j++) 
                for (int k = 0; k < MATRIX_DIMENSION; k++) 
                     cost += Math.Abs(permutations[i][j][k] - matrix[j][k]);
            costs.Add(cost);           
        }
        costs.Sort();
        return costs[0];
    }

    //---------------------------------------------------------------------------------------------------------

    static List<List<List<int>>> listMatrixPermutations() {
        int c_magic = MagicConstant();
        List<List<List<int>>> list = new List<List<List<int>>>();
        Random rnd = new Random();
        List<int> numbers = new List<int>();
        List<List<List<int>>> constrained_combination = new List<List<List<int>>>();
        while (constrained_combination.Count < COMBINATIONS_AMOUNT) {
            List<List<int>> combination_matrix = new List<List<int>>();
            int index_comb = 0;
            while (index_comb < MATRIX_DIMENSION) {
                List<int> sub_comb = new List<int>();
                int index = 0;
                while (index < MATRIX_DIMENSION) {
                    if (numbers.Count == 0)
                        for (int i = MIN; i < MAX + 1; i++)
                            numbers.Add(i);
                    int numberRnd = rnd.Next(0, numbers.Count);
                    sub_comb.Add(numbers[numberRnd]);
                    numbers.RemoveAt(numberRnd);
                    index++;
                }
                combination_matrix.Add(sub_comb);
                index_comb++;
            }
            bool IsCMagic = true;
            for (int i = 0; i < MATRIX_DIMENSION; i++) {
                int sum = 0;
                for (int j = 0; j < MATRIX_DIMENSION; j++)
                    sum += combination_matrix[i][j];
                if (sum != c_magic)
                    IsCMagic = false;
            }
            if (IsCMagic && IsMiddleOk(combination_matrix) && 
                !constrained_combination.Exists(x=>IsTheSameMatrix(x, combination_matrix)))
                constrained_combination.Add(combination_matrix);
        }
               
        if (MATRIX_DIMENSION % 2 != 0) {
            for (int i = 0; i < constrained_combination.Count; i++) {
                if (IsMagicSquare(constrained_combination[i]))
                      list.Add(constrained_combination[i]);
            }
        }        
        return list;    
    }

    //---------------------------------------------------------------------------------------------------------

    static bool IsTheSameMatrix(List<List<int>> x, List<List<int>> combination_matrix) {
        bool isTheSame = true;
        for (int i = 0; i < combination_matrix.Count; i++) 
            for (int j = 0; j < combination_matrix[i].Count; j++) 
                if (x[i][j] != combination_matrix[i][j])
                    isTheSame = false;
        return isTheSame;
    }

    //---------------------------------------------------------------------------------------------------------

    static bool IsMiddleOk(List<List<int>> combination_matrix) {
        bool isMiddleOk = false;
        List<int> numbers = new List<int>();
        for (int j = MIN; j < MAX + 1; j++)
            numbers.Add(j);
        if (MATRIX_DIMENSION % 2 != 0)
            if (combination_matrix[MATRIX_DIMENSION / 2][MATRIX_DIMENSION / 2] == numbers[numbers.Count / 2])
                isMiddleOk = true;
        return isMiddleOk;
    }

    //---------------------------------------------------------------------------------------------------------
}
