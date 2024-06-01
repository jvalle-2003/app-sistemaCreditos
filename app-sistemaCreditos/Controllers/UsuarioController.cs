using app_sistemaCreditos.Models;
using Microsoft.Reporting.WebForms;
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
    public class UsuarioController : Controller
    {

        public ActionResult Usuario(String ID)
        {
            DataSet dsi = new DataSet();
            var url = "";

            if (string.IsNullOrEmpty(ID))
                url = "http://localhost/api_Creditos/rest/api/listarUsuarios";
            else
                url = "http://localhost/api_Creditos/rest/api/listarUsuarioXID?id=" + ID;


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
                ViewBag.ErrorMessage = "Error al realizar la acción";
            }

            var empleados = GetApiData("http://localhost/api_Creditos/rest/api/listarEmpleados");

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

        public ActionResult newUsuario(string Usuario, string Contraseña, int IdEmpleado)
        {
            var url = "http://localhost/api_Creditos/rest/api/insertarUsuario";

            var nuevoUsuario = new
            {
                Usuario = Usuario,
                Contraseña = Contraseña,
                IdEmpleado = IdEmpleado
            };

            string json = JsonConvert.SerializeObject(nuevoUsuario);

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
                return RedirectToAction("Usuario");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al realizar la acción";
                return RedirectToAction("Usuario");
            }
        }

        public ActionResult updateUsuario(int ID, string Usuario, string Contraseña, int IdEmpleado)
        {
            var url = "http://localhost/api_Creditos/rest/api/actualizarUsuario";

            var actualizarUsuario = new
            {
                ID = ID,
                Usuario = Usuario,
                Contraseña = Contraseña,
                IdEmpleado = IdEmpleado
            };

            string json = JsonConvert.SerializeObject(actualizarUsuario);

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
                return RedirectToAction("Usuario");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al realizar la acción";
                return RedirectToAction("Usuario");
            }
        }

        public ActionResult deleteUsuario(int ID)
        {
            var url = "http://localhost/api_Creditos/rest/api/eliminarUsuario";

            var idUsuario = new
            {
                ID = ID,
            };

            string json = JsonConvert.SerializeObject(idUsuario);

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
                return RedirectToAction("Usuario");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al realizar la acción";
                return RedirectToAction("Usuario");
            }

        }

        public ActionResult GenerateReport()
        {
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/UsuariosReport.rdl");

            DataSet ds = GetApiData("http://localhost/api_Creditos/rest/api/listarUsuarios");

            ReportDataSource reportDataSource = new ReportDataSource("UsuarioDataSet", ds.Tables[0]);
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

            return File(renderedBytes, mimeType, "UsuarioReport.pdf");
        }

    }
}