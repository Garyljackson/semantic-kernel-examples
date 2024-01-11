https://learn.microsoft.com/en-us/semantic-kernel/agents/planners/?tabs=Csharp#what-is-a-planner

A planner is a function that takes a user's ask and returns back a plan on how to accomplish the request. 
It does so by using AI to mix-and-match the plugins registered in the kernel so that it can recombine them into a series of steps that complete a goal.


The handlebars planner is named that becuase internally it is using the handlebars syntax to define the prompt for the planner.
The prompt can be seen here: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/Planners/Planners.Handlebars/Handlebars/CreatePlanPrompt.handlebars

The plan that is produced and executed is also in the handlebars syntax.


When to use a planner  
https://learn.microsoft.com/en-us/semantic-kernel/agents/planners/?tabs=Csharp#when-to-use-a-planner