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
    public class EmpleadoController : Controller
    {
        public ActionResult Empleado(string ID)
        {
            DataSet dsi = new DataSet();
            var url = "";

            if (string.IsNullOrEmpty(ID))
                url = "http://localhost/api-sistemaCreditos/rest/api/listarEmpleados";            
            else           
                url = "http://localhost/api-sistemaCreditos/rest/api/listarEmpleadoById?id=" + ID;
            

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            string responseBody;

            try
            {
                using(WebResponse response = request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        using(StreamReader objReader = new StreamReader(strReader))
                        {
                            responseBody = objReader.ReadToEnd();
                        }
                    }
                    dsi = JsonConvert.DeserializeObject<DataSet>(responseBody);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al realizar la acción";
            }
            return View(dsi);
        }

        public ActionResult newEmpleado(string Nombres, string Apellidos, string Puesto)
        {
            var url = "http://localhost/api-sistemaCreditos/rest/api/insertarEmpleado";

            var nuevoEmpleado = new
            {
                Nombres = Nombres,
                Apellidos = Apellidos,
                Puesto = Puesto
            };

            string json = JsonConvert.SerializeObject(nuevoEmpleado);

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
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al realizar la acción";
            }
            return RedirectToAction("Empleado");
        }

        public ActionResult updateEmpleado(int ID, string Nombres, string Apellidos, string Puesto)
        {
            var url = "http://localhost/api-sistemaCreditos/rest/api/actualizarEmpleado";

            var actualizarEmpleado = new
            {
                ID = ID,
                Nombres = Nombres,
                Apellidos = Apellidos,
                Puesto = Puesto
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

            return RedirectToAction("Empleado");
        }

        public ActionResult deleteEmpleado(int ID)
        {
            var url = "http://localhost/api-sistemaCreditos/rest/api/eliminarEmpleado";

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

            return RedirectToAction("Empleado");
        }
    }
}