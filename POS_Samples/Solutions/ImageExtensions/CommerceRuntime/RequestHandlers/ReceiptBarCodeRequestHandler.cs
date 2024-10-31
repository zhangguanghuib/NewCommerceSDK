using System.Net.NetworkInformation;

namespace Contoso.GasStationSample.CommerceRuntime.RequestHandlers
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Contoso.GasStationSample.CommerceRuntime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    // 3rd-Party Packages.
    using ZXing;
    using ZXing.Common;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Formats.Png;

    public class ReceiptBarCodeRequestHandler : IRequestHandlerAsync
    {
        public IEnumerable<System.Type> SupportedRequestTypes
        {
            get
            {
                return new[]
                {
                    typeof(GetBarCodeImageRequest),
                };
            }
        }

        public async Task<Response> Execute(Request request)
        {
            ThrowIf.Null(request, nameof(request));

            switch (request)
            {
                case GetBarCodeImageRequest getBarCodeImageRequest:
                    return await this.GetEncodedBarCode(getBarCodeImageRequest).ConfigureAwait(false);
                default:
                    throw new NotSupportedException($"Request '{request.GetType()}' is not supported.");
            }
        }

        private  async Task<GetBarCodeImageResponse> GetEncodedBarCode(GetBarCodeImageRequest getBarCodeImageRequest)
        {
            string barcodeBase64String = string.Empty;

            if (!string.IsNullOrEmpty(getBarCodeImageRequest.BarCodeString))
            {
                barcodeBase64String = this.GenerateBarcodeBase64(getBarCodeImageRequest.BarCodeString);
            }

            return await Task.FromResult(new GetBarCodeImageResponse(barcodeBase64String)).ConfigureAwait(false); 
        }


        /// <summary>
        /// Generates a barcode image from a string and returns its base64 representation.
        /// </summary>
        /// <param name="text">The text to encode in the barcode.</param>
        /// <returns>A base64 string representation of the generated barcode image.</returns>
        public string GenerateBarcodeBase64(string text)
        {
            var barcodeWriter = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Width = 300,         // Width of the barcode image
                    Height = 100,        // Height of the barcode image
                    Margin = 10          // Margin around the barcode
                }
            };

            var pixelData = barcodeWriter.Write(text);
            using var image = SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(pixelData.Pixels, pixelData.Width, pixelData.Height);         
            using var memoryStream = new MemoryStream();
            image.Save(memoryStream, new PngEncoder());        // Save as PNG to memory stream
            byte[] barcodeBytes = memoryStream.ToArray();      // Convert stream to byte array
            string base64String = Convert.ToBase64String(barcodeBytes);

            return base64String;
        }
    }
}
