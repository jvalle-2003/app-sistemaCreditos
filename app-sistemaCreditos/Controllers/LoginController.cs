using app_sistemaCreditos.Models.usuarios;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Security;

namespace app_sistemaCreditos.Controllers
{
    public class LoginController : Controller
    {
        [NoLoginAccess]
        public ActionResult Login()
        {
            ViewBag.ErrorMessage = "";
            return View();
        }

        public ActionResult Logged(string usuario, string password)
        {
            try
            {
                if (usuario != "" && password != "")
                {
                    var url = "http://localhost/api-sistemaCreditos/rest/api/listarUsuarios";

                    var request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "GET";
                    request.ContentType = "application/json";
                    request.Accept = "application/json";

                    using (var response = (HttpWebResponse)request.GetResponse())
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string jsonResponse = reader.ReadToEnd();

                        var responseObj = JsonConvert.DeserializeObject<Dictionary<string, List<Usuarios>>>(jsonResponse);

                        var usuarios = responseObj["usuarios"];
                        var usuarioValido = usuarios.Any(u => u.Usuario == usuario && u.Contraseña == password);

                        if (usuarioValido)
                        {
                            FormsAuthentication.SetAuthCookie(usuario, false);

                            // Indicar que el usuario está autenticado
                            HttpContext.User = new GenericPrincipal(new GenericIdentity(usuario), null);
                            return RedirectToAction("Index", "Home");
                        }
                    }

                    ViewBag.ErrorMessage = "Credenciales inválidas";
                    return View("Login");
                }
                ViewBag.ErrorMessage = "Debe completar todos los campos";
                return View("Login");

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error: " + ex.Message;
                return View("Login");
            }
        }

    }
}