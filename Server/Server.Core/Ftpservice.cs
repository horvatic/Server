using System;
using System.Text;

namespace Server.Core
{
    public class Ftpservice : IHttpServiceProcessor
    {
        private string _packetBound;
        private string _directory;
        private string _file;
        private bool _removedContentType = false;

        public bool CanProcessRequest(string request, ServerProperties serverProperties)
        {
            var requestItem = CleanRequest(request);
            return requestItem == "upload";
        }

        public IHttpResponse ProcessRequest(string request, IHttpResponse httpResponse,
            ServerProperties serverProperties)
        {
            return request.Contains("GET /") && request.IndexOf("GET /", StringComparison.Ordinal) == 0
                ? GetRequest(request, httpResponse)
                : PostRequest(request, httpResponse, serverProperties);
        }

        private string GetPacketBound(string request)
        {
            var packetSplit = request.Substring(request.IndexOf("boundary=----"
                , StringComparison.Ordinal) + 13);
            packetSplit = packetSplit.Remove(packetSplit.IndexOf("\r\n"
                , StringComparison.Ordinal));
            return packetSplit;
        }

        private void SetDirectoryAndFile(string request)
        {
            if (request.Contains("name=\"saveLocation\"\r\n\r\n") && _directory == null)
            {
                _directory = CleanPost(request, "name=\"saveLocation\"\r\n\r\n", "\r\n");
                if (!_directory.EndsWith("/"))
                    _directory += "/";
            }
            if (request.Contains("filename=\"") && _file == null)
            {
                _file = CleanPost(request, "filename=\"", "\"\r\n");
            }
        }

        private string RemoveHeaderAndSetPacketBound(string request)
        {
            _packetBound = GetPacketBound(request);
            return request.Substring(request.IndexOf("\r\n\r\n"
                , StringComparison.Ordinal) + 4);
        }


        private IHttpResponse ProcessRequestWithPath(string data, IHttpResponse httpResponse,
            ServerProperties serverProperties)
        {
            SetDirectoryAndFile(data);
            if (_directory != null && _file != null
                && (!serverProperties.DirReader.Exists(_directory)
                || serverProperties.FileReader.Exists(_directory + _file))
                )
            {
                httpResponse.HttpStatusCode = "409 Conflict";
                httpResponse.Body = PostWebPage("Could not make item");
                return httpResponse;
            }
            if(_directory != null && _file != null
                && data.Contains("Content-Type: "))
                return ProcessData(data, httpResponse,
                        serverProperties);
            return httpResponse;
        }
        

        private string RemoveContentDisposition(string data)
        {
            var processedData = data;
            processedData = processedData
                .Substring(processedData.IndexOf("Content-Disposition: form-data;"
                + @" name=""fileToUpload"""
                    , StringComparison.Ordinal));
            processedData = processedData.Substring(processedData.IndexOf("\r\n"
                    , StringComparison.Ordinal) + 2 );
            return processedData;
        }

        private string RemoveContentType(string data)
        {
            var processedData = data;
            _removedContentType = true;
            processedData = processedData.Substring(data.IndexOf("Content-Type: "
                , StringComparison.Ordinal));
            processedData = processedData.Substring(data.IndexOf("\r\n\r\n"
                , StringComparison.Ordinal) + 4);
            return processedData;
        }

        private IHttpResponse ProcessData(string data, IHttpResponse httpResponse,
            ServerProperties serverProperties)
        {
            var processedData = data;
            processedData = RemoveContentDisposition(processedData);
            processedData = RemoveContentType(processedData);
            
            return SaveFile(processedData, httpResponse,
                serverProperties);
        }

        private IHttpResponse SaveFile(string data, IHttpResponse httpResponse,
            ServerProperties serverProperties)
        {
            if (_file == "" || _directory == "")
            {
                httpResponse.HttpStatusCode = "409 Conflict";
                httpResponse.Body = PostWebPage("Could not make item");
                return httpResponse;
            }
            var sendData = data;
            if (sendData.EndsWith("\r\n------" + _packetBound + "--\r\n"))
                sendData = sendData.Replace("\r\n------" + _packetBound + "--\r\n", "");
            serverProperties.Io.PrintToFile(sendData,_directory + _file);
            httpResponse.HttpStatusCode = "201 Created";
            httpResponse.Body = PostWebPage("Item Made");
            return httpResponse;
        }

