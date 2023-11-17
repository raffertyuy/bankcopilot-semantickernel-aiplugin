# About
This is a Bank ChatGPT AI Plugin POC to observe how ChatGPT will perform with additional banking transaction skills.

This repo uses [Semantic Kernel](https://github.com/microsoft/semantic-kernel) with **two** ChatGPT Plugins. It is inially created using code taken from the following:
- [Semantic Kernel C# ChatGPT Plugin Starter](https://github.com/microsoft/semantic-kernel-starters)
- [Semantic Kernel Sample ChatGPT Math Plugin](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/14-Create-ChatGPT-Plugin)
- [Semantic Kernel Sample Skills](https://github.com/microsoft/semantic-kernel/tree/main/samples/skills)

## Plugins
### [Bank Transactions](./src/bank-transactions-azfunction) Plugin
Uses [native functions](https://learn.microsoft.com/en-us/semantic-kernel/ai-orchestration/plugins/native-functions/using-the-skfunction-decorator?tabs=Csharp) to perform banking transactions.
- [x] Balance Inquiry
- [x] Transfer funds between accounts
- [ ] Update account billing address

To run: [start-banktransactions.bat](./start-banktransactions.bat)
Local URL: `http://localhost:7071` or `http://localhost:7071/.well-known/ai-plugin.json`

### [Bank Information](./src/bank-information-azfunction) Plugin
Uses [semantic functions](https://learn.microsoft.com/en-us/semantic-kernel/ai-orchestration/plugins/semantic-functions/inline-semantic-functions?tabs=Csharp) to provide information about banking products. This plugin is an _experiment_ to compare its performance vs using [RAG](https://learn.microsoft.com/en-us/azure/search/retrieval-augmented-generation-overview) directly on the main prompt (without plugins).
- [ ] Inquiry on bank account types
- [ ] Inquiry on credit card promotions
- [ ] Send interests to CRM **_\*additional feature\*_**

To run: [start-bankinginformation.bat](./start-bankinginformation.bat)
Local URL: `http://localhost:7072` or `http://localhost:7072/.well-known/ai-plugin.json`

## How to Test the Plugins
### Semantic Kernal Chat Copilot
1. Run [Semantic Kernel Chat Copilot](https://github.com/microsoft/chat-copilot)
2. Run the two plugins above, or simply run [start.bat](./start.bat)
3. Add the plugins to the Chat Copilot app (see local URLs above)

### SK Chat Copilot Persona Metaprompt
```
You are a banking assistant for Raz Bank. You help customers with banking queries and banking transactions.
You are to answer queries related to banking only. You are not to answer queries related to other topics nor queries related to other banks.

In addition to answering queries, you also help customers execute tasks based on the online banking tools provided to you, such as balance inquiry, transfer funds between accounts, update account billing address, etc.
Before using tools that does financial transactions (e.g. funds transfer), you must re-confirm with the user before proceeding.
```