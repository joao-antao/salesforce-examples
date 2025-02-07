using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Salesforce;

// Example of salesforce simple integration using REST APIs

var clientId = "YOUR_CONSUMER_KEY"; 
var clientSecret = "YOUR_CONSUMER_SECRET";
var username = "YOUR_SALESFORCE_USERNAME";
var password = "YOUR_SALESFORCE_PASSWORD" + "YOUR_SECURITY_TOKEN";
var tokenUrl = "https://login.salesforce.com/services/oauth2/token";
var recordId = "YOUR_RECORD_ID";
var objectName = "YOUR_OBJECT_NAME";

string instanceUrl;
string accessToken;

// Authenticate using OAuth2 protocol with grant type: client credentials
using (var client = new HttpClient())
{
    var content = new FormUrlEncodedContent([
        new KeyValuePair<string, string>("grant_type", "client_credentials"),
        new KeyValuePair<string, string>("client_id", clientId),
        new KeyValuePair<string, string>("client_secret", clientSecret),
    ]);

    var response = await client.PostAsync(tokenUrl, content);
    var responseString = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine($"Error: {responseString}");
        return;
    }
    
    var salesforceAccessToken = await response.Content.ReadFromJsonAsync<SalesforceAccessToken>();
    if (salesforceAccessToken == null)
    {
        Console.WriteLine($"Salesforce access token is null");
        return;
    }

    accessToken = salesforceAccessToken.AccessToken;
    instanceUrl = salesforceAccessToken.InstanceUrl;

    Console.WriteLine("Authentication successful!");
    Console.WriteLine($"Access Token: {salesforceAccessToken.AccessToken}");
}

// Get objects
using (var client = new HttpClient())
{
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
    var apiUrl = $"{instanceUrl}/services/data/v58.0/sobjects/account/";

    var response = await client.GetAsync(apiUrl);
    var responseString = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine($"Error getting account: {responseString}");
        return;
    }

    Console.WriteLine($"Account fetch successfully: {responseString}");
}

// Create object
using (var client = new HttpClient())
{
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
    var apiUrl = $"{instanceUrl}/services/data/v58.0/sobjects/account/";

    var accountData = new
    {
        Name = "Test Kramerica Industries",
    };

    var json = JsonSerializer.Serialize(accountData, JsonSerializerOptions.Default);
    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var response = await client.PostAsync(apiUrl, content);
    var responseString = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine($"Error creating Account: {responseString}");
        return;
    }

    Console.WriteLine($"Account created successfully: {responseString}");
}

Console.ReadKey();

// Update record
using (var client = new HttpClient())
{
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
    var apiUrl = $"{instanceUrl}/services/data/v58.0/sobjects/Account/{recordId}";

    var updateData = new { Industry = "Updated Industry" };
    var json = JsonSerializer.Serialize(updateData, JsonSerializerOptions.Default);
    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var request = new HttpRequestMessage(new HttpMethod("PATCH"), apiUrl) { Content = content };
    var response = await client.SendAsync(request);
    var responseString = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine($"Error updating account: {responseString}");
        return;
    }

    Console.WriteLine("Account updated successfully!");
}

Console.ReadKey();

// Read records
using (var client = new HttpClient())
{
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
    var apiUrl = $"{instanceUrl}/services/data/v58.0/query/?q=SELECT+Id,Name+FROM+{objectName}";

    var response = await client.GetAsync(apiUrl);
    var responseString = await response.Content.ReadAsStringAsync();
    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine($"Error retrieving records: {responseString}");
        return;
    }
    
    Console.WriteLine($"Records retrieved successfully: {responseString}");
}

Console.ReadKey();
