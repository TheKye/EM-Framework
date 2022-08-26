using System;
using System.IO;
using System.Net;
using System.Text;

namespace Eco.EM.Framework.Discord
{
    public class DiscordWebhook
    {
        /// <summary>
        /// Webhook url
        /// </summary>
        public string Url { get; set; }

        private void AddField(MemoryStream stream, string bound, string cDisposition, string cType, byte[] data)
        {
            string prefix = stream.Length > 0 ? "\r\n--" : "--";
            string fBegin = $"{prefix}{bound}\r\n";

            byte[] fBeginBuffer = Utils.Encode(fBegin);
            byte[] cDispositionBuffer = Utils.Encode(cDisposition);
            byte[] cTypeBuffer = Utils.Encode(cType);

            stream.Write(fBeginBuffer, 0, fBeginBuffer.Length);
            stream.Write(cDispositionBuffer, 0, cDispositionBuffer.Length);
            stream.Write(cTypeBuffer, 0, cTypeBuffer.Length);
            stream.Write(data, 0, data.Length);
        }

        private void SetJsonPayload(MemoryStream stream, string bound, string json)
        {
            string cDisposition = "Content-Disposition: form-data; name=\"payload_json\"\r\n";
            string cType = "Content-Type: application/octet-stream\r\n\r\n";
            AddField(stream, bound, cDisposition, cType, Utils.Encode(json));
        }

        private void SetFile(MemoryStream stream, string bound, int index, FileInfo file)
        {
            string cDisposition = $"Content-Disposition: form-data; name=\"file_{index}\"; filename=\"{file.Name}\"\r\n";
            string cType = "Content-Type: application/octet-stream\r\n\r\n";
            AddField(stream, bound, cDisposition, cType, File.ReadAllBytes(file.FullName));
        }

        /// <summary>
        /// Send webhook message
        /// </summary>
        public void Send(DiscordMessage message, params FileInfo[] files)
        {
            if (string.IsNullOrEmpty(Url))
                throw new ArgumentNullException("Invalid Webhook URL.");

            string bound = "------------------------" + DateTime.Now.Ticks.ToString("x");
            WebClient webhookRequest = new();
            webhookRequest.Headers.Add("Content-Type", "multipart/form-data; boundary=" + bound);
            
            MemoryStream stream = new();
            for (int i = 0; i < files.Length; i++)
                SetFile(stream, bound, i, files[i]);

            string json = message.ToString();
            SetJsonPayload(stream, bound, json);

            byte[] bodyEnd = Utils.Encode($"\r\n--{bound}--");
            stream.Write(bodyEnd, 0, bodyEnd.Length);

            try
            {
                webhookRequest.UploadData(this.Url, stream.ToArray());
            }
            catch (WebException ex)
            {
                throw new WebException(Utils.Decode(ex.Response.GetResponseStream()));
            }

            stream.Dispose();
        }
    }
}
