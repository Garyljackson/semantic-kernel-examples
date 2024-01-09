using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace _04_Native_Functions.Plugins;

public class MathPlugin
{
    [KernelFunction, Description("Take the square root of a number")]
    public static double Sqrt(
        [Description("The number to take a square root of")] double number1
    )
    {
        return Math.Sqrt(number1);
    }

    [KernelFunction, Description("Add two numbers")]
    public static double Add(
        [Description("The first number to add")] double number1,
        [Description("The second number to add")] double number2
    )
    {
        return number1 + number2;
    }

}
