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
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using System.Threading.Tasks;

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
        TextView tempHighTextView;
        TextView tempLowTextView;
        TextView windSpeedTextView;
        TextView airPressureTextView;
        TextView updateTimeTextView;
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
            tempHighTextView = (TextView)FindViewById(Resource.Id.cell0);
            tempLowTextView = (TextView)FindViewById(Resource.Id.cell1);
            windSpeedTextView = (TextView)FindViewById(Resource.Id.cell2);
            airPressureTextView = (TextView)FindViewById(Resource.Id.cell3);
            updateTimeTextView = (TextView)FindViewById(Resource.Id.textView1);




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

            //checks null input
            if (string.IsNullOrEmpty(place))
            {
                // notification for invalid input
                Toast.MakeText(this, "Invalid City Name", ToastLength.Short).Show();
                return;
            }

            ShowProgressDialogue("Fetching Data");
            // creates the url based on previous variables
            string url = apiBase + place + "&appid=" + apiKey;

            //we're using HttpClient to get a JSON response in string format
            var handler = new HttpClientHandler();
            string result = "";
            HttpClient client = new HttpClient(handler);
            
            //checks if city is valid
            try
            {
                result = await client.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Invalid City Name", ToastLength.Short).Show();
                CloseProgressDialogue();
                return;
            }
            


            var resultObject = JObject.Parse(result);
            string airPressure = resultObject["main"]["pressure"].ToString();
            string windSpeed = resultObject["wind"]["speed"].ToString();
            string weatherDescription = resultObject["weather"][0]["description"].ToString();
            string icon = resultObject["weather"][0]["icon"].ToString();
            double temperature = Convert.ToDouble(resultObject["main"]["temp"]);
            string location = resultObject["name"].ToString();
            string country = resultObject["sys"]["country"].ToString();
            string humidity = resultObject["main"]["humidity"].ToString();
            double high = Convert.ToDouble(resultObject["main"]["temp_max"]);
            double low = Convert.ToDouble(resultObject["main"]["temp_min"]);

            // to make the names all upercase we can:
            weatherDescription = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(weatherDescription);

            //Convert to celsius from kelvin
            string temp = convert(temperature);
            string hi = convert(high);
            string lo = convert(low);

            //formatting
            weatherTypeView.Text = weatherDescription;
            tempTextView.Text = temp;
            placeTextView.Text = (location + ", " + country);
            humidityTextView.Text = humidity + "% Humidity";
            tempHighTextView.Text = "High: " + hi;
            tempLowTextView.Text = "Low: " + lo;
            airPressureTextView.Text = "Pressure: " + airPressure;
            windSpeedTextView.Text = "Wind: " + windSpeed + " mph";
            updateTimeTextView.Text = DateTime.Now.ToString("MMM dd yyyy,hh:mm:ss");
            string date = DateTime.Now.ToString("MMM dd yyyy, hh:mm:ss");

            //we'll be using a webrequest to download the icon
            Bitmap bitmap = await IconAsync(icon);
            CloseProgressDialogue();


        }


        private async Task<Bitmap> IconAsync(string icon)
        {
            System.Net.WebRequest request = default(System.Net.WebRequest);
            request = WebRequest.Create("http://openweathermap.org/img/wn/" + icon + ".png");
            request.Timeout = int.MaxValue;
            request.Method = "GET"; // we're fetching data from a server
            WebResponse response = default(WebResponse);
            response = await request.GetResponseAsync();
            MemoryStream ms = new MemoryStream();
            response.GetResponseStream().CopyTo(ms);
            byte[] imageData = ms.ToArray();
            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            weatherImageView.SetImageBitmap(bitmap);

            return bitmap;
        }

        string convert(double num)
        {
            num -= 273.15;
            num = (num * 1.8 + 32);
            string temp = num.ToString();
            temp = String.Format("{0:0.00}", num);
            return temp;
        }

        void ShowProgressDialogue(string status)
        {
            progressDialogue = new progressDialogueFragment(status);
            var transaction = SupportFragmentManager.BeginTransaction();
            progressDialogue.Cancelable = false;
            progressDialogue.Show(transaction, "progress");
        }

        void CloseProgressDialogue()
        {
            progressDialogue.Dismiss();
        }
    }

}
