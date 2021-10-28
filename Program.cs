using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab3Tema
{
    class Program
    {
        private static DriveService _service;
        private static string _token;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Initialize();
        }
         static void Initialize()
        {
            string[] scopes = new string[] {
                DriveService.Scope.Drive,
                DriveService.Scope.DriveFile
            };

            var clientId = "15731291236-95l9tk2mld93vfd00dkug7lf5tsecvun.apps.googleusercontent.com";
            var clientSecret = "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";
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                },
                scopes,
                Environment.UserName,
                CancellationToken.None,
                new FileDataStore("Daimto.GoogleDrive.Auth.Store3")
                ).Result;

            _service = new DriveService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });

            _token = credential.Token.AccessToken;

            Console.Write("Token: " + credential.Token.AccessToken);

            GetMyFiles();
        }
        static void GetMyFiles()
        {
            var request = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/drive/v3/files?q=%27root%27%20in%20parents%22");

            request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + _token);

            using (var response = request.GetResponse())
            {
                using (Stream data = response.GetResponseStream())

                using (var reader = new StreamReader(data))
                {
                    string text = reader.ReadToEnd();

                    var myData = JObject.Parse(text);

                    foreach (var file in myData["files"])
                    {
                        if (file["mimeType"].ToString() != "application/vnd.google-apps.folder")
                        {
                            Console.WriteLine("File name: " + file["name"]);
                        }
                    }
                }
            }
        }

    }
}