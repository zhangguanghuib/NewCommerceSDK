using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.Localization.Services.Messages;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;


namespace Contoso.QRCode.CommerceRuntime
{
    public class GetSalesTransactionCustomReceiptFieldService : IRequestHandlerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes => new[] { typeof(GetLocalizationCustomReceiptFieldServiceRequest) };

        internal const int DefaultQrCodeWith = 100;

        internal const int DefaultQrCodeHeight = 100;

        public async Task<Response> Execute(Request request)
        {
            ThrowIf.Null(request, nameof(request));
            ThrowIf.Null(request.RequestContext, $"{nameof(request)}.{nameof(request.RequestContext)}");

            switch (request)
            {
                case GetLocalizationCustomReceiptFieldServiceRequest receiptRequest:
                    return await GetCustomReceiptForSalesTransactionReceiptsAsync(receiptRequest).ConfigureAwait(false);
                default:
                    throw new NotSupportedException(string.Format("Request '{0}' is not supported.", request.GetType()));
            }
        }

        private static async Task<Response> GetCustomReceiptForSalesTransactionReceiptsAsync(GetLocalizationCustomReceiptFieldServiceRequest request)
        {
            ThrowIf.Null(request.SalesOrder, $"{nameof(request)}, {nameof(request.SalesOrder)}");

            string receiptFieldName = request.CustomReceiptField;
            string receiptFieldValue;

            switch (receiptFieldName)
            {
                case "QRCODESAMPLE":
                    receiptFieldValue = await GetQRCode(request).ConfigureAwait(false);
                    break;
                default:
                    return NotHandledResponse.Instance;
            }

            return new GetCustomReceiptFieldServiceResponse(receiptFieldValue);
        }

        private static async Task<string> GetQRCode(GetLocalizationCustomReceiptFieldServiceRequest request)
        {
            string receiptFieldValue = string.Empty;
            string qrCodeContent = "https://microsoft.com";

            if (!string.IsNullOrEmpty(qrCodeContent))
            {
                string encodedQrCode = await EncodedQrCode(qrCodeContent, request.RequestContext).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(encodedQrCode))
                {
                    encodedQrCode = ConvertBase64PngToBase64JpgImageSharp(encodedQrCode);

                    receiptFieldValue = $"<L:{encodedQrCode}>";
                }
            }
            return receiptFieldValue;
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
                        BitsPerPixel = BmpBitsPerPixel.Pixel8,
                    });

                    outputStream.Position = 0;
                    byte[] jpgBytes = outputStream.ToArray();
                    return Convert.ToBase64String(jpgBytes);
                }
            }
        }
    }
}
