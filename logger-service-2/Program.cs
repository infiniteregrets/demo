using System;
using System.IO;
using System.Net;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        string url = "http://*:8080/";
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add(url);
        listener.Start();
        Console.WriteLine("Listening on {0}", url);

        while (true)
        {
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;

            if (request.HttpMethod == "GET")

            {
                if (request.Headers["service-log"] == "true")
                {
                    string logMessage = string.Format(
                        "{0} - {1} {2}",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        request.HttpMethod,
                        request.RawUrl);

                    LogToFile("output.txt", logMessage);
                    string responseString = "service-logger logged the request";
                    SendResponse(context.Response, responseString);
                }
                else if (request.Headers["service-log"] == "false")
                {
                    string responseString = "service-logger did not log the request";
                    SendResponse(context.Response, responseString);
                }
                else
                {
                    string? endpoint = request.Url?.AbsolutePath;
                    if (endpoint == "/api/v1")
                    {
                        string responseString = "API v1 endpoint";
                        SendResponse(context.Response, responseString);
                    }
                    else if (endpoint == "/api/v2")
                    {
                        string responseString = "API v2 endpoint";
                        SendResponse(context.Response, responseString);
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        context.Response.Close();
                    }
                }

                Console.WriteLine("Request: {0} {1}", request.HttpMethod, request.RawUrl, request.Headers["service-log"]);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                context.Response.Close();
            }
        }
    }

    static void SendResponse(HttpListenerResponse response, string responseString)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        Stream output = response.OutputStream;
        // check if the client closed the connection
        Console.WriteLine("Client disconnected: {0}", response.OutputStream.CanWrite);

        output.Write(buffer, 0, buffer.Length);
        output.Close();
    }

    static void LogToFile(string filePath, string logMessage)
    {
        using (StreamWriter writer = File.AppendText(filePath))
        {
            writer.WriteLine(logMessage);
        }
    }
}
