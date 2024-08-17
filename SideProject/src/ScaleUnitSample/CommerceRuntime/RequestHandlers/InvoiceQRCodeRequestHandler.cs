

namespace CommerceRuntime.RequestHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Data;
    using Microsoft.Dynamics.Commerce.Runtime.DataAccess.SqlServer;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Contoso.CommerceRuntime.Entities.DataModel;
    using Contoso.CommerceRuntime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Localization.Services.Messages;
    using SixLabors.ImageSharp.Formats.Bmp;
    using System.IO;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp;

    public class InvoiceQRCodeRequestHandler : IRequestHandlerAsync
    {
        internal const int DefaultQrCodeWith = 200;

        internal const int DefaultQrCodeHeight = 200;

        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[]
                {
                    typeof(GetQRCodeImageRequest),
                };
            }
        }

        public Task<Response> Execute(Request request)
        {
            ThrowIf.Null(request, nameof(request));

            switch (request)
            {
                case GetQRCodeImageRequest getQRCodeImageRequest:
                    return this.GetEncodedQrCode(getQRCodeImageRequest);
                default:
                    throw new NotSupportedException($"Request '{request.GetType()}' is not supported.");
            }
        }

        private async Task<Response> GetEncodedQrCode(GetQRCodeImageRequest getQRCodeImageRequest)
        {
            string qrCodeContent = !string.IsNullOrEmpty(getQRCodeImageRequest.QRCodeurl) ? getQRCodeImageRequest.QRCodeurl : "https://microsoft.com";

            string encodedQrCode = string.Empty;

            if (!string.IsNullOrEmpty(qrCodeContent))
            {
                encodedQrCode = await EncodedQrCode(qrCodeContent, getQRCodeImageRequest.RequestContext).ConfigureAwait(false);

                //TODO
                //if (!string.IsNullOrEmpty(encodedQrCode))
                //{
                //    encodedQrCode = ConvertBase64PngToBase64JpgImageSharp(encodedQrCode);
                //}
            }
            return new GetQRCodeImageResponse(encodedQrCode); ;
        }

        private static async Task<string> EncodedQrCode(string qrCodeContent, RequestContext requestContext)
        {
            EncodeQrCodeServiceRequest qrCodeServiceRequest = new EncodeQrCodeServiceRequest(qrCodeContent)
            {
                Width = DefaultQrCodeWith,
                Height = DefaultQrCodeHeight
            };

            EncodeQrCodeServiceResponse qrCodeServiceResponse = await requestContext.ExecuteAsync<EncodeQrCodeServiceResponse>(qrCodeServiceRequest).ConfigureAwait(false); ;

            return qrCodeServiceResponse.QRCode;
        }

        public static string ConvertBase64PngToBase64JpgImageSharp(string base64Png)
        {
            byte[] imageBytes = Convert.FromBase64String(base64Png);

            using (var inputStream = new MemoryStream(imageBytes))
            using (var image = Image.Load(inputStream))
            {
                using (var outputStream = new MemoryStream())
                {
                    image.SaveAsBmp(outputStream, new BmpEncoder()
                    {
                        BitsPerPixel = BmpBitsPerPixel.Pixel32,
                    });

                    outputStream.Position = 0;
                    byte[] jpgBytes = outputStream.ToArray();
                    return Convert.ToBase64String(jpgBytes);
                }
            }
        }
    }
}
