## How to call CreateTenderDeclarationTransactionClientRequest/Response?
- The Microsoft Doc is: https://learn.microsoft.com/en-us/azure/ai-services/openai/quickstart?tabs=command-line%2Cpython-new&pivots=programming-language-csharp
```CS
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Graph.Models;
using System;
using static System.Environment;

string endpoint = "https://******.openai.azure.com/";
string key = "******";

OpenAIClient client = new(new Uri(endpoint), new AzureKeyCredential(key));

for (int i = 0; i < 1; i++)
{
    var chatCompletionsOptions = new ChatCompletionsOptions()
    {
        DeploymentName = "openai10", //This must match the custom deployment name you chose for your model
        Messages =
        {  
            new ChatRequestUserMessage("Can you please generate a json based on the below product description :This product is a T-Shirt with a price of $100. It is categorized as a seasonal item, specifically meant for use in the summer months.The T - Shirt is likely made of a lightweight fabric to ensure breathability and comfort in warmer temperatures.Overall, it appears to be a stylish and practical option for those looking to stay cool and comfortable during the summer"),
        },
        MaxTokens = 3000
    };

   Response<ChatCompletions> response = client.GetChatCompletions(chatCompletionsOptions);
   Console.WriteLine(response.Value.Choices[0].Message.Content);

   Console.WriteLine();
}

```


