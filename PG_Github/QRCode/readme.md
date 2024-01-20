# Printing QR Code Sample
## Overview
This sample showcases a sample Store Commerce extension that prints a QR code on the receipt. The QR code can be scanned by a mobile device to open the receipt in the browser.

## Configuring the sample
1. Sign in to HQ.
2. Got to Retail and Commerce > Channel setup > POS setup > POS profiles > Language text.
3. On the POS tab, select Add to add new POS language text.
4. Enter the following values:
    - Language ID: en-us
    - Text ID: 10001
    - Text: QR Code
    <img width="646" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/087e2cd4-431a-43c0-b2f9-b2b161b35377">
5. On the Action Pane, select Save to save your changes.
6. Go to Retail and Commerce > Channel setup > POS setup > POS profiles > Custom fields.
7. On the Action Pane, select Add to add a new custom field, and specify the following information:
    - Name: QRCODESAMPLE
    - Type: Receipt
    - Caption text ID: 10001
    <img width="463" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/06f9a5ab-f416-466d-80f2-31d486d46b98">
8. On the Action Pane, select Save to save your changes.
9. Go to Retail and Commerce > Channel setup > POS setup > POS profiles > Receipt formats.
10. Select an existing or create a new receipt format and then select Designer on the Action Pane.
11. If you're prompted to confirm that you want to open the application, select Open, and then follow the installation instructions.
12. After the designer is installed, you're asked for Azure Active Directory (Azure AD) credentials. Enter the information to start the designer.
13. In the designer, drag and drop the **QR Code** field from the left pane to the receipt designer.
14. Save the changes.
    <img width="1365" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/f94aa695-5245-41b9-8798-31742586e3fe">
16. Go to Retail and Commerce > Retail and Commerce IT > Distribution schedule.
17. Select the Channel configuration (1070) job, and then select Run now.

## Running the sample
- Build the solution and install Scale Unit Extension and Store Commerce Extension
- You can see the QR code is able showing on the printer
<img width="464" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/88cefdbb-90bb-435a-980f-462a3d3da999">

## Additional Resources
- [Generate QR codes and print them on receipts](https://learn.microsoft.com/en-us/dynamics365/commerce/localizations/ind-generate-qr-code-print-receipt)
- Because of System.Drawing is not supported by .Net Standard, so the recommended way is using ImageSharp:
  https://github.com/SixLabors/ImageSharp
- So firstly Nuget Package ImageSharp will be added to the project:<br/>
  <img width="460" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/6e540b12-ac41-47bc-b7c7-b8576a4c3805"> <br/>
  Then using the below code to convert PNG to BMP:
  ```cs
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Formats.Bmp;

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
  ```

