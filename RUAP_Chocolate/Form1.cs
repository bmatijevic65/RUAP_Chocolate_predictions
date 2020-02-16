using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Globalization;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace RUAP_Chocolate
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        

       public static float ConvertFunc(string strlist )
        {

            string newString;
            strlist = strlist.Replace("\"", "");  //micanje dvostrukih navodnika
            int startIndex = strlist.IndexOf(":") + ":".Length;
            int endIndex = strlist.IndexOf(",");
            newString = strlist.Substring(startIndex, endIndex - startIndex);  //izdvajanje predviđenih vjerojatnosi
            float s1 = float.Parse(newString); //pretvaranje u float vrijednost
            return s1;
        }

    private string company;
        private string cocoaPercent;
        private string beanType;

        private async void button1_Click(object sender, EventArgs e)
        {
            company = txtCompany.Text.ToString();


            if (trackBar1.Value <= 99)   
            {

                cocoaPercent = "0." + trackBar1.Value.ToString();   //podatci iz trackBara
            }

            else
            {
                
                cocoaPercent = "1.0";
            }

            beanType = listBox1.GetItemText(listBox1.SelectedItem);  //podatci iz ListBoxa

            
            using (var client = new HttpClient())   //Request-Response
            {
                var scoreRequest = new
                {
                    Inputs = new Dictionary<string, List<Dictionary<string, string>>>() {
                        {
                            "input1",
                            new List<Dictionary<string, string>>(){new Dictionary<string, string>(){
                                            {
                                                "Company", company.ToString()
                                            },
                                            {
                                                "Cocoa Percent", cocoaPercent.ToString()
                                            },
                                            {
                                                "Bean Type", beanType.ToString()
                                            },
                                }
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };

              
                const string apiKey = "uCtXmwtCz2IwkYixE/VEbZKM8vFGxFRALAKnZZQSIB2hNcqw7YfN9sdmO88MDPQFek1eeWdUYjK+sctX5Z/ptw=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                client.BaseAddress = new Uri("https://europewest.services.azureml.net/workspaces/e4a7547db8f74b0e87e7382f6d2ace05/services/a1fcb6303b63488fa85e0c4e6a09be1e/execute?api-version=2.0&format=swagger");


                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                  
                  
                    // Taking a string 
                    string[] spearator = { "Scored " };   //ekstrahiranje potrebnih podataka iz varijable result
                    Int32 count = 5;
                    // using the method 
                    String[] strlist = result.Split(spearator, count,
                           StringSplitOptions.RemoveEmptyEntries);

                    float bad = ConvertFunc(strlist[1]);
                    float good = ConvertFunc(strlist[2]);
                    float excelent = ConvertFunc(strlist[3]);


                    float max = Math.Max(Math.Max(bad, good), excelent);
                    string temp;

                    if (max == bad)
                        temp = "BAD";
                    else if (max == good)
                        temp = "GOOD";
                    else
                        temp = "EXCELENT";

                    MessageBox.Show("It's a " + temp + " chocolate", "Result Prediction", MessageBoxButtons.OK);
                }
                else
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("The request failed with status code: {0}" + response.StatusCode, "Error", MessageBoxButtons.OK);
                }
            }

        }
       

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

            if (trackBar1.Value <= 99)   //kod ne radi dobro zbog ovoga
            {
                label5.Text = "0." + trackBar1.Value.ToString();
                
            }

            else
            {
                label5.Text = "1.0";
         
            }
        }

        
    }
}
