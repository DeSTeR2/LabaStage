using System.Text;

namespace HospitalMVC.HospitalInfrastructure.Services.PdfService
{
    public static class PDFTemplateGenerator
    {
        public static string Generate(DataContainer data)
        {
            var sb = new StringBuilder();
            sb.Append(@"
                        <html>
                            <head>
                            </head>
                            <body>
                                <div class='header'><h1>This is the generated PDF report!!!</h1></div>
                                <table align='center'>
                                    <tr>
                                        <th>Medical product name</th>
                                        <th>Description</th>
                                    </tr>");
            for (int i=0; i<data.names.Count(); i++)
            {
                sb.AppendFormat(@"<tr>
                                    <td>{0}</td>
                                    <td>{1}</td>
                                  </tr>", data.names[i], data.descriptions[i]);
            }
            sb.Append(@"
                                </table>
                            </body>
                        </html>");
            return sb.ToString();
        }
    }
}
