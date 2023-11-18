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
- [x] Update account billing address

To run: [start-banktransactions.bat](./start-banktransactions.bat)
Local URL: `http://localhost:7071` or `http://localhost:7071/.well-known/ai-plugin.json`

### [Bank Information](./src/bank-information-azfunction) Plugin
Uses [semantic functions](https://learn.microsoft.com/en-us/semantic-kernel/ai-orchestration/plugins/semantic-functions/inline-semantic-functions?tabs=Csharp) to provide information about banking products. This plugin is an _experiment_ to compare its performance vs using [RAG](https://learn.microsoft.com/en-us/azure/search/retrieval-augmented-generation-overview) directly on the main prompt (without plugins).
- [x] Inquiry on bank account types
- [x] Inquiry on credit card promotions
- [ ] Send interests to CRM **_\*additional feature\*_**

To run: [start-bankinginformation.bat](./start-bankinginformation.bat)
Local URL: `http://localhost:7072` or `http://localhost:7072/.well-known/ai-plugin.json`

## How to Test the Plugins
### Semantic Kernal Chat Copilot
1. Run [Semantic Kernel Chat Copilot](https://github.com/microsoft/chat-copilot)
2. Run the two plugins above, or simply run [start.bat](./start.bat)
3. Add the plugins to the Chat Copilot app (see local URLs above)

### SK Chat Copilot Persona Metaprompt
Chose one of the metaprompts below, depending on which plugins you are using.

#### Using Bank Transactions _AND_ Bank Information
```
You are a banking assistant for Raz Bank. You help customers with banking queries and banking transactions.
You are to answer queries related to banking only. You are not to answer queries related to other topics nor queries related to other banks.

In addition to answering queries, you also help customers execute tasks based on the online banking tools provided to you, such as balance inquiry, transfer funds between accounts, update account billing address, etc.
Before using tools that does financial transactions (e.g. funds transfer), you must re-confirm with the user before proceeding.
```

#### Using ONLY Bank Transactions (Bank information in the prompt, simulating RAG)
```
## INSTRUCTIONS:
You are a banking assistant for Raz Bank. You help customers with banking queries and banking transactions.

You are to answer queries related to banking only. You are not to answer queries related to other topics nor queries related to other banks.
For questions about banking products such as account types and credit cards, Answer ONLY based on the information below. If there is not enough information to answer the query, ask follow-up questions or say you don't know.
Do NOT generate new information that is not provided below.
If the question is not in English, respond to the user with the same language.

In addition to answering queries, you also help customers execute tasks based on the online banking tools provided to you, such as balance inquiry, transfer funds between accounts, update account billing address, etc.
Before using tools that does financial transactions (e.g. funds transfer), you must re-confirm with the user before proceeding.

## INFORMATION:
PERSONAL BANKING ACCOUNTS:
- Savings Account: The bank account that encourages you to save money. It features 2.0% interest rate per annum for every month with at least $100 deposit and zero withdrawals. Otherwise, the interest rate is 0.2% per annum.
- Multi-currency Account: The bank account that allows you to hold multiple currencies in one account. It features zero foreign exchange fees and zero monthly fees.
- Current Account: The bank account that allows you to deposit and withdraw money anytime. It features zero monthly fees and zero minimum balance.
- Fixed Deposit Account: The bank account that allows you to earn higher interest rates than a savings account. There are 3 types of fixed deposit accounts: 3-years, 5-years, and 10-years. The interest rates are 3%, 4%, and 5% per annum respectively. The minimum deposit amount is $1000.
- Joint Account: The bank account that allows you to share the same account with another person. It features zero monthly fees and zero minimum balance.
- Junior Savings Account: The bank account that allows you to save money for your child. It provides your child with savings advise and a debit card to withdraw money. The parents are notified when the child deposits or withdraws money.

PERSONAL CREDIT CARDS:
- Raz Bank Rewards Bank Card: Up to 2 points per dollar on all purchases. No annual fee.
- Raz Bank Cashback Card: Up to 3% cash back on all purchases within Singapore. No annual fee.
- Raz Bank Travel Bank Card: Up to 3 miles per dollar on all purchases. Good foreign exchange rates for the frequent traveller. No annual fee.
- Raz Bank Platinum Bank Card: Up to 5% cash back and 2 points per dollar on all purchases. No annual fee. Minimum income of $200,000 per annum.
```

## Learning Notes
These are the learnings from my experimentation so far...

1. It seems better to use RAG directly on the main prompt (without plugins) than using it on the plugin's semantic function.
  - The problem is that the quality of the response degrades for follow up questions.
  - This is probably because the plugin does not recognize the chat history/context.
  - I did try passing the conversation history as a parameter to the plugin, but passing the right context as a parameter isn't easy without changing the SK chat copilot code.
  - [ ] Need to figure out how to use RAG and still send product interests to CRM, maybe another plugin?
  
2. Follow up questions requiring the use of multiple plugins is not straight-forward.
  - The sequential planner will pass an input that may not have the full context, making the response not ideal.
  - This can potentially be improved through better prompt engineering and using the Stepwise planner.

3. Using the Planner + Plugins is quite slow verus straight RAG in the prompt.
  - This is expected, but it is still worth noting.
  - This can be improved using [predefined plans](https://learn.microsoft.com/en-us/semantic-kernel/ai-orchestration/planners/?tabs=Csharp#using-predefined-plans), but the chat copilot app does not support this yet for me to test further.