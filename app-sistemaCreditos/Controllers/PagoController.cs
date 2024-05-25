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
    public class PagoController : Controller
    {
        public ActionResult Pago(string ID)
        {
            DataSet dsi = new DataSet();
            var url = "";

            if (string.IsNullOrEmpty(ID))
                url = "http://localhost/api-sistemaCreditos/rest/api/listarPagos";
            else
                url = "http://localhost/api-sistemaCreditos/rest/api/listarPagoXID?ID=" + ID;


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
            return View(dsi);
        }

        public ActionResult newPago(int IdCredito, int MontoPago, int IdEmpleado)
        {
            var url = "http://localhost/api-sistemaCreditos/rest/api/insertarPago";

            var nuevoPago = new
            {
                IdCredito = IdCredito,
                MontoPago = MontoPago,
                IdEmpleado = IdEmpleado
            };

            string json = JsonConvert.SerializeObject(nuevoPago);

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
                return RedirectToAction("Pago");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al realizar la acción";
                return RedirectToAction("Pago");
            }
        }

        public ActionResult updatePago(int ID, int IdCredito, int MontoPago, int IdEmpleado)
        {
            var url = "http://localhost/api-sistemaCreditos/rest/api/actualizarPagos";

            var actualizarPago = new
            {
                ID = ID,
                IdCredito = IdCredito,
                MontoPago = MontoPago,
                IdEmpleado = IdEmpleado
            };

            string json = JsonConvert.SerializeObject(actualizarPago);

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
                return RedirectToAction("Pago");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al realizar la acción";
                return RedirectToAction("Pago");
            }

        }

        public ActionResult deletePago(int ID)
        {
            var url = "http://localhost/api-sistemaCreditos/rest/api/eliminarPago";

            var idPago = new
            {
                ID = ID,
            };

            string json = JsonConvert.SerializeObject(idPago);

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
                return RedirectToAction("Pago");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al realizar la acción";
                return RedirectToAction("Pago");
            }

        }
    }
}