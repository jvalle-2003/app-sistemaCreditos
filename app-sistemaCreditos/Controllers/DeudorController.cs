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
    public class DeudorController : Controller
    {
        public ActionResult Deudor(string ID)
        {
            DataSet dsDeudor = new DataSet();
            var url = "";

            if (string.IsNullOrEmpty(ID))
                url = "http://localhost/api-sistemaCreditos/rest/api/listarDeudores";
            else
                url = "http://localhost/api-sistemaCreditos/rest/api/listarDeudorXID?ID=" + ID;


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
            }
            return View(dsDeudor);
        }

        public ActionResult NuevoDeudor(int NIT, int DPI, string Nombre, string Apellido, string Domicilio, string Telefono)
        {
            var url = "http://localhost/api-sistemaCreditos/rest/api/insertarDeudor";

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
                return RedirectToAction("Deudor");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error al realizar la acción";
                return RedirectToAction("Deudor");
            }
        }

        public ActionResult updateDeudor(int ID, int NIT, int DPI, string Nombre, string Apellido, string Domicilio, string Telefono)
        {
            var url = "http://localhost/api-sistemaCreditos/rest/api/actualizarDeudor";

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
                return RedirectToAction("Deudor");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al realizar la acción";
                return RedirectToAction("Deudor");

            }
        }

        public ActionResult deleteDeudor(int ID)
        {
            var url = "http://localhost/api-sistemaCreditos/rest/api/eliminarDeudor";

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
                return RedirectToAction("Deudor");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al realizar la acción";
                return RedirectToAction("Deudor");
            }
        }
    }
}
