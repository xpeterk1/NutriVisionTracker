using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NutriVisionTracker.Model;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace NutriVisionTracker.Controllers
{
    public enum ReplyStatus 
    {
        OK, ERR
    }

    public class GPTController
    {
        readonly string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        private OpenAIAPI API { get; set; }

        private string PredictFoodSystemMessage =
            "You are a food nutrition expert estimating nutrition values in food. " +
            "You always output the name, and nutrition values in a JSON format. Always provide amount of calories, fats, proteins, carbs and fibers in this order." +
            "Do NOT estimate range, just one number for each. " +
            "Always provide numbers for all requested values. If the food does not contain e.g. any proteins, return 0 for proteins." +
            "Always provide numbers as integers, round up if the estimated value in a floating point." +
            "In the output json also include a property called status. It will have a value of OK, if you successfully managed to identift the food in the picture. If you don't think there's" +
            "a food on the picture, set the value to ERR.";

        private string UpgradeFoodSystemMessage = "You are a nutrition expert helping people achieving nutrition goals. " +
            "People will tell you their goals and what they ate during the day. " +
            "Analyze their goals and then suggest more appropriate alternatives for each food given current progresses and targets of the user." +
            "Provide respnse for every food, do not use phrases like `as above` or `as mentioned above`." +
            "The most important thing is to ALWAYS OUTPUT ONLY THE RESULTS IN A JSON FILE where the key will be the number of the food and the value will be the list of your suggestions. " +
            "Output a list of 1-3 suggestions for to improve this particular food to achieve derised goals in an array, aim for shortened desription.";

        public GPTController()
        {
            API = new OpenAIAPI(apiKey);
        }

        public async Task<Food> EstimateFoodFromPicture(string path) 
        {
            ChatRequest request = new ChatRequest()
            {
                Model = OpenAI_API.Models.Model.GPT4_Vision,
                MaxTokens = 500
            };

            var chat = API.Chat.CreateConversation(request);
            chat.AppendSystemMessage(PredictFoodSystemMessage);
            chat.AppendUserInput("Estimate values in this image.", ChatMessage.ImageInput.FromFile(path, "low"));
            string response = await chat.GetResponseFromChatbotAsync();

            ReplyStatus status = ProcessJsonIntoFood(response, out Food food);

            return food;
        }

        public async Task UpgradeFood(User user, List<Food> foods) 
        {
            double proteins = foods.Sum(x => x.Proteins);
            double fats = foods.Sum(x => x.Fats);
            double carbs = foods.Sum(x => x.Carbohydrates);
            double fiber = foods.Sum(x => x.Fiber);
            double cals = foods.Sum(x => x.Calories);

            StringBuilder userMessage = new StringBuilder("My Progress:\n");
            string userProgress = $"Calories: {cals}g/{user.CaloriesGoal}g\nProteins: {proteins}g/{user.ProteinsGoal}g\nCarbohydrates: {carbs}g/{user.CarbsGoal}g\n" +
                $"Fats: {fats}g/{user.FatsGoal}g\nFiber: {fiber}g/{user.FiberGoal}g\n\n";
            userMessage.Append(userProgress);

            userMessage.AppendLine("What I ate during the day:");
            for (int i = 0; i < foods.Count; i++) 
            {
                Food f = foods[i];
                string foodTxt = $"{i + 1}: {f.Name}, calories: {f.Calories}, proteins: {f.Proteins}g, fats: {f.Fats}g, carbohydrates: {f.Carbohydrates}g, fibers: {f.Fiber}g\n";
                userMessage.AppendLine(foodTxt);
            }

            ChatRequest request = new ChatRequest()
            {
                Model = OpenAI_API.Models.Model.GPT4_Turbo,
                MaxTokens = 500
            };

            ChatMessage message = new ChatMessage(ChatMessageRole.User, userMessage.ToString());

            var chat = API.Chat.CreateConversation(request);
            chat.AppendSystemMessage(UpgradeFoodSystemMessage);
            chat.AppendMessage(message);
            string response = await chat.GetResponseFromChatbotAsync();

            ProcessFoodUpgradeJson(response, foods);
        }

        private ReplyStatus ProcessJsonIntoFood(string json, out Food foodOut) 
        {
            int startIndex = json.IndexOf('{');
            int endIndex = json.LastIndexOf('}');
            string jsonInput = json.Substring(startIndex, endIndex - startIndex + 1);

            JsonDocument jsonFile = JsonDocument.Parse(jsonInput);
            JsonElement root = jsonFile.RootElement;

            ReplyStatus status = (ReplyStatus)System.Enum.Parse(typeof(ReplyStatus), root.GetProperty("status").GetString(), true);

            if (!status.Equals(ReplyStatus.OK)) 
            {
                foodOut = new Food(string.Empty, -1, -1, -1, -1, -1);
                return status;
            }

            string name = root.GetProperty("name").GetString();
            int calories = root.GetProperty("calories").GetInt32();
            int fats = root.GetProperty("fats").GetInt32();
            int proteins = root.GetProperty("proteins").GetInt32();
            int carbs = root.GetProperty("carbs").GetInt32();
            int fiber = root.GetProperty("fibers").GetInt32();

            foodOut = new Food(name, calories, fats, proteins, carbs, fiber);

            return status;
        }

        private void ProcessFoodUpgradeJson(string json, List<Food> foods) 
        {
            int startIndex = json.IndexOf('{');
            int endIndex = json.LastIndexOf('}');
            string jsonInput = json.Substring(startIndex, endIndex - startIndex + 1);

            JsonDocument jsonFile = JsonDocument.Parse(jsonInput, new JsonDocumentOptions());
            JsonElement root = jsonFile.RootElement;

            for (int i = 0; i < foods.Count; i++) 
            {
                foods[i].Suggestion = string.Join("\n\n", root.GetProperty((i + 1).ToString()).EnumerateArray());
            }
        }
    }
}
