using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using System.IO;
using System.Net;
using Android.Preferences;
using System.Json;
using Java.Util;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace SimpleWeather
{
    public class Fragment_City : Fragment
    {
        private const string APIKEY = "5588384b737ae40c132cb3c1d921d139";
        private const string URL_weather = "http://api.openweathermap.org/data/2.5/weather";
        private const string URL_forecast = "http://api.openweathermap.org/data/2.5/forecast";

        private const int ADD = 1089;
        private const int MODIFY = 1090;

        private View view;
        int id;
        string name;
        Class_Weather city_weather;
        List<Class_Forecast> city_forecast;
        SimpleAdapter adapter;

        private View_Weather weatherView;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            view = inflater.Inflate(Resource.Layout.fragment_city, container, false);

            name = Arguments.GetString("CityName");

            id = Arguments.GetInt("CityId");
            getData(id);

            var imageview_over = view.FindViewById<ImageView>(Resource.Id.imageview_main_overflow);
            imageview_over.Click += delegate
            {
                showPopwindow();
            };

            return view;
        }

        private void getData(int id)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            ISharedPreferencesEditor editor = prefs.Edit();
            string result_weather, result_forecast;

            result_weather = "";//prefs.GetString("weather_" + id, "");
            result_forecast = "";// prefs.GetString("forecast_" + id, "");

            if (result_forecast.Equals("") || result_weather.Equals(""))
            {
                string url_weather = URL_weather + "?" + "id=" + id + "&APPID=" + APIKEY;
                string url_forecast = URL_forecast + "?" + "id=" + id + "&APPID=" + APIKEY;
                result_weather = Get(url_weather);
                result_forecast = Get(url_forecast);

                editor.PutString("weather_" + id, result_weather);
                editor.PutString("forecast_" + id, result_forecast);
                editor.Apply();
            }

            JsonObject json_weather = (JsonObject)JsonValue.Parse(result_weather);
            JsonValue json_forecast = JsonValue.Parse(result_forecast);

            JsonArray weather = (JsonArray)json_weather["weather"];
            JsonObject main = (JsonObject)json_weather["main"];
            JsonObject clouds = (JsonObject)json_weather["clouds"];
            JsonObject winds = (JsonObject)json_weather["wind"];
            Console.WriteLine(main["temp"].ToString());

            city_weather = new Class_Weather();
            city_weather.temperature = (float)main["temp"];
            city_weather.humidity = (int)main["humidity"];
            city_weather.pressure = (int)main["pressure"];
            city_weather.weather = weather[0]["main"].ToString();
            city_weather.weather = city_weather.weather.Substring(1, city_weather.weather.Length - 2);
            string icon = weather[0]["icon"].ToString();
            icon = icon.Substring(1, icon.Length - 2);
            setBackground(icon);
            if (json_weather.ContainsKey("visibility")){
                city_weather.visibility = (int)json_weather["visibility"];
            }
            city_weather.clouds = (int)clouds["all"];
            if (winds.ContainsKey("deg"))
            {
                city_weather.wind_degree = (int)winds["deg"];
            }
            if (winds.ContainsKey("speed"))
            {
                city_weather.wind_speed = (int)winds["speed"];
            }

            city_forecast = new List<Class_Forecast>();
            JsonArray list = (JsonArray)json_forecast["list"];
            for (int i = 0; i < list.Count; i++)
            {
                Class_Forecast forecast = new Class_Forecast();
                forecast.time = list[i]["dt_txt"];
                forecast.weather = list[i]["weather"][0]["main"].ToString();
                forecast.weather = forecast.weather.Substring(1, forecast.weather.Length - 2);
                forecast.icon = list[i]["weather"][0]["icon"].ToString();
                forecast.icon = forecast.icon.Substring(1, forecast.icon.Length - 2);
                //Console.WriteLine(forecast.icon);
                //forecast.icon=Resource.Drawable.
                forecast.temp = (float)list[i]["main"]["temp"];
                city_forecast.Add(forecast);
            }

            setValueWeather();
            setCityForecast();
            drawViewWeather();
            //TextView textweather = view.FindViewById<TextView>(Resource.Id.textview_fragment_weather1);
            //TextView textforecast = view.FindViewById<TextView>(Resource.Id.textview_fragment_forecast1);
            //textweather.Text = json_forecast.ToString();
            //textforecast.Text = result_forecast;
        }

        private void setValueWeather()
        {
            var city_name = view.FindViewById<TextView>(Resource.Id.textview_fragment_city);
            var temperature = view.FindViewById<TextView>(Resource.Id.textview_fragment_temperature);
            var weather = view.FindViewById<TextView>(Resource.Id.textview_fragment_weather);
            city_name.Text = name.Substring(0,name.Length-3);
            temperature.Text = (int)(city_weather.temperature - 273.15) + "бу";
            weather.Text = city_weather.weather;

            var wind_direction = view.FindViewById<TextView>(Resource.Id.textview_fragment_wind_direction);
            var wind_speed = view.FindViewById<TextView>(Resource.Id.textview_fragment_wind_speed);
            var visibility = view.FindViewById<TextView>(Resource.Id.textview_fragment_visibility);
            var humidity = view.FindViewById<TextView>(Resource.Id.textview_fragment_humidity);

            humidity.Text = city_weather.humidity + "%";
            wind_speed.Text = city_weather.wind_speed + "m/s";
            visibility.Text = city_weather.visibility + "m";

            string direction="North";
            if (city_weather.wind_degree > 45 && city_weather.wind_degree <= 135) direction = "East";
            if (city_weather.wind_degree > 135 && city_weather.wind_degree <= 225) direction = "South";
            if (city_weather.wind_degree > 225 && city_weather.wind_degree <= 315) direction = "West";
            wind_direction.Text = direction + " Wind";
        }

        private void setCityForecast()
        {
            DateTime DT = DateTime.Now;
            List<IDictionary<string, object>> mData = new List<IDictionary<string, object>>();
            ListView list_forecast = view.FindViewById<ListView>(Resource.Id.listview_fragment_forecast);

            string[] weeks = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            string dayOfWeek = DT.DayOfWeek.ToString();
            string dateToday = DateTime.Now.ToString("yyyy-MM-dd");
            string date = dateToday;
            float max_temp_of_day = -1000, min_temp_of_day = 50000;
            int dayPass = 0;
            int num_week = 0;
            for (int j = 0; j < 7; j++)
            {
                if (weeks[j].Equals(dayOfWeek)) { num_week = j; break; }
            }

            int i = 0;
            while (i < city_forecast.Count)
            {
                while (city_forecast[i].time.StartsWith(dateToday)) i++;
                if (!city_forecast[i].time.StartsWith(date))
                {
                    dayPass++;
                    max_temp_of_day = -1000; min_temp_of_day = 50000;
                    date = city_forecast[i].time.Substring(0, 10);
                    IDictionary<string, object> item = new JavaDictionary<string, object>();
                    item.Add("date", weeks[(num_week + dayPass) % 7].Substring(0, 3));
                    //Console.WriteLine(city_forecast[i].time + "   " + weeks[(num_week + dayPass) % 7].Substring(0, 3));
                    while (i < city_forecast.Count && city_forecast[i].time.StartsWith(date))
                    {
                        if (city_forecast[i].temp > max_temp_of_day) max_temp_of_day = city_forecast[i].temp;
                        if (city_forecast[i].temp < min_temp_of_day) min_temp_of_day = city_forecast[i].temp;
                        if (city_forecast[i].time.Substring(11, 2).Equals("12"))
                        {
                            item.Add("weather", city_forecast[i].weather);
                            Bitmap bitmap = BitmapFactory.DecodeResource(Resources, getResourceId(city_forecast[i].icon));
                            item.Add("icon", getResourceId(city_forecast[i].icon));
                        }
                        i++;
                    }
                    min_temp_of_day = min_temp_of_day - (float)273.15;
                    max_temp_of_day = max_temp_of_day - (float)273.15;
                    int min = (int)min_temp_of_day, max = (int)max_temp_of_day;
                    item.Add("range", min + "бу" + "/" + max + "бу");
                    //Console.WriteLine("date " + item["date"]);
                    //Console.WriteLine("weather " + item["weather"]);
                    //Console.WriteLine("Range " + item["range"]);
                    mData.Add(item);
                }
            }
            //Console.WriteLine("Count = " + mData.Count);
            adapter = new SimpleAdapter(Context, mData, Resource.Layout.item_listview_forecast,
                            new string[] { "icon", "date", "weather", "range" }, new int[] {Resource.Id.imageview_listview_fragment_icon,
                                Resource.Id.textview_listview_fragment_date,Resource.Id.textview_listview_fragment_weather,
                                Resource.Id.textview_listview_fragment_range});
            list_forecast.Adapter = adapter;
        }

        private void showPopwindow()
        {
            LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
            View view1 = inflater.Inflate(Resource.Layout.popwindow_main, null);

            PopupWindow window = new PopupWindow(view1,
                    WindowManagerLayoutParams.MatchParent,
                    WindowManagerLayoutParams.WrapContent);

            window.Focusable = true;


            Color c = new Color(176, 00, 00, 00);
            ColorDrawable dw = new ColorDrawable(c);
            window.SetBackgroundDrawable(dw);


            window.AnimationStyle = (Resource.Style.mypopwindow_anim_style);
            window.ShowAtLocation(view.FindViewById(Resource.Id.lin_fragment_all), GravityFlags.Top, 0, 0);

            ImageView button_close = view1.FindViewById<ImageView>(Resource.Id.imageview_popwindow_close);
            button_close.Click += delegate
            {
                window.Dismiss();
            };


            TextView textview_modify = view1.FindViewById<TextView>(Resource.Id.textview_popwindow_main_modify);
            textview_modify.Click += delegate
            {
                window.Dismiss();
                Intent intent = new Intent(Context, typeof(Activity_CityList));
                Activity.StartActivityForResult(intent, MODIFY);
            };

        }

        private void drawViewWeather()
        {
            weatherView = view.FindViewById<View_Weather>(Resource.Id.view_weather_fragment);
            List<float> temperature_data = new List<float>();
            List<string> time = new List<string>();
            List<string> bitmapList = new List<string>();
            List<List<float>> datas = new List<List<float>>();
            for (int i = 0; i < 16; i++)
            {
                int temp= (int)(city_forecast[i].temp - 273.15);
                temperature_data.Add(temp);
                time.Add(city_forecast[i].time.Substring(11, 5));
                bitmapList.Add(city_forecast[i].icon);
            }
            datas.Add(temperature_data);
            weatherView.setPaddingLeft(80);
            weatherView.setPaddingRight(80);
            weatherView.setIsSetYTitle(false);
            weatherView.setTitleXList(time);
            weatherView.setBitmapList(bitmapList);
            weatherView.setPointList(datas);
            weatherView.setNumberOfX(16);
        }

        private int getResourceId(string icon)
        {
            int id=0;
            if (icon.Equals("01d")) id = Resource.Drawable.Drawable_sunny_d;
            if (icon.Equals("01n")) id = Resource.Drawable.Drawable_sunny_n;
            if (icon.Equals("02d")) id = Resource.Drawable.Drawable_b_d;
            if (icon.Equals("02n")) id = Resource.Drawable.Drawable_b_n;
            if (icon.Equals("03d")) id = Resource.Drawable.Drawable_c_d;
            if (icon.Equals("03d")) id = Resource.Drawable.Drawable_c_n;
            if (icon.Equals("04n")) id = Resource.Drawable.Drawable_d_d;
            if (icon.Equals("04n")) id = Resource.Drawable.Drawable_d_n;
            if (icon.Equals("09d")) id = Resource.Drawable.Drawable_e_d;
            if (icon.Equals("09n")) id = Resource.Drawable.Drawable_e_n;
            if (icon.Equals("10d")) id = Resource.Drawable.Drawable_f_d;
            if (icon.Equals("10n")) id = Resource.Drawable.Drawable_f_n;
            if (icon.Equals("11d")) id = Resource.Drawable.Drawable_g_d;
            if (icon.Equals("11n")) id = Resource.Drawable.Drawable_g_n;
            if (icon.Equals("13d")) id = Resource.Drawable.Drawable_h_d;
            if (icon.Equals("13n")) id = Resource.Drawable.Drawable_h_n;
            if (icon.Equals("50d")) id = Resource.Drawable.Drawable_i_d;
            if (icon.Equals("50n")) id = Resource.Drawable.Drawable_i_n;
            return id;
        }

        private void setBackground(string icon)
        {
            LinearLayout lin = view.FindViewById<LinearLayout>(Resource.Id.lin_fragment_background);
            Console.WriteLine(name + "  "+icon);
            if (icon.Equals("01d") || icon.Equals("01n"))
            {
                Activity.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Activity.Window.SetStatusBarColor(Color.ParseColor("#8A8E9A"));
                lin.SetBackgroundResource(Resource.Drawable.Drawable_Sunny);
                return;
            }
            if (icon.Equals("09d") || icon.Equals("09n") || icon.Equals("10d") || icon.Equals("10n"))
            {
                Activity.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Activity.Window.SetStatusBarColor(Color.ParseColor("#00964F"));
                lin.SetBackgroundResource(Resource.Drawable.Drawable_Rainy);
                return;
            }
            if (icon.Equals("13d") || icon.Equals("13n"))
            {
                Activity.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Activity.Window.SetStatusBarColor(Color.ParseColor("#537D67"));
                lin.SetBackgroundResource(Resource.Drawable.Drawable_Snowy);
                return;
            }
            lin.SetBackgroundResource(Resource.Drawable.Drawable_Cool);
            Activity.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            Activity.Window.SetStatusBarColor(Color.ParseColor("#483F98"));
            return;
        }

        public static string Get(string url)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            try
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            finally
            {
                stream.Close();
            }
            return result;
        }
    }
}