using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Astove.BlurAdmin.WebApi.Controllers.Services
{
    public class UploadController : ApiController
    {

        /// <summary>
        /// Accept form post with files
        /// </summary>
        /// <returns></returns>
        [ActionName("UploadFile")]
        public IHttpActionResult PostUploadFile()
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
                return BadRequest("not form-data");

            var provider = Request.Content.ReadAsMultipartAsync<InMemoryMultipartFormDataStreamProvider>(new InMemoryMultipartFormDataStreamProvider()).ContinueWith(t => t.Result);

            //access form data
            Dictionary<string, string> formData = provider.Result.FormData;

            //access files
            IList<HttpContent> fileContentList = provider.Result.Files;

            var files = provider.Result.GetFiles();

            // formulate the response
            if (files.Any())
                return Ok(string.Join("; ", 
                    (from f in files 
                     select string.Format("uploaded: {0} ({1} bytes = '{2}')", f.FileName, f.Size, f.ContentType)).ToArray()));
            else
                return BadRequest("No files attached");
        }

       

        public class InMemoryMultipartFormDataStreamProvider : MultipartStreamProvider
        {
            private Dictionary<string, string> _formData = new Dictionary<string, string>();
            private List<HttpContent> _fileContents = new List<HttpContent>();

            // Set of indexes of which HttpContents we designate as form data
            private Collection<bool> _isFormData = new Collection<bool>();

            /// <summary>
            /// Gets a <see cref="NameValueCollection"/> of form data passed as part of the multipart form data.
            /// </summary>
            public Dictionary<string, string> FormData
            {
                get { return _formData; }
            }

            /// <summary>
            /// Gets list of <see cref="HttpContent"/>s which contain uploaded files as in-memory representation.
            /// </summary>
            public List<HttpContent> Files
            {
                get { return _fileContents; }
            }

            /// <summary>
            /// Convert list of HttpContent items to FileData class task
            /// </summary>
            /// <returns></returns>
            public FileData[] GetFiles()
            {
                return Files.Select(f => FileData.ReadFile(f)).ToArray();
            }

            public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
            {
                // For form data, Content-Disposition header is a requirement
                ContentDispositionHeaderValue contentDisposition = headers.ContentDisposition;
                if (contentDisposition != null)
                {
                    // We will post process this as form data
                    _isFormData.Add(String.IsNullOrEmpty(contentDisposition.FileName));

                    return new MemoryStream();
                }

                // If no Content-Disposition header was present.
                throw new InvalidOperationException(string.Format("Did not find required '{0}' header field in MIME multipart body part..", "Content-Disposition"));
            }

            /// <summary>
            /// Read the non-file contents as form data.
            /// </summary>
            /// <returns></returns>
            public override Task ExecutePostProcessingAsync()
            {
                return Task.Factory.StartNew(new Action(() => {
                    // Find instances of non-file HttpContents and read them asynchronously
                    // to get the string content and then add that as form data
                    for (int index = 0; index < Contents.Count; index++)
                    {
                        if (_isFormData[index])
                        {
                            HttpContent formContent = Contents[index];
                            // Extract name from Content-Disposition header. We know from earlier that the header is present.
                            ContentDispositionHeaderValue contentDisposition = formContent.Headers.ContentDisposition;
                            string formFieldName = UnquoteToken(contentDisposition.Name) ?? String.Empty;

                            // Read the contents as string data and add to form data
                            string formFieldValue = formContent.ReadAsStringAsync().ContinueWith(t => t.Result).Result;
                            FormData.Add(formFieldName, formFieldValue);
                        }
                        else
                        {
                            _fileContents.Add(Contents[index]);
                        }
                    }
                }));                
            }

            /// <summary>
            /// Remove bounding quotes on a token if present
            /// </summary>
            /// <param name="token">Token to unquote.</param>
            /// <returns>Unquoted token.</returns>
            private static string UnquoteToken(string token)
            {
                if (String.IsNullOrWhiteSpace(token))
                {
                    return token;
                }

                if (token.StartsWith("\"", StringComparison.Ordinal) && token.EndsWith("\"", StringComparison.Ordinal) && token.Length > 1)
                {
                    return token.Substring(1, token.Length - 2);
                }

                return token;
            }
        }

        /// <summary>
        /// Class to store attached file info
        /// </summary>
        public class FileData
        {
            public string FileName { get; set; }
            public string ContentType { get; set; }
            public byte[] Data { get; set; }

            public long Size { get { return (Data != null ? Data.LongLength : 0L); } }

            /// <summary>
            /// Create a FileData from HttpContent 
            /// </summary>
            /// <param name="file"></param>
            /// <returns></returns>
            public static FileData ReadFile(HttpContent file)
            {
                var data = file.ReadAsByteArrayAsync().ContinueWith(t => t.Result).Result;
                var result = new FileData()
                {
                    FileName = FixFilename(file.Headers.ContentDisposition.FileName),
                    ContentType = file.Headers.ContentType.ToString(),
                    Data = data
                };

                return result;
            }

            /// <summary>
            /// Amend filenames to remove surrounding quotes and remove path from IE
            /// </summary>
            /// <param name="original"></param>
            /// <returns></returns>
            private static string FixFilename(string original)
            {
                var result = original.Trim();
                // remove leading and trailing quotes
                if (result.StartsWith("\""))
                    result = result.TrimStart('"').TrimEnd('"');
                // remove full path versions
                if(result.Contains("\\"))
                    // parse out path
                    result = new System.IO.FileInfo(result).Name;

                return result;
            }
        }    
    }
}