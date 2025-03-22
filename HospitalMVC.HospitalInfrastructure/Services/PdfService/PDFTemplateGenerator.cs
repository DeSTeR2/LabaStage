using System.Reflection.Metadata;
using System.Text;
using Utils;
using System.IO; // For file handling
using Microsoft.AspNetCore.Hosting; // For IWebHostEnvironment (if using .NET Core)
// Added: For culture handling
using System.Globalization;

namespace HospitalMVC.HospitalInfrastructure.Services.PdfService
{
    public static class PDFTemplateGenerator
    {
        public static string Generate(DataContainer data, IWebHostEnvironment env)
        {
            string doctorName = data.doctor.Name;
            string patientName = data.patient.Name;
            string receiptNumber = data.receiptId.ToString();
            DateTime receiptDate = data.createTime;

            string hospitalLogoPath = Path.Combine(env.WebRootPath, "css", "images", "hospital-logo.png");

            string signatureRelativePath = data.doctor.SignaturePictireUrl;
            string doctorSignaturePath = env.WebRootPath + signatureRelativePath;

            string stampPath = env.WebRootPath + Constants.HospitalStampImagePath;

            string hospitalLogoBase64 = File.Exists(hospitalLogoPath)
                ? Convert.ToBase64String(File.ReadAllBytes(hospitalLogoPath))
                : "";
            string doctorSignatureBase64 = File.Exists(doctorSignaturePath)
                ? Convert.ToBase64String(File.ReadAllBytes(doctorSignaturePath))
                : "";

            string stampPathBase64 = File.Exists(stampPath)
                ? Convert.ToBase64String(File.ReadAllBytes(stampPath))
                : "";

            // Added: Debug logging for image paths and base64 strings
            Console.WriteLine($"Hospital Logo Path: {hospitalLogoPath}");
            Console.WriteLine($"Doctor Signature Path: {doctorSignaturePath}");
            Console.WriteLine($"Stamp Path: {stampPath}");
            Console.WriteLine($"Hospital Logo Base64: {(string.IsNullOrEmpty(hospitalLogoBase64) ? "Empty" : "Generated")}");
            Console.WriteLine($"Doctor Signature Base64: {(string.IsNullOrEmpty(doctorSignatureBase64) ? "Empty" : "Generated")}");
            Console.WriteLine($"Stamp Base64: {(string.IsNullOrEmpty(stampPathBase64) ? "Empty" : "Generated")}");

            // Added: Fallback for hospital logo in JPG format
            string hospitalLogoJpgPath = Path.Combine(env.WebRootPath, "css", "images", "hospital-logo.jpg");
            string hospitalLogoJpgBase64 = File.Exists(hospitalLogoJpgPath)
                ? Convert.ToBase64String(File.ReadAllBytes(hospitalLogoJpgPath))
                : "";

            // Added: More detailed debug logging for hospital logo
            if (File.Exists(hospitalLogoPath))
            {
                FileInfo hospitalLogoInfo = new FileInfo(hospitalLogoPath);
                Console.WriteLine($"Hospital Logo File Size: {hospitalLogoInfo.Length} bytes");
            }
            if (File.Exists(hospitalLogoJpgPath))
            {
                FileInfo hospitalLogoJpgInfo = new FileInfo(hospitalLogoJpgPath);
                Console.WriteLine($"Hospital Logo JPG File Size: {hospitalLogoJpgInfo.Length} bytes");
            }
            Console.WriteLine($"Hospital Logo JPG Base64: {(string.IsNullOrEmpty(hospitalLogoJpgBase64) ? "Empty" : "Generated")}");

            string hospitalName = Constants.HospitalName;
            string hospitalAddress = Constants.HospitalAddress;

            // Added: Format the date in English using en-US culture
            string receiptDateEnglish = receiptDate.ToString("MMMM dd, yyyy", new CultureInfo("en-US"));

            var sb = new StringBuilder();
            sb.Append(@"
                <html>
                    <head>
                        <style>
                            body { font-family: Arial, sans-serif; margin: 20px; }
                            .header { text-align: center; margin-bottom: 20px; }
                            .hospital-info { margin-bottom: 20px; border-bottom: 2px solid #000; padding-bottom: 10px; }
                            .logo { max-width: 100px; float: left; margin-right: 20px; }
                            .patient-doctor-info { margin: 20px 0; padding: 15px; background: #f5f5f5; }
                            .table-container { margin: 20px 0; }
                            table { width: 100%; border-collapse: collapse; }
                            th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
                            th { background-color: #4CAF50; color: white; }
                            .receipt-info { margin: 20px 0; }
                            .footer { margin-top: 40px; border-top: 1px solid #000; padding-top: 20px; }
                            .stamp { color: red; font-style: italic; float: left; width: 50%; }
                            .signature { float: right; width: 50%; text-align: right; }
                            .signature-img { max-width: 200px; max-height: 100px; }
                            /* Added new classes for better image sizing */
                            .hospital-logo { width: 80px; height: 80px; object-fit: contain; }
                            .doctor-signature { width: 150px; height: 60px; object-fit: contain; }
                            .hospital-stamp { width: 100px; height: 100px; object-fit: contain; }
                            /* Added: Ensure hospital logo renders correctly in PDF */
                            .hospital-logo-container { display: inline-block; float: left; margin-right: 20px; }
                            .hospital-logo-fallback { width: 80px; height: 80px; background: #f0f0f0; text-align: center; line-height: 80px; font-size: 12px; color: #666; }
                            /* Added: Clearfix to prevent floating issues in PDF */
                            .clearfix::after { content: ''; display: table; clear: both; }
                        </style>
                    </head>
                    <body>
                        <!-- Hospital Information -->
                        <div class='hospital-info clearfix'>");

            // Embed hospital logo as base64
            if (!string.IsNullOrEmpty(hospitalLogoBase64))
            {
                // Added: Wrap the hospital logo in a container with a fallback
                sb.Append("<div class='hospital-logo-container'>");
                sb.Append($"<img src='data:image/png;base64,{hospitalLogoBase64}' class='logo hospital-logo' alt='Hospital Logo' onerror=\"this.style.display='none'; this.nextSibling.style.display='block';\"/>");
                // Added: Try JPG fallback if PNG fails
                if (!string.IsNullOrEmpty(hospitalLogoJpgBase64))
                {
                    sb.Append($"<img src='data:image/jpeg;base64,{hospitalLogoJpgBase64}' class='logo hospital-logo' alt='Hospital Logo (JPG)' style='display: none;' onerror=\"this.style.display='none'; this.nextSibling.style.display='block';\"/>");
                }
                sb.Append("<div class='hospital-logo-fallback' style='display: none;'>Logo Failed</div>");
                sb.Append("</div>");
            }
            else
            {
                sb.Append("<p>[Hospital Logo Missing]</p>");
            }

            sb.Append($@"
                            <h2>{hospitalName}</h2>
                            <p>{hospitalAddress}</p>
                        </div>

                        <!-- Patient and Doctor Information -->
                        <div class='patient-doctor-info'>
                            <h3>Patient and Doctor Details</h3>
                            <p><strong>Patient Name:</strong> {patientName}</p>
                            <p><strong>Doctor Name:</strong> {doctorName}</p>
                        </div>

                        <!-- Receipt Information -->
                        <div class='receipt-info'>
                            <h3>Receipt Information</h3>
                            <p><strong>Receipt Number:</strong> {receiptNumber}</p>
                            <p><strong>Date:</strong> {receiptDateEnglish}</p>
                        </div>

                        <!-- Products Table -->
                        <div class='table-container'>
                            <div class='header'><h1>Medical Products Receipt</h1></div>
                            <table>
                                <tr>
                                    <th>Medical Product Name</th>
                                    <th>Description</th>
                                </tr>");

            for (int i = 0; i < data.names.Count(); i++)
            {
                sb.AppendFormat(@"<tr>
                                    <td>{0}</td>
                                    <td>{1}</td>
                                  </tr>", data.names[i], data.descriptions[i]);
            }

            sb.Append(@$"
                            </table>
                        </div>

                        <!-- Footer with Stamp and Signature -->
                        <div class='footer'>
                            <div class='stamp'>
                                <p>Official Hospital Stamp</p>
                                <img src='data:image/png;base64,{stampPathBase64}' class='signature-img hospital-stamp' alt='Hospital Stamp'/>
                            </div>
                            <div class='signature'>");

            // Embed doctor signature as base64
            if (!string.IsNullOrEmpty(doctorSignatureBase64))
            {
                sb.Append($"<img src='data:image/png;base64,{doctorSignatureBase64}' class='signature-img doctor-signature' alt='Doctor Signature'/>");
            }
            else
            {
                sb.Append("<p>Doctor's Signature: ___________________</p>");
            }

            sb.Append($@"
                                <p>Dr. {doctorName}</p>
                            </div>
                        </div>
                    </body>
                </html>");
            return sb.ToString();
        }
    }
}