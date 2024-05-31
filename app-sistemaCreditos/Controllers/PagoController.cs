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

            var empleados = GetApiData("http://localhost/api-sistemaCreditos/rest/api/listarEmpleados");
            var creditos = GetApiData("http://localhost/api-sistemaCreditos/rest/api/listarCreditos");
            var todoCreditos = GetApiData("http://localhost/api-sistemaCreditos/rest/api/listarTodosCreditos");  

            ViewBag.Creditos = creditos.Tables[0];
            ViewBag.Empleados = empleados.Tables[0];
            ViewBag.TodosCreditos = todoCreditos.Tables[0];

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
                            var idCredit = IdCredito.ToString();
                            GetCredito(idCredit, MontoPago);
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

        public void GetCredito(string Id, decimal pago)
        {
            DataSet dsi = new DataSet();
            var url = "http://localhost/api-sistemaCreditos/rest/api/listarCreditoXID?ID=" + Id;


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


                    DataRow creditoRow = dsi.Tables[0].Rows[0];
                    int IdDeudor = Convert.ToInt32(creditoRow["IdDeudor"]);
                    int IdArticulo = Convert.ToInt32(creditoRow["IdArticulo"]);
                    int IdAmortizacion = Convert.ToInt32(creditoRow["IdAmortizacion"]);
                    int IdEmpleado = Convert.ToInt32(creditoRow["IdEmpleado"]);
                    decimal SaldoAmortizacion = Convert.ToDecimal(creditoRow["SaldoAmortizacion"]);
                    decimal SaldoActual = Convert.ToDecimal(creditoRow["SaldoActual"]) + pago;
                    decimal Cuota = Convert.ToDecimal(creditoRow["Cuota"]);
                    int NoCuotas = Convert.ToInt32(creditoRow["NoCuotas"]);

                    int creditoId = Convert.ToInt32(Id);

                    updateCredito(creditoId, IdDeudor, IdArticulo, IdAmortizacion, IdEmpleado, SaldoAmortizacion, SaldoActual, Cuota, NoCuotas);

                }
            }
            catch (Exception ex)
            {

            }
        }

        public void GetCreditoActualizarEliminar(string Id)
        {
            DataSet dsi = new DataSet();
            var url = "http://localhost/api-sistemaCreditos/rest/api/listarCreditoXID?ID=" + Id;


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


                    DataRow creditoRow = dsi.Tables[0].Rows[0];
                    int IdDeudor = Convert.ToInt32(creditoRow["IdDeudor"]);
                    int IdArticulo = Convert.ToInt32(creditoRow["IdArticulo"]);
                    int IdAmortizacion = Convert.ToInt32(creditoRow["IdAmortizacion"]);
                    int IdEmpleado = Convert.ToInt32(creditoRow["IdEmpleado"]);
                    decimal Cuota = Convert.ToDecimal(creditoRow["Cuota"]);
                    decimal SaldoAmortizacion = Convert.ToDecimal(creditoRow["SaldoAmortizacion"]);
                    decimal SaldoActual = Convert.ToDecimal(creditoRow["SaldoActual"]) - Cuota;
                    int NoCuotas = Convert.ToInt32(creditoRow["NoCuotas"]);
                    int creditoId = Convert.ToInt32(Id);

                    updateCredito(creditoId, IdDeudor, IdArticulo, IdAmortizacion, IdEmpleado, SaldoAmortizacion, SaldoActual, Cuota, NoCuotas);

                }
            }
            catch (Exception ex)
            {

            }
        }


        public ActionResult updateCredito(int ID, int IdDeudor, int IdArticulo, int IdAmortizacion, int IdEmpleado, decimal SaldoAmortizacion, decimal SaldoActual, decimal Cuota, int NoCuotas)
        {
            var url = "http://localhost/api-sistemaCreditos/rest/api/actualizarCredito";

            var estado = 1;
            if(SaldoAmortizacion == SaldoActual)
            {
                estado = 0;
            }

            var actualizarCredito = new
            {
                ID = ID,
                IdDeudor = IdDeudor,
                IdArticulo = IdArticulo,
                IdAmortizacion = IdAmortizacion,
                IdEmpleado = IdEmpleado,
                SaldoAmortizacion = SaldoAmortizacion,
                SaldoActual = SaldoActual,
                Cuota = Cuota,
                NoCuotas = NoCuotas,
                Estado = estado
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

        public ActionResult updatePago(int ID, int IdCredito, decimal MontoPago, int IdEmpleado, int IDCreditoAnterior)
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
                            var idCredit = IdCredito.ToString();
                            var idCreditoAnterior = IDCreditoAnterior.ToString();
                            GetCreditoActualizarEliminar(idCreditoAnterior);
                            GetCredito(idCredit, MontoPago);
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

        public ActionResult deletePago(int ID, int IdCredito)
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
                            var idCredit = IdCredito.ToString();
                            GetCreditoActualizarEliminar(idCredit);
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