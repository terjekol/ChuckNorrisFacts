﻿using System;
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
        private readonly Dictionary<string, List<string>> _userFavorites;
        private readonly RestClient _client = new RestClient("https://api.chucknorris.io");
        private string _userName;
        private List<string> Favorites => _userFavorites[_userName];

        public FactsView()
        {
            InitializeComponent();

            var categoryList = new List<string> { "Random" };
            categoryList.AddRange(GetCategories());
            CategoryPicker.ItemsSource = categoryList;
            CategoryPicker.SelectedIndex = 0;
            _userFavorites = new Dictionary<string, List<string>>();
        }

        private void GetFavoriteClicked(object sender, EventArgs e)
        {
            FactLabel.Text = Favorites.Count == 0
                ? "You have no favorites yet."
                : Favorites[new Random().Next(0, Favorites.Count)];
        }

        private void GetFactClicked(object sender, EventArgs e)
        {
            var isRandom = CategoryPicker.SelectedIndex == 0;
            FactLabel.Text = GetFact(isRandom ? null : CategoryPicker.SelectedItem.ToString());
        }

        private void AddFavoriteClicked(object sender, EventArgs e)
        {
            Favorites.Add(FactLabel.Text);
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

        public void HandleLoginChanged(string userName)
        {
            FactLabel.Text = "";
            bool isLoggedIn = userName != null;
            AddFavoriteButton.IsEnabled = GetFavoriteButton.IsEnabled = isLoggedIn;
            _userName = isLoggedIn ? userName : null;
            if (userName != null && !_userFavorites.ContainsKey(userName))
                _userFavorites.Add(userName, new List<string>());
        }
    }
}