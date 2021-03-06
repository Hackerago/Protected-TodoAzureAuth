﻿/*
   Copyright 2018 PreEmptive Solutions, LLC
   Portions Copyright 2018 Xamarin Inc.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using Xamarin.Forms;

namespace TodoAzure
{
    public partial class LoginPage : ContentPage
    {
        private bool authenticated = false;
        private string lastAuthStatus;

        public string LastAuthStatus
        {
            get => lastAuthStatus;
            set
            {
                lastAuthStatus = value;
                OnPropertyChanged();
            }
        }

        public LoginPage()
        {
            InitializeComponent();
            LastAuthStatus = "Not yet attempted";
        }

        protected override async void OnAppearing()
        {
            if (App.IsDisabled)
            {
                IsVisible = false;
                var message = "The security of this device has been compromised. "
                  + "The app will exit.";
                await DisplayAlert("App deactivated", message, "Exit App");
                App.Exit(); // delegates to platform-specific exit logic
            }
            base.OnAppearing();
        }
        
        async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            try
            {
                if (App.Authenticator != null)
                {
                    authenticated = await App.Authenticator.AuthenticateAsync();
                }

                if (authenticated)
                {
                    LastAuthStatus = "Authentication successful";
                    Application.Current.MainPage = new TodoList(this);
                }
                else
                {
                    LastAuthStatus = "Did not authenticate";
                }
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException && ex.Message.Contains("Authentication was cancelled"))
                {
                    LastAuthStatus = "Authentication cancelled by the user";
                }
                else
                {
                    LastAuthStatus = "Authentication failed: " + ex.Message;
                }
            }
        }
    }
}

