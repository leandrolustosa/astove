using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace AInBox.Astove.Core.Reporting
{
    public class ReportViewer
    {
        public const string ContentTypePDF = "application/pdf";
        public const string ExtensionPDF = "pdf";
        public const string ContentTypeOctetStream = "application/octet-stream";
        public const string Attachment = "attachment";

        public static HttpResponseMessage GenerateHttpResponseMessage(string reportPath, string reportDataSourceName, object reportDataSourceValue, string outputFileName, List<Microsoft.Reporting.WebForms.ReportParameter> parameters)
        {
            var bytes = GeneratePDF(reportPath, reportDataSourceName, reportDataSourceValue, outputFileName, parameters);
            var stream = new MemoryStream(bytes, 0, bytes.Length, false, true);

            // Reset the stream position; otherwise, download will not work
            stream.Position = 0;

            // Create response message with blob stream as its content
            var message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(stream)
            };

            // Set content headers
            message.Content.Headers.ContentLength = stream.Length;
            message.Content.Headers.ContentType = new MediaTypeHeaderValue(ContentTypeOctetStream);
            message.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue(Attachment)
            {
                FileName = HttpUtility.UrlDecode(string.Format("{0}.{1}", outputFileName, ExtensionPDF)),
                Size = stream.Length
            };

            return message;
        }

        public static void RenderPDF(System.Web.UI.Page page, string reportPath, string reportDataSourceName, object reportDataSourceValue, string outputFileName, List<Microsoft.Reporting.WebForms.ReportParameter> parameters, string contentType)
        {
            Dictionary<string, object> dataSources = new Dictionary<string, object>();
            dataSources.Add(reportDataSourceName, reportDataSourceValue);
            RenderPDF(page, reportPath, dataSources, outputFileName, parameters, contentType);
        }

        public static byte[] GeneratePDF(string reportPath, string reportDataSourceName, object reportDataSourceValue, string outputFileName, List<Microsoft.Reporting.WebForms.ReportParameter> parameters, string contentType)
        {
            Dictionary<string, object> dataSources = new Dictionary<string, object>();
            dataSources.Add(reportDataSourceName, reportDataSourceValue);
            return GeneratePDF(reportPath, dataSources, outputFileName, parameters, contentType);
        }

        public static void RenderPDF(System.Web.UI.Page page, string reportPath, string reportDataSourceName, object reportDataSourceValue, string outputFileName, List<Microsoft.Reporting.WebForms.ReportParameter> parameters)
        {
            RenderPDF(page, reportPath, reportDataSourceName, reportDataSourceValue, outputFileName, parameters, ContentTypeOctetStream);
        }

        public static byte[] GeneratePDF(string reportPath, string reportDataSourceName, object reportDataSourceValue, string outputFileName, List<Microsoft.Reporting.WebForms.ReportParameter> parameters)
        {
            return GeneratePDF(reportPath, reportDataSourceName, reportDataSourceValue, outputFileName, parameters, ContentTypeOctetStream);
        }

        public static void RenderPDF(System.Web.UI.Page page, string reportPath, System.Collections.Generic.Dictionary<string, object> reportDataSources, string outputFileName, List<Microsoft.Reporting.WebForms.ReportParameter> parameters)
        {
            RenderPDF(page, reportPath, reportDataSources, outputFileName, parameters, ContentTypeOctetStream);
        }

        public static byte[] GeneratePDF(string reportPath, System.Collections.Generic.Dictionary<string, object> reportDataSources, string outputFileName, List<Microsoft.Reporting.WebForms.ReportParameter> parameters)
        {
            return GeneratePDF(reportPath, reportDataSources, outputFileName, parameters, ContentTypeOctetStream);
        }

        public static void RenderPDF(System.Web.UI.Page page, string reportPath, System.Collections.Generic.Dictionary<string, object> reportDataSources, string outputFileName, List<Microsoft.Reporting.WebForms.ReportParameter> parameters, string contentType)
        {
            byte[] bytes = GeneratePDF(reportPath, reportDataSources, outputFileName, parameters, contentType);

            System.IO.MemoryStream stream = new System.IO.MemoryStream(bytes);
            stream.Position = 0;

            page.Response.Clear();
            page.Response.ContentType = contentType;

            if (contentType.Equals(ContentTypeOctetStream, StringComparison.CurrentCultureIgnoreCase))
            {
                if (string.IsNullOrEmpty(outputFileName))
                    outputFileName = "report";

                page.Response.AppendHeader("Content-Disposition", "attachment;filename=" + outputFileName + ".pdf");
            }

            int bufSize = (int)stream.Length;
            Byte[] buf = new Byte[bufSize];
            int bytesRead = stream.Read(buf, 0, bufSize);
            stream.Close();
            stream.Dispose();

            page.Response.OutputStream.Write(buf, 0, bytesRead);
            page.Response.End();
        }

        public static byte[] GeneratePDF(string reportPath, System.Collections.Generic.Dictionary<string, object> reportDataSources, string outputFileName, List<Microsoft.Reporting.WebForms.ReportParameter> parameters, string contentType)
        {
            Microsoft.Reporting.WebForms.ReportViewer ReportViewer1 = new Microsoft.Reporting.WebForms.ReportViewer();
            ReportViewer1.LocalReport.ReportPath = reportPath;
            ReportViewer1.LocalReport.EnableExternalImages = true;

            if (parameters != null && parameters.Count > 0)
                ReportViewer1.LocalReport.SetParameters(parameters);

            if (reportDataSources != null)
            {
                foreach (System.Collections.Generic.KeyValuePair<string, object> dataSource in reportDataSources)
                    ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource(dataSource.Key, dataSource.Value));
            }

            Microsoft.Reporting.WebForms.Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            byte[] bytes = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
            return bytes;
        }
    }
}