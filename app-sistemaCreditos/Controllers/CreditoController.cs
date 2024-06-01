using app_sistemaCreditos.Models;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace app_sistemaCreditos.Controllers
{
    [Authorize]
    public class CreditoController : Controller
    {
        public ActionResult Credito(string ID)
        {
            DataSet dsi = new DataSet();
            var url = "";

            if (string.IsNullOrEmpty(ID))
                url = "http://localhost/api_Creditos/rest/api/listarCreditos";
            else
                url = "http://localhost/api_Creditos/rest/api/listarCreditoXID?ID=" + ID;


            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            string responseBody;

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            responseBody = objReader.ReadToEnd();
                        }
                    }
                    dsi = JsonConvert.DeserializeObject<DataSet>(responseBody);
                }
            }
            catch (Exception ex)
            {

            }

            var deudores = GetApiData("http://localhost/api_Creditos/rest/api/listarDeudores");
            var articulos = GetApiData("http://localhost/api_Creditos/rest/api/listarArticulos");
            var amortizaciones = GetApiData("http://localhost/api_Creditos/rest/api/listarAMortizaciones");
            var empleados = GetApiData("http://localhost/api_Creditos/rest/api/listarEmpleados");

            // Pasar los datos a la vista
            ViewBag.Deudores = deudores.Tables[0];
            ViewBag.Articulos = articulos.Tables[0];
            ViewBag.Amortizaciones = amortizaciones.Tables[0];
            ViewBag.Empleados = empleados.Tables[0];

            return View(dsi);
        }

        private DataSet GetApiData(string apiUrl)
        {
            var request = (HttpWebRequest)WebRequest.Create(apiUrl);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            string responseBody;

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            responseBody = objReader.ReadToEnd();
                        }
                    }
                    return JsonConvert.DeserializeObject<DataSet>(responseBody);
                }
            }
            catch (Exception ex)
            {
                return new DataSet();
            }
        }

        public ActionResult newCredito(int IdDeudor, int IdArticulo, int IdAmortizacion, int IdEmpleado, int NoCuotas)
        {
            var url = "http://localhost/api_Creditos/rest/api/insertarCredito";
            var articuloUrl = "http://localhost/api_Creditos/rest/api/listarArticuloXID?ID=" + IdArticulo;
            var amortizacionUrl = "http://localhost/api_Creditos/rest/api/listarAmortizacionesXID?ID=" + IdAmortizacion;

            dynamic articuloInfo;
            try
            {
                var articuloRequest = (HttpWebRequest)WebRequest.Create(articuloUrl);
                articuloRequest.Method = "GET";
                articuloRequest.ContentType = "application/json";
                articuloRequest.Accept = "application/json";

                using (var articuloResponse = (HttpWebResponse)articuloRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(articuloResponse.GetResponseStream()))
                    {
                        var responseText = streamReader.ReadToEnd();
                        articuloInfo = JsonConvert.DeserializeObject<dynamic>(responseText);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al obtener información del artículo";
                return RedirectToAction("Credito");
            }

            dynamic amortizacionInfo;
            try
            {
                var amortizacionRequest = (HttpWebRequest)WebRequest.Create(amortizacionUrl);
                amortizacionRequest.Method = "GET";
                amortizacionRequest.ContentType = "application/json";
                amortizacionRequest.Accept = "application/json";

                using (var amortizacionResponse = (HttpWebResponse)amortizacionRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(amortizacionResponse.GetResponseStream()))
                    {
                        var responseText = streamReader.ReadToEnd();
                        amortizacionInfo = JsonConvert.DeserializeObject<dynamic>(responseText);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al obtener información de la amortización";
                return RedirectToAction("Credito");
            }

            var articulo = articuloInfo.Articulo[0];
            var amortizacion = amortizacionInfo.Amortizacion[0];

            decimal precio = (decimal)articulo.Precio;
            decimal tasaInteres = ((decimal)amortizacion.TasaInteres) / 100;

            var SaldoAmortizacion = precio + (precio * tasaInteres);
            var Cuota = SaldoAmortizacion / NoCuotas;
            var stock = (int)articulo.Stock - 1;

            if ((int)articulo.Stock == 0)
            {
                TempData["ErrorMessage"] = "No hay stock de este articulo";
                return RedirectToAction("Credito");
            }

            var nuevoArticulo = new
            {
                IdDeudor = IdDeudor,
                IdArticulo = IdArticulo,
                IdAmortizacion = IdAmortizacion,
                IdEmpleado = IdEmpleado,
                SaldoAmortizacion = SaldoAmortizacion,
                SaldoActual = 0,
                Cuota = Cuota,
                NoCuotas = NoCuotas,
                Estado = 1
            };

            string json = JsonConvert.SerializeObject(nuevoArticulo);

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        var responseText = streamReader.ReadToEnd();
                        var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseText);

                        if (apiResponse.Respuesta == 1)
                        {
                            updateArticulo(IdArticulo, articulo.Nombre.ToString(), articulo.Descripcion.ToString(), precio, stock);
                            TempData["SuccessMessage"] = "La acción se completó satisfactoriamente";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Error al realizar la acción";
                        }
                    }
                }
                return RedirectToAction("Credito");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al realizar la acción";
                return RedirectToAction("Credito");
            }
        }

        public Boolean updateArticulo(int Id, string Nombre, string Descripcion, decimal Precio, int Stock)
        {
            var url = "http://localhost/api_Creditos/rest/api/actualizarArticulo";

            var actualizarArticulo = new
            {
                Id = Id,
                Nombre = Nombre,
                Descripcion = Descripcion,
                Precio = Precio,
                Stock = Stock
            };

            string json = JsonConvert.SerializeObject(actualizarArticulo);

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        var responseText = streamReader.ReadToEnd();
                        var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseText);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al obtener articulo";
            }

            return true;
        }


        public ActionResult updateCredito(int ID, int IdDeudor, int IdArticulo, string IdArticuloAnterior, int IdAmortizacion, int IdEmpleado, int NoCuotas)
        {
            var url = "http://localhost/api_Creditos/rest/api/actualizarCredito";
            var articuloUrl = "http://localhost/api_Creditos/rest/api/listarArticuloXID?ID=" + IdArticulo;
            var amortizacionUrl = "http://localhost/api_Creditos/rest/api/listarAmortizacionesXID?ID=" + IdAmortizacion;
            GetArticulo(IdArticuloAnterior);

            dynamic articuloInfo;
            try
            {
                var articuloRequest = (HttpWebRequest)WebRequest.Create(articuloUrl);
                articuloRequest.Method = "GET";
                articuloRequest.ContentType = "application/json";
                articuloRequest.Accept = "application/json";

                using (var articuloResponse = (HttpWebResponse)articuloRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(articuloResponse.GetResponseStream()))
                    {
                        var responseText = streamReader.ReadToEnd();
                        articuloInfo = JsonConvert.DeserializeObject<dynamic>(responseText);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al obtener información del artículo";
                return RedirectToAction("Credito");
            }

            dynamic amortizacionInfo;
            try
            {
                var amortizacionRequest = (HttpWebRequest)WebRequest.Create(amortizacionUrl);
                amortizacionRequest.Method = "GET";
                amortizacionRequest.ContentType = "application/json";
                amortizacionRequest.Accept = "application/json";

                using (var amortizacionResponse = (HttpWebResponse)amortizacionRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(amortizacionResponse.GetResponseStream()))
                    {
                        var responseText = streamReader.ReadToEnd();
                        amortizacionInfo = JsonConvert.DeserializeObject<dynamic>(responseText);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al obtener información de la amortización";
                return RedirectToAction("Credito");
            }

            var articulo = articuloInfo.Articulo[0];
            var amortizacion = amortizacionInfo.Amortizacion[0];

            decimal precio = (decimal)articulo.Precio;
            decimal tasaInteres = ((decimal)amortizacion.TasaInteres) / 100;

            var SaldoAmortizacion = precio + (precio * tasaInteres);
            var Cuota = SaldoAmortizacion / NoCuotas;
            var stock = (int)articulo.Stock - 1;

            if ((int)articulo.Stock == 0)
            {
                TempData["ErrorMessage"] = "No hay stock de este articulo";
                return RedirectToAction("Credito");
            }

            var actualizarCredito = new
            {
                ID = ID,
                IdDeudor = IdDeudor,
                IdArticulo = IdArticulo,
                IdAmortizacion = IdAmortizacion,
                IdEmpleado = IdEmpleado,
                SaldoAmortizacion = SaldoAmortizacion,
                SaldoActual = 0,
                Cuota = Cuota,
                NoCuotas = NoCuotas,
                Estado = 1
            };


            string json = JsonConvert.SerializeObject(actualizarCredito);

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        var responseText = streamReader.ReadToEnd();
                        var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseText);

                        if (apiResponse.Respuesta == 1)
                        {
                            updateArticulo(IdArticulo, articulo.Nombre.ToString(), articulo.Descripcion.ToString(), precio, stock);
                            TempData["SuccessMessage"] = "La acción se completó satisfactoriamente";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Error al realizar la acción";
                        }
                    }
                }
                return RedirectToAction("Credito");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al realizar la acción";
                return RedirectToAction("Credito");
            }

        }

        public void GetArticulo(string Id)
        {
            DataSet dsi = new DataSet();
            var url = "http://localhost/api_Creditos/rest/api/listarArticuloXID?ID=" + Id;


            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            string responseBody;

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            responseBody = objReader.ReadToEnd();
                        }
                    }
                    dsi = JsonConvert.DeserializeObject<DataSet>(responseBody);


                    DataRow articuloRow = dsi.Tables[0].Rows[0];
                    string Nombre = articuloRow["Nombre"].ToString();
                    string Descripcion = articuloRow["Descripcion"].ToString();
                    string Precio = articuloRow["Precio"].ToString();
                    string Stock = articuloRow["Stock"].ToString();

                    int articuloId = Convert.ToInt32(Id);
                    decimal precio = Convert.ToDecimal(Precio);
                    int stock = Convert.ToInt32(Stock) + 1;

                    updateArticulo(articuloId, Nombre, Descripcion, precio, stock);

                }
            }
            catch (Exception ex)
            {

            }
        }

        public ActionResult deleteCredito(int ID, string IdArticulo)
        {
            var url = "http://localhost/api_Creditos/rest/api/eliminarCredito";

            var idCredito = new
            {
                ID = ID,
            };

            string json = JsonConvert.SerializeObject(idCredito);

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        var responseText = streamReader.ReadToEnd();
                        var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseText);

                        if (apiResponse.Respuesta == 1)
                        {
                            TempData["SuccessMessage"] = "La acción se completó satisfactoriamente";
                            GetArticulo(IdArticulo);
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Error al realizar la acción";
                        }
                    }
                }
                return RedirectToAction("Credito");

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al realizar la acción";
                return RedirectToAction("Credito");
            }

        }

        
        public ActionResult GenerateReport()
        {
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/CreditosReport.rdl");

            DataSet ds = GetApiData("http://localhost/api_Creditos/rest/api/listarCreditos");

            ReportDataSource reportDataSource = new ReportDataSource("CreditoDataSet", ds.Tables[0]);
            localReport.DataSources.Add(reportDataSource);

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            string[] streams;
            Warning[] warnings;
            byte[] renderedBytes;

            renderedBytes = localReport.Render(
                reportType,
                null,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            return File(renderedBytes, mimeType, "CreditosReport.pdf");
        }
    }
}