using CarRental.Models;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace CarRental.Controllers
{
    [RoutePrefix("api/bill")]
    public class BillController : ApiController
    {
        CarEntities db = new CarEntities();
        Response response = new Response();
        private string pdfPath = @"C:\";

        [HttpPost, Route("generateReport")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GenerateReport([FromBody] Bill bill)
        {
            try
            {
                var token = Request.Headers.GetValues("Authorization").FirstOrDefault(); // Use FirstOrDefault to avoid exceptions
                if (string.IsNullOrEmpty(token))
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Authorization token is missing");
                }

                TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                var ticks = DateTime.Now.Ticks;
                var guid = Guid.NewGuid().ToString();
                var uniqueId = ticks.ToString() + guid;
                bill.createdBy = tokenClaim.Email;
                bill.uuid = uniqueId;
                db.Bills.Add(bill);
                db.SaveChanges();
                GeneratePdf(bill);

                return Request.CreateResponse(HttpStatusCode.OK, new { uid = bill.uuid });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        private void GeneratePdf(Bill bill)
        {
            try
            {
                dynamic productDetails = JsonConvert.DeserializeObject(bill.productDetails);
                var todayDate = "Date: " + DateTime.Today.ToString("MM/dd/yyyy");

                PdfWriter writer = new PdfWriter(pdfPath + bill.uuid + ".pdf");
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                //Header
                Paragraph header = new Paragraph("Car Rental")
                    .SetBold()
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(25);
                document.Add(header);

                //New line
                Paragraph newline = new Paragraph(new Text("\n"));

                //Line Separator
                LineSeparator lineSeparator = new LineSeparator(new SolidLine());
                document.Add(lineSeparator);

                //Customer Details
                Paragraph customerDetails = new Paragraph("Name: " + bill.name + "\nEmail: " + bill.email + "\nContact Number: " + bill.contactNumber + "\nPayment Method: " + bill.paymentMethod);
                document.Add(customerDetails);

                // Create a table with 5 columns and set its width
                Table table = new Table(5, false);
                table.SetWidth(new UnitValue(UnitValue.PERCENT, 100));

                //Header 
                Cell headerName = new Cell(1, 1)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .Add(new Paragraph("Name"));

                Cell headerCategory = new Cell(1, 1)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .Add(new Paragraph("Category"));

                Cell headerPrice = new Cell(1, 1)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .Add(new Paragraph("Price"));

                Cell headerDuration = new Cell(1, 1)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .Add(new Paragraph("Duration in Days"));

                Cell headerSubTotal = new Cell(1, 1)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .Add(new Paragraph("SubTotal"));

                // Add header cells to the table
                table.AddHeaderCell(headerName);
                table.AddHeaderCell(headerCategory);
                table.AddHeaderCell(headerPrice);
                table.AddHeaderCell(headerDuration);
                table.AddHeaderCell(headerSubTotal);

                foreach (JObject product in productDetails)
                {
                    Cell nameCell = new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(product["name"].ToString()));

                    Cell categoryCell = new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(product["category"].ToString()));

                    Cell durationCell = new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(product["duration"].ToString()));

                    Cell priceCell = new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(product["price"].ToString()));

                    Cell totalCell = new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(product["total"].ToString()));

                    // Add these cells to the table
                    table.AddCell(nameCell);
                    table.AddCell(categoryCell);
                    table.AddCell(durationCell);
                    table.AddCell(priceCell);
                    table.AddCell(totalCell);
                }

                document.Add(table);
                Paragraph last = new Paragraph("Total: " + bill.totalAmount + "\nThank You for visiting. Please visit again!");
                document.Add(last);
                document.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [HttpPost, Route("getPdf")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetPdf([FromBody] Bill bill)
        {
            try
            {
                if (bill.name != null)
                {
                    GeneratePdf(bill);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                string filePath = pdfPath + bill.uuid.ToString() + ".pdf";
                byte[] bytes = File.ReadAllBytes(filePath);

                response.Content = new ByteArrayContent(bytes);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = bill.uuid.ToString() + ".pdf";
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(bill.uuid.ToString() + ".pdf"));

                return response;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet, Route("getBills")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetBills()
        {
            try
            {
                var token = Request.Headers.GetValues("Authorization").FirstOrDefault(); // Use FirstOrDefault to avoid exceptions
                if (string.IsNullOrEmpty(token))
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Authorization token is missing");
                }

                TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                if (tokenClaim.Role == "admin")
                {
                    var userResult = db.Bills
                        .Where(x => x.createdBy == tokenClaim.Email)
                        .AsEnumerable()
                        .Reverse()
                        .ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, userResult);
                }
                else
                {
                    var adminResult = db.Bills
                        .AsEnumerable()
                        .Reverse()
                        .ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, adminResult);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost, Route("deleteBill/{id}")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage DeleteBill(int id)
        {
            try
            {
                Bill billObj = db.Bills.Find(id);
                if (billObj == null)
                {
                    response.message = "Bill id does not found";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }

                db.Bills.Remove(billObj);
                db.SaveChanges();

                response.message = "Bill Deleted Successfully";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
