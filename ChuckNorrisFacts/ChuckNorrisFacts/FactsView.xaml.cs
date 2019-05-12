using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChuckNorrisFacts
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FactsView : ContentView
    {
        private readonly List<string> _favorites = new List<string>();
        private readonly RestClient _client = new RestClient("https://api.chucknorris.io");

        public FactsView()
        {
            InitializeComponent();

            var categoryList = new List<string> { "Random" };
            categoryList.AddRange(GetCategories());
            CategoryPicker.ItemsSource = categoryList;
            CategoryPicker.SelectedIndex = 0;
        }

        private void GetFavoriteClicked(object sender, EventArgs e)
        {
            FactLabel.Text = _favorites.Count == 0
                ? "You have no favorites yet."
                : _favorites[new Random().Next(0, _favorites.Count)];
        }

        private void GetFactClicked(object sender, EventArgs e)
        {
            var isRandom = CategoryPicker.SelectedIndex == 0;
            FactLabel.Text = GetFact(isRandom ? null : CategoryPicker.SelectedItem.ToString());
        }

        private void AddFavoriteClicked(object sender, EventArgs e)
        {
            _favorites.Add(FactLabel.Text);
        }

        public string GetFact(string category = null)
        {
            var url = "/jokes/random";
            if (category != null) url += "?category=" + category;
            var fact = Get<Fact>(url);
            return fact?.value;
        }

        public string[] GetCategories()
        {
            return Get<string[]>("/jokes/categories");
        }

        private T Get<T>(string url)
        {
            var request = new RestRequest(url, Method.GET);
            var response = _client.Execute(request);
            return JsonConvert.DeserializeObject<T>(response.Content);
        }
    }
}