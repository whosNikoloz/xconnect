using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using XConnectTest.Utilitys;
using static System.Windows.Forms.AxHost;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace XConnectTest
{
    public partial class Form1 : Form
    {
        private readonly HttpClient _client;
        public Form1()
        {
            InitializeComponent();
            _client = new HttpClient();
        }

        private string _accessToken = "";
        private readonly string _apiHostAdr = "localhost";
        private readonly string _apiHostPort = "6678";
        private readonly string _apiVersion = "v105";
        private string _operationId = "";
        private string _documentNr = "";
        private string _currencyCode = "981"; // Georgian Lari
        private decimal _value = 0;
        private string _reciptText = "";

        //OpenPos
        private string _Alias = "";
        private string _LicenseToken = "ASHBURN";


        private async void btnPay_Click(object sender, EventArgs e)
        {
            if (!await OpenPosAsync())
            {
                return;
            }
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                if (!await UnlockAsync())
                {
                    return;
                }
                if (!await GetEvent("ONCARD"))
                {
                    MessageBox.Show("Card Not Found");
                    return;
                }
                if (!await Authorize())
                {
                    return;
                }

                if (await GetEvent("ONTRNSTATUS"))
                {
                    MessageBox.Show("Payed");
                }
                else
                {
                    MessageBox.Show("NotPayed");
                }
                if(!await CloseDoc())
                {
                    return;
                }
                MessageBox.Show("Doc Closed");
            }
        }

        private async Task<bool> CloseDoc()
        {
            var url = $"http://{_apiHostAdr}:{_apiHostPort}/{_apiVersion}/executeposcmd";

            LoggingUtility.Log($"documentNr : {_documentNr} ; operations {_operationId}");

            var requestBody = new
            {
                header = new
                {
                    command = "CLOSEDOC"
                },
                @params = new
                {
                    documentNr = _documentNr ?? "null",
                    operations = new[] { _operationId }
                }
            };


            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Create HttpRequestMessage and set headers
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.SendAsync(requestMessage);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Parse the JSON content
                    var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);

                    // Check for errors in the response
                    if (jsonResponse != null && jsonResponse.ContainsKey("resultCode") && jsonResponse["resultCode"] == "INVALID_ARG")
                    {
                        Console.WriteLine("\nError in response!");
                        Console.WriteLine("Message: {0}", jsonResponse["resultMessage"]);
                        MessageBox.Show(jsonResponse["resultMessage"]);
                        return false; // Operation failed due to an invalid argument
                    }

                    response.EnsureSuccessStatusCode();
                    return true; // Operation was successful
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message: {0}", e.Message);
                return false; // Operation failed
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nUnexpected Exception Caught!");
                Console.WriteLine("Message: {0}", ex.Message);
                return false; // Operation failed due to an unexpected error
            }
        }
        private async Task<bool> Authorize()
        {
            var url = $"http://{_apiHostAdr}:{_apiHostPort}/{_apiVersion}/executeposcmd";

            // Convert amount from text input
            _value = Convert.ToInt32(txtAmount.Text);

            var random = new Random();
            _currencyCode = ComboCurrency.SelectedValue.ToString();



            // Generate two random integers
            int firstInt = random.Next(0, 1000); // Generate a random integer between 1000 and 9999
            int secondInt = random.Next(0, 1000); // Generate a random integer between 1000 and 9999

            _documentNr = $"{firstInt}{secondInt}";
            // Define the request body
            var requestBody = new
            {
                header = new
                {
                    command = "Authorize"
                },
                @params = new
                {
                    amount = _value,
                    currencyCode = _currencyCode,
                    documentNr = $"{firstInt}{secondInt}" // Combine the random integers into documentNr
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Create HttpRequestMessage and set headers
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.SendAsync(requestMessage);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Parse the JSON content
                    var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);

                    // Check for errors in the response
                    if (jsonResponse != null && jsonResponse.ContainsKey("resultCode") && jsonResponse["resultCode"] == "INVALID_ARG")
                    {
                        Console.WriteLine("\nError in response!");
                        Console.WriteLine("Message: {0}", jsonResponse["resultMessage"]);
                        MessageBox.Show(jsonResponse["resultMessage"]);
                        return false; // Operation failed due to an invalid argument
                    }

                    response.EnsureSuccessStatusCode();
                    return true; // Operation was successful
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message: {0}", e.Message);
                return false; // Operation failed
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nUnexpected Exception Caught!");
                Console.WriteLine("Message: {0}", ex.Message);
                return false; // Operation failed due to an unexpected error
            }
        }

        private async Task<bool> GetEvent(string eventType,bool refund = false)
        {
            var url = $"http://{_apiHostAdr}:{_apiHostPort}/{_apiVersion}/getEvent";

            try
            {
                while (true)
                {
                    // Create a new HttpRequestMessage for each request
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                    requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Send the GET request
                    var response = await _client.SendAsync(requestMessage);
                    response.EnsureSuccessStatusCode();

                    // Read the response content
                    var responseBody = await response.Content.ReadAsStringAsync();

                    // Parse the JSON response
                    using (var jsonDoc = JsonDocument.Parse(responseBody))
                    {
                        var root = jsonDoc.RootElement;

                        // Check if the response contains the desired event
                        if (root.TryGetProperty("eventName", out var eventName) &&
                            eventName.GetString() == eventType)
                        {
                            if (eventType == "ONTRNSTATUS")
                            {
                                if (root.TryGetProperty("properties", out var properties) &&
                                    properties.TryGetProperty("operationId", out var operationId))
                                {
                                    _operationId = operationId.GetString();
                                    txtTrID.Text = _operationId;
                                }
                                if (refund)
                                {
                                    _documentNr = null;
                                    return true;
                                }
                                if (properties.TryGetProperty("state", out var state))
                                {
                                    // Return true if status is "approved"
                                    if (state.GetString() == "Approved")
                                    {
                                        return true; // Status is approved
                                    }
                                    else
                                    {
                                        return false; // Status is not approved
                                    }
                                }
                            }else if (eventType == "ONPRINT")
                            {
                                if (root.TryGetProperty("properties", out var properties))
                                {
                                    if (properties.TryGetProperty("receiptText", out var receiptText))
                                    {
                                        _reciptText = receiptText.GetString();
                                        return true;
                                    }
                                }

                                return false;
                            }
                            else
                            {
                                // For other event types, you might have different logic
                                return true; // Event found
                            }
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return false; // Operation failed
            }
        }


        private async Task<bool> UnlockAsync()
        {
            var url = $"http://{_apiHostAdr}:{_apiHostPort}/{_apiVersion}/executeposcmd";

            _currencyCode = ComboCurrency.SelectedValue.ToString();

            _value = Convert.ToDecimal(txtAmount.Text);

            // Define the request body
            var requestBody = new
            {
                header = new
                {
                    command = "UNLOCKDEVICE"
                },
                @params = new
                {
                    posOperation = "AUTHORIZE", // sale
                    amount = _value,
                    cashBackAmount = 0,
                    currencyCode = _currencyCode,
                    idleText = "Idle Text",
                    language = "EN",
                    ecrVersion = "FINA-RESTcon-V22"
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Create HttpRequestMessage and set headers
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.SendAsync(requestMessage);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Parse the JSON content
                    var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);

                    // Check for errors in the response
                    if (jsonResponse != null && jsonResponse.ContainsKey("resultCode") && jsonResponse["resultCode"] == "UNAUTHORIZED")
                    {
                        Console.WriteLine("\nError in response!");
                        Console.WriteLine("Message: UNAUTHORIZED");
                        MessageBox.Show("UNAUTHORIZED");
                        return false; // Operation failed due to an invalid argument
                    }

                    response.EnsureSuccessStatusCode();
                    return true; // Operation was successful
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message: {0}", e.Message);
                return false; // Operation failed
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nUnexpected Exception Caught!");
                Console.WriteLine("Message: {0}", ex.Message);
                return false; // Operation failed due to an unexpected error
            }
        }


        private async void btnRefund_Click(object sender, EventArgs e)
        {
            if (!await OpenPosAsync())
            {
                return;
            }
            if (!await UnlockRefund())
            {
                return;
            }
            if (!await GetEvent("ONCARD"))
            {
                MessageBox.Show("Card Not Found");
                return;
            }
            if(!await Refund())
            {
                MessageBox.Show("Not Refunded");
                return;
            }
            if (!await GetEvent("ONTRNSTATUS",true))
            {
                MessageBox.Show("Not Refunded");
            }
            else
            {
                MessageBox.Show("Refunded");
            }
            if(!await CloseDoc())
            {
                return;
            }
            MessageBox.Show("Doc Closed");
        }

        private async Task<bool> UnlockRefund()
        {
            var url = $"http://{_apiHostAdr}:{_apiHostPort}/{_apiVersion}/executeposcmd";

            _currencyCode = ComboCurrency.SelectedValue.ToString();

            _value = Convert.ToDecimal(txtAmount.Text);

            // Define the request body
            var requestBody = new
            {
                header = new
                {
                    command = "UNLOCKDEVICE"
                },
                @params = new
                {
                    posOperation = "CREDIT", 
                    amount = _value,
                    cashBackAmount = 0,
                    currencyCode = _currencyCode,
                    idleText = "Idle Text",
                    language = "GE",
                    ecrVersion = "FINA-RESTcon-V22"
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Create HttpRequestMessage and set headers
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.SendAsync(requestMessage);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Parse the JSON content
                    var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);

                    // Check for errors in the response
                    if (jsonResponse != null && jsonResponse.ContainsKey("resultCode") && jsonResponse["resultCode"] == "INVALID_ARG")
                    {
                        Console.WriteLine("\nError in response!");
                        Console.WriteLine("Message: {0}", jsonResponse["resultMessage"]);
                        MessageBox.Show(jsonResponse["resultMessage"]);
                        return false; // Operation failed due to an invalid argument
                    }

                    response.EnsureSuccessStatusCode();
                    return true; // Operation was successful
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message: {0}", e.Message);
                return false; // Operation failed
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nUnexpected Exception Caught!");
                Console.WriteLine("Message: {0}", ex.Message);
                return false; // Operation failed due to an unexpected error
            }
        }
        private async Task<bool> Refund()
        {
            var url = $"http://{_apiHostAdr}:{_apiHostPort}/{_apiVersion}/executeposcmd";

            _documentNr = null;
            _currencyCode = ComboCurrency.SelectedValue.ToString();

            var requestBody = new
            {
                header = new
                {
                    command = "Refund"
                },
                @params = new
                {
                    amount = _value,
                    currencyCode = _currencyCode,
                    documentNr = _documentNr ?? "null",
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Create HttpRequestMessage and set headers
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.SendAsync(requestMessage);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Parse the JSON content
                    var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);

                    // Check for errors in the response
                    if (jsonResponse != null && jsonResponse.ContainsKey("resultCode") && jsonResponse["resultCode"] == "INVALID_ARG")
                    {
                        Console.WriteLine("\nError in response!");
                        Console.WriteLine("Message: {0}", jsonResponse["resultMessage"]);
                        MessageBox.Show(jsonResponse["resultMessage"]);
                        return false; // Operation failed due to an invalid argument
                    }

                    response.EnsureSuccessStatusCode();
                    return true; // Operation was successful
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message: {0}", e.Message);
                return false; // Operation failed
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nUnexpected Exception Caught!");
                Console.WriteLine("Message: {0}", ex.Message);
                return false; // Operation failed due to an unexpected error
            }
        }   

        private async Task<bool> OpenPosAsync()
        {
            _Alias = txtAlias.Text;

            var url = $"http://{_apiHostAdr}:{_apiHostPort}/{_apiVersion}/openpos";

            var requestBody = new
            {
                Username = "none",
                Password = "none",
                Alias = _Alias,
                LicenseToken = _LicenseToken,
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Create HttpRequestMessage and set headers
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.SendAsync(requestMessage);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Parse the JSON content
                    var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);

                    // Check for errors in the response
                    if (jsonResponse != null && jsonResponse.ContainsKey("resultCode") && jsonResponse["resultCode"] == "INVALID_ARG")
                    {
                        Console.WriteLine("\nError in response!");
                        Console.WriteLine("Message: {0}", jsonResponse["resultMessage"]);
                        MessageBox.Show(jsonResponse["resultMessage"]);
                        return false; // Operation failed due to an invalid argument
                    }

                    // Save the access token if present in the response
                    if (jsonResponse != null && jsonResponse.ContainsKey("accessToken"))
                    {
                        _accessToken = jsonResponse["accessToken"].ToString();
                    }

                    response.EnsureSuccessStatusCode();
                    return true; // Operation was successful
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message: {0}", e.Message);
                return false; // Operation failed
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nUnexpected Exception Caught!");
                Console.WriteLine("Message: {0}", ex.Message);
                return false; // Operation failed due to an unexpected error
            }
        }
        private async void btnVoid_Click(object sender, EventArgs e)
        {
            if (!await OpenPosAsync())
            {
                return;
            }
            if (!await Void())
            {
                return;
            }
            MessageBox.Show("Voided");
        }

        private async Task<bool> Void()
        {
            var url = $"http://{_apiHostAdr}:{_apiHostPort}/{_apiVersion}/executeposcmd";
            _operationId = txtTrID.Text;

            // Define the request body
            var requestBody = new
            {
                header = new
                {
                    command = "Void"
                },
                @params = new
                {
                    operationId = _operationId,
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Create HttpRequestMessage and set headers
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.SendAsync(requestMessage);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Parse the JSON content
                    var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);

                    // Check for errors in the response
                    if (jsonResponse != null && jsonResponse.ContainsKey("resultCode") && jsonResponse["resultCode"] == "INVALID_ARG")
                    {
                        Console.WriteLine("\nError in response!");
                        Console.WriteLine("Message: {0}", jsonResponse["resultMessage"]);
                        MessageBox.Show(jsonResponse["resultMessage"]);
                        return false; // Operation failed due to an invalid argument
                    }

                    response.EnsureSuccessStatusCode();
                    return true; // Operation was successful
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message: {0}", e.Message);
                return false; // Operation failed
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nUnexpected Exception Caught!");
                Console.WriteLine("Message: {0}", ex.Message);
                return false; // Operation failed due to an unexpected error
            }
        }

        private async void btnCloseDay_Click(object sender, EventArgs e)
        {
            if(!await OpenPosAsync())
            {
                return;
            }
            if(!await CloseDay())
            {
                MessageBox.Show("Day Not Closed");
            }
            if(!await GetEvent("ONPRINT"))
            {
                MessageBox.Show("Day Not Closed Printed");
            }
            MessageBox.Show("Day Closed");
        }

        private async Task<bool> CloseDay()
        {
            var url = $"http://{_apiHostAdr}:{_apiHostPort}/{_apiVersion}/executeposcmd";

            // Define the request body
            var requestBody = new
            {
                header = new
                {
                    command = "CLOSEDAY"
                },
                @params = new
                {
                    operatorId = "M23",
                    operatorName = "Nika",
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Create HttpRequestMessage and set headers
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.SendAsync(requestMessage);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Parse the JSON content
                    var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);

                    // Check for errors in the response
                    if (jsonResponse != null && jsonResponse.ContainsKey("resultCode") && jsonResponse["resultCode"] == "INVALID_ARG")
                    {
                        Console.WriteLine("\nError in response!");
                        Console.WriteLine("Message: {0}", jsonResponse["resultMessage"]);
                        MessageBox.Show(jsonResponse["resultMessage"]);
                        return false; // Operation failed due to an invalid argument
                    }

                    response.EnsureSuccessStatusCode();
                    return true; // Operation was successful
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message: {0}", e.Message);
                return false; // Operation failed
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nUnexpected Exception Caught!");
                Console.WriteLine("Message: {0}", ex.Message);
                return false; // Operation failed due to an unexpected error
            }
        }

        private async void btnPrintTotal_Click(object sender, EventArgs e)
        {
            if(!await OpenPosAsync())
            {
                MessageBox.Show("POS Not Opened");
                return;
            }
            if(await PrintTotal())
            {
                MessageBox.Show("Total Printed");
                var eventType = "ONPRINT";
                var eventFound = await GetEvent(eventType);
                if (eventFound)
                {
                    MessageBox.Show(_reciptText);
                }
                else
                {
                    MessageBox.Show("No Recipt");
                }
            }
            else
            {
                MessageBox.Show("Total not Printed");
            }
        }

        private async Task<bool> PrintTotal()
        {
            var url = $"http://{_apiHostAdr}:{_apiHostPort}/{_apiVersion}/executeposcmd";

            // Define the request body
            var requestBody = new
            {
                header = new
                {
                    command = "printtotals"
                },
                @params = new
                {
                    operatorId = "M23",
                    operatorName = "Nika",
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Create HttpRequestMessage and set headers
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.SendAsync(requestMessage);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Parse the JSON content
                    var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);

                    // Check for errors in the response
                    if (jsonResponse != null && jsonResponse.ContainsKey("resultCode") && jsonResponse["resultCode"] == "INVALID_ARG")
                    {
                        Console.WriteLine("\nError in response!");
                        Console.WriteLine("Message: {0}", jsonResponse["resultMessage"]);
                        MessageBox.Show(jsonResponse["resultMessage"]);
                        return false; // Operation failed due to an invalid argument
                    }

                    response.EnsureSuccessStatusCode();
                    return true; // Operation was successful
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message: {0}", e.Message);
                return false; // Operation failed
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nUnexpected Exception Caught!");
                Console.WriteLine("Message: {0}", ex.Message);
                return false; // Operation failed due to an unexpected error
            }
        }

        

        private void txtAlias_TextChanged(object sender, EventArgs e)
        {

        }

        

        private void txtAmount_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTrID_TextChanged(object sender, EventArgs e)
        {

        }

        private void ComboCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ComboCurrency.DataSource = new BindingSource 
            {
                DataSource = new Dictionary<string, string>() { {"GEL", "981" }, { "EUR", "978" }, { "ERTGULI", "894" }, { "GEL (PLUS Mode)", "981:Georgian Lari" }, { "PLUS Point (PLUS Mode)", "981:Plus Points" }, { "SCOOL Point (PLUS Mode)", "981:Scool Points" } }
            };
            ComboCurrency.DisplayMember = "Key";
            ComboCurrency.ValueMember = "Value";
        }
    }
}
