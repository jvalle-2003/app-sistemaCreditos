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

            // Crear un objeto con los datos del nuevo articulo
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

            // Escribir los datos JSON en el cuerpo de la solicitud
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            // Enviar la solicitud y recibir la respuesta
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    // Puedes hacer algo con la respuesta si es necesario
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier error que ocurra durante la solicitud
                // Puedes agregar código aquí para manejar errores de manera apropiada
            }

            return RedirectToAction("Pago");
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
                    ViewBag.SuccessMessage = "La acción se completó satisfactoriamente";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al realizar la acción";
            }

            return RedirectToAction("Pago");
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
                    ViewBag.SuccessMessage = "La acción se completó satisfactoriamente";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al realizar la acción";
            }

            return RedirectToAction("Pago");
        }
    }
}