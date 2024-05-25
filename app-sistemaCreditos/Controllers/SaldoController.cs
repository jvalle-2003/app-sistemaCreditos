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
    public class SaldoController : Controller
    {
        public ActionResult Saldo(string ID)
        {
            DataSet dsi = new DataSet();
            var url = "";

            if (string.IsNullOrEmpty(ID))
                url = "http://localhost/api_Creditos/rest/api/listarSaldos";
            else
                url = "http://localhost/api_Creditos/rest/api/listarSaldoXID?ID=" + ID;


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
            return View(dsi);
        }

        public ActionResult newSaldo(double SaldoInicial, double SaldoActual, double CuotaMensual, double InteresGenerado, int IdCredito)
        {
            var url = "http://localhost/api_Creditos/rest/api/insertarSaldo";

            var nuevoSaldo = new
            {
                SaldoInicial = SaldoInicial,
                SaldoActual = SaldoActual,
                CuotaMensual = CuotaMensual,
                InteresGenerado = InteresGenerado,
                IdCredito = IdCredito   
            };

            string json = JsonConvert.SerializeObject(nuevoSaldo);

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

            return RedirectToAction("Saldo");
        }

        public ActionResult updateSaldo(int ID, double SaldoInicial, double SaldoActual, double CuotaMensual, double InteresGenerado, int IdCredito)
        {
            var url = "http://localhost/api_Creditos/rest/api/actualizarSaldo";

            var actualizarSaldo = new
            {
                ID = ID,
                SaldoInicial = SaldoInicial,
                SaldoActual = SaldoActual,
                CuotaMensual = CuotaMensual,
                InteresGenerado = InteresGenerado,
                IdCredito = IdCredito
            };

            string json = JsonConvert.SerializeObject(actualizarSaldo);

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

            return RedirectToAction("Saldo");
        }

        public ActionResult deleteSaldo(int ID)
        {
            var url = "http://localhost/api_Creditos/rest/api/eliminarSaldo";

            var idSaldo = new
            {
                ID = ID,
            };

            string json = JsonConvert.SerializeObject(idSaldo);

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

            return RedirectToAction("Saldo");
        }

    }
}