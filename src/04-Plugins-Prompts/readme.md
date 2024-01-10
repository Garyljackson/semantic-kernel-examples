https://learn.microsoft.com/en-us/semantic-kernel/agents/plugins/?tabs=Csharp#what-does-a-plugin-look-like

At a high-level, a plugin is a group of functions that can be exposed to AI apps and services.   
The functions within plugins can then be orchestrated by an AI application to accomplish user requests.   
Within Semantic Kernel, you can invoke these functions either manually or automatically with function calling or planners.  


Just providing functions, however, is not enough to make a plugin.  
To power automatic orchestration with a planner, plugins also need to provide details that semantically describe how they behave.  
Everything from the function's input, output, and side effects need to be described in a way that the AI can understand, otherwise, the planner will provide unexpected results.  