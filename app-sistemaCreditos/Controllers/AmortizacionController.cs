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
    public class AmortizacionController : Controller
    {
        public ActionResult Amortizacion(string ID)
        {
            DataSet dsi = new DataSet();
            var url = "";

            if (string.IsNullOrEmpty(ID))
                url = "http://localhost/api_Creditos/rest/api/listarAMortizaciones";
            else
                url = "http://localhost/api_Creditos/rest/api/listarAmortizacionesXID?ID=" + ID;


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

        public ActionResult newAmortizacion(string Tipo, int TasaInteres)
        {
            var url = "http://localhost/api_Creditos/rest/api/insertarAmortizacion";

            // Crear un objeto con los datos del nuevo articulo
            var nuevoAmortizacion = new
            {
                Tipo = Tipo,
                TasaInteres = TasaInteres
            };

            string json = JsonConvert.SerializeObject(nuevoAmortizacion);

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

            return RedirectToAction("Amortizacion");
        }

        public ActionResult updateAmortizacion(int ID, string Tipo, int TasaInteres)
        {
            var url = "http://localhost/api_Creditos/rest/api/actualizarAmortizacion";

            var actualizarAmortizacion = new
            {
                ID = ID,
                Tipo = Tipo,
                TasaInteres = TasaInteres
            };

            string json = JsonConvert.SerializeObject(actualizarAmortizacion);

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

            return RedirectToAction("Amortizacion");
        }

        public ActionResult deleteAmortizacion(int ID)
        {
            var url = "http://localhost/api_Creditos/rest/api/eliminarAmortizacion";

            var idAmortizacion = new
            {
                ID = ID,
            };

            string json = JsonConvert.SerializeObject(idAmortizacion);

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

            return RedirectToAction("Amortizacion");
        }


    }
}