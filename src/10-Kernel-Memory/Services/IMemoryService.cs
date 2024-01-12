using _10_Kernel_Memory.Models;

namespace _10_Kernel_Memory.Services;

public interface IMemoryService
{
    Task<bool> StoreText(string text);

    Task<bool> StoreFile(string content, string filename);

    Task<bool> StoreWebsite(string url);

    Task<KernelResponse> AskQuestion(string question);
}