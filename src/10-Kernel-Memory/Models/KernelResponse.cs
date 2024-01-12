using Microsoft.KernelMemory;

namespace _10_Kernel_Memory.Models;
public class KernelResponse
{
    public string Answer { get; set; }
    public List<Citation> Citations { get; set; }
}