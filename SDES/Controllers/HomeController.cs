using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using SDES.Models;
using System.Text;

namespace SDES.Controllers
{
    public class HomeController : Controller
    {
        public static string sPath = "";
        public static string ArchivoAcutal = "";
        Logica objLogica = new Logica();

        public ActionResult Index()
        {
            if (!Directory.Exists(Server.MapPath("~/Archivos")))
            {
                Directory.CreateDirectory(Server.MapPath("~/Archivos"));
            }

            if (!Directory.Exists(Server.MapPath("~/Permutacion")))
            {
                Directory.CreateDirectory(Server.MapPath("~/Permutacion"));
            }

            sPath = Server.MapPath("~/Archivos");

            return View();
        }
        public ActionResult Archivo (){
            return View();
        }

        [HttpPost]
        public ActionResult Archivo(HttpPostedFileBase File, string clave)
        {
            var Permutaciones = new string[6];

            using (var stream = new FileStream(Server.MapPath("~/Permutacion") + "\\Permutaciones.txt", FileMode.OpenOrCreate))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var bytebuffer = new byte[10];
                    bytebuffer = reader.ReadBytes(10);
                    Permutaciones[0] = Encoding.UTF8.GetString(bytebuffer);
                    bytebuffer = reader.ReadBytes(8);
                    Permutaciones[1] = Encoding.UTF8.GetString(bytebuffer);
                    bytebuffer = reader.ReadBytes(4);
                    Permutaciones[2] = Encoding.UTF8.GetString(bytebuffer);
                    bytebuffer = reader.ReadBytes(8);
                    Permutaciones[3] = Encoding.UTF8.GetString(bytebuffer);
                    bytebuffer = reader.ReadBytes(8);
                    Permutaciones[4] = Encoding.UTF8.GetString(bytebuffer);
                }
            }

            Permutaciones[5] = Convert.ToString(Convert.ToInt32(clave), 2).PadLeft(10, '0');

            objLogica.ObtenerClaves(Permutaciones);

            var listLetra = new List<char>();
            using (var stream = new FileStream(File.FileName, FileMode.OpenOrCreate))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var bytebuffer = new byte[10000];
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        bytebuffer = reader.ReadBytes(10000);
                        foreach (var letra in bytebuffer)
                        {
                            listLetra.Add((char)letra);
                        }
                    }
                }
            }

            var opc = 0;
            var sPathArchivo = "";
            if (Path.GetExtension(File.FileName) != ".scif")
            {
                opc = 2;
                sPathArchivo = sPath + "\\" + Path.GetFileNameWithoutExtension(File.FileName) + ".scif";
            }
            else
            {
                sPathArchivo = sPath + "\\" + Path.GetFileNameWithoutExtension(File.FileName) + ".txt";

            }
            ArchivoAcutal = sPathArchivo;
            var text = new byte[listLetra.Count()];
            using (var streamW = new FileStream(sPathArchivo, FileMode.OpenOrCreate))
            {
                using (var writer = new BinaryWriter(streamW))
                {
                    var i = 0;
                    foreach (var item in listLetra)
                    {
                        text[i] = Convert.ToByte(objLogica.DescifradoYCifrado(item, opc));
                        i++;
                    }
                    foreach (var item in text)
                    {
                        writer.Write(item);
                    }
                }
            }
            return RedirectToAction("Descargar");
        }

        #region DESCARGAR
        public ActionResult Descargar()
        {
            var name = Path.GetFileName(ArchivoAcutal);
            return File(ArchivoAcutal, System.Net.Mime.MediaTypeNames.Application.Octet, name);
        }
        #endregion

        #region OTRAS FUNCIONES
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        #endregion
    }
}