using System;

namespace AvaloniaSilkOpengles.World2;

public class RandomTable
{
    public double[] Table { get; }

    int Index { get; set; }

    public RandomTable(int number)
    {
        Table = new double[number];
        var random = new Random();
        for (int i = 0; i < number; i++)
            Table[i] = random.NextDouble();
    }

    public RandomTable(double[] table)
    {
        Table = table;
    }

    public RandomTable()
    {
        Table = [];
    }

    public double Next()
    {
        Index++;
        Index = Index < Table.Length ? Index : 0;
        return Table[Index];
    }

    public void ResetIndex()
    {
        Index = 0;
    }

    public double Current()
    {
        return Table[Index];
    }
}