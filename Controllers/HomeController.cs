using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication11.Models;
using System.Data.OleDb;
namespace WebApplication11.Controllers
{
    public class HomeController : Controller
    {
        string connectionString =
@"Provider=Microsoft.ACE.OLEDB.12.0;
Data Source=C:\Users\hp\OneDrive\Documents\Airport.accdb"; 
        static List<Ticket> tickets = new List<Ticket>();
        public IActionResult Index()
        {
            List<Ticket> list = new List<Ticket>();

            using (OleDbConnection con = new OleDbConnection(connectionString))
            {
                con.Open();

                OleDbCommand cmd = new OleDbCommand("SELECT * FROM Table1", con);
                OleDbDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new Ticket
                    {
                        Code = reader["Code"].ToString(),
                        Flight = reader["Flight"].ToString(),
                        Time = reader["Time"].ToString()
                    });
                }
            }

            return View(list);
        }
        [HttpPost]
        public IActionResult AddTicket(string flight, string time)
        {
            string code = "FLY" + new Random().Next(100, 999);

            using (OleDbConnection con = new OleDbConnection(connectionString))
            {
                con.Open();

                OleDbCommand cmd = new OleDbCommand(
                    "INSERT INTO Table1 (Code, Flight, [Time]) VALUES (?, ?, ?)", con);

                cmd.Parameters.AddWithValue("?", code);
                cmd.Parameters.AddWithValue("?", flight);
                cmd.Parameters.AddWithValue("?", time);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Scan(string code)
        {
            Ticket foundTicket = null;

            using (OleDbConnection con = new OleDbConnection(connectionString))
            {
                con.Open();

                OleDbCommand cmd = new OleDbCommand(
                    "SELECT * FROM Table1 WHERE Code = ?", con);

                cmd.Parameters.AddWithValue("?", code);

                OleDbDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    foundTicket = new Ticket
                    {
                        Code = reader["Code"].ToString(),
                        Flight = reader["Flight"].ToString(),
                        Time = reader["Time"].ToString()
                    };
                }
            }

            if (foundTicket != null)
            {
                ViewBag.Status = "ACIQ";
                ViewBag.Message = "Xoş gəlmisiniz! Keçid açıqdır.";
            }
            else
            {
                ViewBag.Status = "BAGLI";
                ViewBag.Message = "XƏTA: Etibarsız bilet kodu!";
            }

            // yenə DB-dən listi yüklə ki soldakı siyahı itməsin
            List<Ticket> list = new List<Ticket>();

            using (OleDbConnection con = new OleDbConnection(connectionString))
            {
                con.Open();
                OleDbCommand cmd = new OleDbCommand("SELECT * FROM Table1", con);
                OleDbDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new Ticket
                    {
                        Code = reader["Code"].ToString(),
                        Flight = reader["Flight"].ToString(),
                        Time = reader["Time"].ToString()
                    });
                }
            }

            return View("Index", list);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
