using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.IO;

namespace FAA_API
{
    class Program
    {
        static void Main()
        {
            //bool testMode = true;
            bool testMode = false;
            FAA_API(testMode);
            Console.ReadLine();
        }
        static async void FAA_API(bool testMode)
        {
            /*
             * ToDo
             * try and get web connection
             * if available continue
             * else throw exception
             * 
             * Implement Try Catch Blocks to handle any exceptions
             */
            if(testMode)
            {
                //Read from local file
                StreamReader reader = new StreamReader("./BWINoDelayResponse.json");
                var jsonFile = JObject.Parse(reader.ReadToEnd());
            }
            else
            {
                //run live program
                Console.Write("Please enter a 3 letter American airport code: ");
                var airportCode = Console.ReadLine();
                Console.WriteLine();

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    using (HttpResponseMessage response = await client.GetAsync("http://services.faa.gov/airport/status" + airportCode))
                    {
                        if (response.StatusCode.ToString() == "NotFound")
                        {
                            Console.WriteLine("Airport not found.");
                        }
                        else
                        {
                            using (HttpContent content = response.Content)
                            {
                                var result = await content.ReadAsStringAsync();
                                JObject json = JObject.Parse(result);
                                Console.WriteLine((string)json["name"]);
                                Console.WriteLine((string)json["city"] + ", " + (string)json["state"]);
                                Console.WriteLine();
                                if((string)json["weather"]["Error"] != null)
                                {
                                    Console.WriteLine("Error retrieving weather.");
                                }
                                else
                                {
                                    Console.WriteLine("Weather: " + (string)json["weather"]["weather"]);
                                    Console.WriteLine("Visibility: " + (string)json["weather"]["visibility"]);
                                    Console.WriteLine("Temperature: " + (string)json["weather"]["temp"]);
                                    Console.WriteLine("Wind: " + (string)json["weather"]["wind"]);
                                    Console.WriteLine("Last Updated: " + (string)json["weather"]["meta"]["updated"]);
                                }
                                if ((string)json["delay"] != "false")
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("Delay: " + (string)json["delay"]);
                                    Console.WriteLine("Reason: " + (string)json["status"]["reason"]);
                                    Console.WriteLine("Closure Begin: " + (string)json["status"]["closureBegin"]);
                                    Console.WriteLine("End Time: " + (string)json["status"]["endTime"]);
                                    Console.WriteLine("Minimum Delay: " + (string)json["status"]["minDelay"]);
                                    Console.WriteLine("Average Delay: " + (string)json["status"]["avgDelay"]);
                                    Console.WriteLine("Maximum Delay: " + (string)json["status"]["maxDelay"]);
                                    Console.WriteLine("Closure End: " + (string)json["status"]["closureEnd"]);
                                    Console.WriteLine("Trend: " + (string)json["status"]["trend"]);
                                    Console.WriteLine("Type: " + (string)json["status"]["type"]);
                                }
                                else
                                    Console.WriteLine("\n" + (string)json["status"]["reason"]);
                            }
                        }
                    }
                }
            }
        }
    }
}