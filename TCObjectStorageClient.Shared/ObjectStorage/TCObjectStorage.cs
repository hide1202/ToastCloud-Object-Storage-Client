﻿using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TCObjectStorageClient.Models;

namespace TCObjectStorageClient.ObjectStorage
{
    public class TCObjectStorage : IObjectStorage
    {
        public async Task<string> PostToken(string tenentName, string userName, string password)
        {
            var url = "https://api-compute.cloud.toast.com/identity/v2.0/tokens";
            HttpClient client = new HttpClient();

            var createTokenReqeust = new CreateTokenRequest
            {
                Auth = new CreateTokenRequest.Authentication
                {
                    TenantName = tenentName,
                    PasswordCredentials = new CreateTokenRequest.PasswordCredentials
                    {
                        UserName = userName,
                        Password = password
                    }
                }
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(createTokenReqeust));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            var response = await client.PostAsync(url, requestContent);
            var content = await response.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<CreateTokenResponse>(content);
            return responseJson?.Access?.Token?.Id;
        }

        public async Task<bool> UploadFile(string token, string account, string containerName, string objectName, byte[] body)
        {
            HttpClient client = new HttpClient();
            var url = $"https://api-storage.cloud.toast.com/v1/{account}/{containerName}/{objectName}";
            client.DefaultRequestHeaders.Add("X-Auth-Token", token);
            var response = await client.PutAsync(url, new ByteArrayContent(body));
            return response.StatusCode == HttpStatusCode.Created;
        }

        public async Task<bool> ContainsContainer(string token, string account, string containerName)
        {
            HttpClient client = new HttpClient();
            var url = $"https://api-storage.cloud.toast.com/v1/{account}/{containerName}";
            client.DefaultRequestHeaders.Add("X-Auth-Token", token);
            var response = await client.GetAsync(url);
            return response.StatusCode == HttpStatusCode.OK;
        }
    }
}
