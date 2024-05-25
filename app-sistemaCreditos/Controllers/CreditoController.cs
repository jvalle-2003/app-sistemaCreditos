using app_sistemaCreditos.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
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
                url = "http://localhost/api-sistemaCreditos/rest/api/listarCreditos";
            else
                url = "http://localhost/api-sistemaCreditos/rest/api/listarCreditoXID?ID=" + ID;


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

            var deudores = GetApiData("http://localhost/api-sistemaCreditos/rest/api/listarDeudores");
            var articulos = GetApiData("http://localhost/api-sistemaCreditos/rest/api/listarArticulos");
            var amortizaciones = GetApiData("http://localhost/api-sistemaCreditos/rest/api/listarAMortizaciones");
            var empleados = GetApiData("http://localhost/api-sistemaCreditos/rest/api/listarEmpleados");

            // Pasar los datos a la vista
            ViewBag.Deudores = deudores;
            ViewBag.Articulos = articulos;
            ViewBag.Amortizaciones = amortizaciones;
            ViewBag.Empleados = empleados;


            return View(dsi);
        }

        private List<dynamic> GetApiData(string apiUrl)
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
                    return JsonConvert.DeserializeObject<List<dynamic>>(responseBody);
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return new List<dynamic>();
            }
        }

        public ActionResult newCredito(int IdDeudor, int IdArticulo, int IdAmortizacion, int IdEmpleado)
        {
            var url = "http://localhost/api-sistemaCreditos/rest/api/insertarCredito";

            var nuevoArticulo = new
            {
                IdDeudor = IdDeudor,
                IdArticulo = IdArticulo,
                IdAmortizacion = IdAmortizacion,
                IdEmpleado = IdEmpleado
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

        public ActionResult updateCredito(int ID, int IdDeudor, int IdArticulo, int IdAmortizacion, int IdEmpleado)
        {
            var url = "http://localhost/api-sistemaCreditos/rest/api/actualizarCredito";

            var actualizarCredito = new
            {
                ID = ID,
                IdDeudor = IdDeudor,
                IdArticulo = IdArticulo,
                IdAmortizacion = IdAmortizacion,
                IdEmpleado = IdEmpleado
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

        public ActionResult deleteCredito(int ID)
        {
            var url = "http://localhost/api-sistemaCreditos/rest/api/eliminarCredito";

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
    }
}