        private IHttpResponse PostRequest(string request, IHttpResponse httpResponse, ServerProperties serverProperties)
        {
            var data = request.Contains("POST /upload HTTP/1.1\r\n") 
                && _directory == null && _file == null
                ? RemoveHeaderAndSetPacketBound(request)
                : request;
            if (data == "")
                return httpResponse;
            else if((data.Contains(@"Content-Disposition: form-data; name=""saveLocation""") 
                || data.Contains(@"Content-Disposition: form-data; name=""fileToUpload"""))
                && ( _directory == null || _file == null))
                return ProcessRequestWithPath(data, httpResponse, serverProperties);
            else
                return SaveFile(data, httpResponse,
                serverProperties);

        }


        private string CleanPost(string request, string head, string tail)
        {
            var cleanInput = request.Substring(request.IndexOf(head
                , StringComparison.Ordinal) + head.Length);
            cleanInput = cleanInput.Remove(cleanInput.IndexOf(tail, StringComparison.Ordinal));

            return cleanInput;
        }


        private IHttpResponse GetRequest(string request, IHttpResponse httpResponse)
        {
            var uploadPage = new StringBuilder();
            uploadPage.Append(@"<form action=""upload"" method=""post"" enctype=""multipart/form-data"">");
            uploadPage.Append(@"Select Save Location<br>");
            uploadPage.Append(@"<input type=""text"" name=""saveLocation""><br>");
            uploadPage.Append(@"Select File To Upload<br>");
            uploadPage.Append(@"<input type=""file"" name=""fileToUpload"" id=""fileToUpload""><br>");
            uploadPage.Append(@"<input type=""submit"" value=""Submit"">");
            uploadPage.Append(@"</form>");

            httpResponse.HttpStatusCode = "200 OK";
            httpResponse.CacheControl = "no-cache";
            httpResponse.ContentType = "text/html";
            httpResponse.Body = HtmlHeader() + UploadPage() + HtmlTail();

            return httpResponse;
        }

        private string UploadPage()
        {
            var uploadPage = new StringBuilder();
            uploadPage.Append(@"<form action=""upload"" method=""post"" enctype=""multipart/form-data"">");
            uploadPage.Append(@"Select Save Location<br>");
            uploadPage.Append(@"<input type=""text"" name=""saveLocation""><br>");
            uploadPage.Append(@"Select File To Upload<br>");
            uploadPage.Append(@"<input type=""file"" name=""fileToUpload"" id=""fileToUpload""><br>");
            uploadPage.Append(@"<input type=""submit"" value=""Submit"">");
            uploadPage.Append(@"</form>");

            return uploadPage.ToString();
        }

        private string CleanRequest(string request)
        {
            var parseVaulue = request.Contains("GET") ? "GET" : "POST";
            var offsets = request.Contains("GET") ? 5 : 6;
            if (request.Contains("HTTP/1.1"))
                return request.Substring(request.IndexOf(parseVaulue + " /", StringComparison.Ordinal) + offsets,
                    request.IndexOf(" HTTP/1.1", StringComparison.Ordinal) - offsets)
                    .Replace("%20", " ");
            return request.Substring(request.IndexOf(parseVaulue + " /", StringComparison.Ordinal) + offsets,
                request.IndexOf(" HTTP/1.0", StringComparison.Ordinal) - offsets)
                .Replace("%20", " ");
        }

        private string HtmlHeader()
        {
            var header = new StringBuilder();
            header.Append(@"<!DOCTYPE html>");
            header.Append(@"<html>");
            header.Append(@"<head><title>Vatic File Upload</title></head>");
            header.Append(@"<body>");
            return header.ToString();
        }

        private string HtmlTail()
        {
            var tail = new StringBuilder();
            tail.Append(@"</body>");
            tail.Append(@"</html>");
            return tail.ToString();
        }

        private string PostWebPage(string message)
        {
            return HtmlHeader() + message + "<br>" + UploadPage() + HtmlTail();

        }
    }
}