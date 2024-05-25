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
    public class DeudorController : Controller
    {
        public ActionResult Deudor(string ID)
        {
            DataSet dsDeudor = new DataSet();
            var url = "";

            if (string.IsNullOrEmpty(ID))
                url = "http://localhost/api_Creditos/rest/api/listarDeudores";
            else
                url = "http://localhost/api_Creditos/rest/api/listarDeudorXID?ID=" + ID;


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
                    dsDeudor = JsonConvert.DeserializeObject<DataSet>(responseBody);
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier error que ocurra durante la solicitud
                // Puedes agregar código aquí para manejar errores de manera apropiada
            }
            return View(dsDeudor);
        }

        public ActionResult NuevoDeudor(int NIT, int DPI, string Nombre, string Apellido, string Domicilio, string Telefono)
        {
            var url = "http://localhost/api_Creditos/rest/api/insertarDeudor";

            // Crear un objeto con los datos del nuevo deudor
            var nuevoDeudor = new
            {
                NIT = NIT,
                DPI = DPI,
                Nombre = Nombre,
                Apellido = Apellido,
                Domicilio = Domicilio,
                Telefono = Telefono,
            };

            string json = JsonConvert.SerializeObject(nuevoDeudor);

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
            catch (Exception)
            {
                // Manejar cualquier error que ocurra durante la solicitud
                // Puedes agregar código aquí para manejar errores de manera apropiada
            }

            return RedirectToAction("Deudor");
        }

        public ActionResult updateDeudor(int ID, int NIT, int DPI, string Nombre, string Apellido, string Domicilio, string Telefono)
        {
            var url = "http://localhost/api_Creditos/rest/api/actualizarDeudor";

            var actualizarEmpleado = new
            {
                ID = ID,
                NIT = NIT,
                DPI = DPI,
                Nombre = Nombre,
                Apellido = Apellido,
                Domicilio = Domicilio,
                Telefono = Telefono
            };

            string json = JsonConvert.SerializeObject(actualizarEmpleado);

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

            return RedirectToAction("Deudor");
        }

        public ActionResult deleteDeudor(int ID)
        {
            var url = "http://localhost/api_Creditos/rest/api/eliminarDeudor";

            var idEmpleado = new
            {
                ID = ID,
            };

            string json = JsonConvert.SerializeObject(idEmpleado);

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

            return RedirectToAction("Deudor");
        }
    }
}
