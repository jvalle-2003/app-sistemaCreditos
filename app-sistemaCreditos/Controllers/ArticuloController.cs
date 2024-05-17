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
    public class ArticuloController : Controller
    {
        public ActionResult Articulo(string Id)
        {
            DataSet dsi = new DataSet();
            var url = "";

            if (string.IsNullOrEmpty(Id))
                url = "http://localhost/api_Creditos/rest/api/listarArticulos";
            else
                url = "http://localhost/api_Creditos/rest/api/listarArticuloXID?ID=" + Id;


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

        public ActionResult newArticulo(string Nombre, string Descripcion, double Precio, int Stock)
        {
            var url = "http://localhost/api_Creditos/rest/api/insertarArticulo";

            // Crear un objeto con los datos del nuevo articulo
            var nuevoArticulo = new
            {
                Nombre = Nombre,
                Descripcion = Descripcion,
                Precio = Precio,
                Stock = Stock
            };

            string json = JsonConvert.SerializeObject(nuevoArticulo);

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

            return RedirectToAction("Articulo");
        }

        public ActionResult updateArticulo(int Id, string Nombre, string Descripcion, double Precio, int Stock)
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
                    ViewBag.SuccessMessage = "La acción se completó satisfactoriamente";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al realizar la acción";
            }

            return RedirectToAction("Articulo");
        }

        public ActionResult deleteArticulo(int Id)
        {
            var url = "http://localhost/api_Creditos/rest/api/eliminarArticulo";

            var idArticulo = new
            {
                Id = Id,
            };

            string json = JsonConvert.SerializeObject(idArticulo);

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

            return RedirectToAction("Articulo");
        }
    }
}