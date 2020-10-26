using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Globalization;
using WeatherApp.Fragments;
using System.Net;
using System.IO;
using Android.Graphics;

namespace WeatherApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        
        Button getWeatherButton;
        TextView tempTextView;
        TextView placeTextView;
        TextView weatherTypeView;
        TextView humidityTextView;
        EditText cityEditText;
        ImageView weatherImageView;

        progressDialogueFragment progressDialogue;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);


            // assigns each detail the respective variable
            cityEditText = (EditText)FindViewById(Resource.Id.cityNameText);
            getWeatherButton = (Button)FindViewById(Resource.Id.getWeatherButton);
            weatherImageView = (ImageView)FindViewById(Resource.Id.weatherImage);
            tempTextView = (TextView)FindViewById(Resource.Id.weatherDescriptionText);
            placeTextView = (TextView)FindViewById(Resource.Id.placeText);
            weatherTypeView = (TextView)FindViewById(Resource.Id.weatherType);
            humidityTextView = (TextView)FindViewById(Resource.Id.humidityText);

            getWeatherButton.Click += GetWeatherButton_Click1;
        }

        private void GetWeatherButton_Click1(object sender, EventArgs e)
        {
            string place = cityEditText.Text;
            getWeather(place);
        }

        /// asynchronous request

        async void getWeather(string place)
        {
            string apiKey = "2375e11e0c9a74a10c492f66805dc2ae";
            string apiBase = "https://api.openweathermap.org/data/2.5/weather?q=";
            string unit = "metric";

            if (string.IsNullOrEmpty(place))
            {
                // notification for invalid input
                Toast.MakeText(this, "Invalid City Name", ToastLength.Short).Show();
                return;
            }

            ShowProgressDialogue("Fetching Data");
            // creates the url based on previous variables
            string url = apiBase + place + "&appid=" + apiKey + "&unit=" + unit;

            //we're using HttpClient to get a JSON response in string format
            var handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);
            string result = await client.GetStringAsync(url);

            Console.WriteLine(result);


            var resultObject = JObject.Parse(result);
            string weatherDescription = resultObject["weather"][0]["description"].ToString();
            string icon = resultObject["weather"][0]["icon"].ToString();
            double temperature = Convert.ToDouble(resultObject["main"]["temp"]);
            string location = resultObject["name"].ToString();
            string country = resultObject["sys"]["country"].ToString();
            string humidity = resultObject["main"]["humidity"].ToString();


            // to make the names all upercase we can:
            weatherDescription = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(weatherDescription);
            
            //Convert.ToDouble(temperature);
            temperature = temperature - 273;
            string temp = temperature.ToString();


            weatherTypeView.Text = weatherDescription;
            tempTextView.Text = temp;
            placeTextView.Text = (location + ", " + country);
            humidityTextView.Text = humidity + "% Humidity";


            //we'll be using a webrequest to download the icons
            string imageURL = "http://openweathermap.org/img/wn/" + icon + ".png";
            System.Net.WebRequest request = default(System.Net.WebRequest);
            request = WebRequest.Create(imageURL);
            request.Timeout = int.MaxValue;
            request.Method = "GET"; // we're fetching data from a server

            WebResponse response = default(WebResponse);
            response = await request.GetResponseAsync();
            MemoryStream ms = new MemoryStream();
            response.GetResponseStream().CopyTo(ms);
            byte[] imageData = ms.ToArray();

            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            weatherImageView.SetImageBitmap(bitmap);

            CloseProgressDialogue();
        }





        void ShowProgressDialogue(string status)
        {
            progressDialogue = new progressDialogueFragment(status);
            var trans = SupportFragmentManager.BeginTransaction();
            progressDialogue.Cancelable = false;
            progressDialogue.Show(trans, "progress");
        }

        void CloseProgressDialogue()
        {
            if(progressDialogue != null)
            {
                progressDialogue.Dismiss();
                progressDialogue = null;
            }
        }
    }
}