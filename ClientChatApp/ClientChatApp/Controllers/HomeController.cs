using ClientChatApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ClientChatApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static string LocalDataPath
        {
            get
            {
                var path = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
                return Path.Combine(path, "accounts.json");
            }
        }
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var exited = Request.Cookies?.FirstOrDefault(a => a.Key.Equals(Constants.AUTHENTICATED_USER, StringComparison.OrdinalIgnoreCase));
            if(exited != null
                && exited.Value.Key != null)
            {
                ViewBag.AuhenUser = exited.Value.Value;
                return View();
            }
            return RedirectToAction("Login");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginModel login)
        {
            var accountsTxt = System.IO.File.ReadAllText(LocalDataPath);
            var accounts = JsonConvert.DeserializeObject<List<AccountModel>>(accountsTxt);

            if (login != null
            && accounts != null)
            {
                var matched = accounts.FirstOrDefault(f => f.UserName.Equals(login.UserName, StringComparison.OrdinalIgnoreCase)
                                                        && f.Password == login.Password);
                if (matched != null)
                {
                    var exited = Request.Cookies != null && Request.Cookies.Any(a => a.Key.Equals(Constants.AUTHENTICATED_USER, StringComparison.OrdinalIgnoreCase));
                    if (exited)
                        Response.Cookies.Delete(Constants.AUTHENTICATED_USER);

                    Response.Cookies.Append(Constants.AUTHENTICATED_USER, JsonConvert.SerializeObject(matched));
                    return RedirectToAction("index");
                }
            }
            return View();
        }
        [HttpPost]
        public IActionResult NewConnect([FromBody]ChannelInfo channel)
        {
            var exited = Request.Cookies?.FirstOrDefault(a => a.Key.Equals(Constants.AUTHENTICATED_USER, StringComparison.OrdinalIgnoreCase));
            if (exited == null || exited.Value.Key == null)
                return RedirectToAction("Login"); 
            
            var currentUser = JsonConvert.DeserializeObject<AccountModel>(exited.Value.Value);
            if (currentUser == null || currentUser.Id <= 0)
            {
                Response.Cookies.Delete(Constants.AUTHENTICATED_USER);
                return RedirectToAction("Login");
            }
               
            if (channel != null)
            {
                var accountsTxt = System.IO.File.ReadAllText(LocalDataPath);
                var accounts = JsonConvert.DeserializeObject<List<AccountModel>>(accountsTxt);

                var matched = accounts.FirstOrDefault(f => f.Id == currentUser.Id);
                if (matched == null)
                {
                    Response.Cookies.Delete(Constants.AUTHENTICATED_USER);
                    return RedirectToAction("Login");
                }
                if (matched.Channels == null)
                    matched.Channels = new List<ChannelInfo>();

                var matchedChannel = matched.Channels.FirstOrDefault(f => f.Id == channel.Id);
                if (matchedChannel != null)
                    matchedChannel.Name = channel.Name ?? matched.UserName;
                else
                    matched.Channels.Add(new ChannelInfo
                    {
                        Id = channel.Id,
                        Name = channel.Name
                    });

                Response.Cookies.Delete(Constants.AUTHENTICATED_USER);
                Response.Cookies.Append(Constants.AUTHENTICATED_USER, JsonConvert.SerializeObject(matched));

                System.IO.File.WriteAllText(LocalDataPath, JsonConvert.SerializeObject(accounts));
            }

            return RedirectToAction("index");
        }
        public IActionResult InitMockData()
        {
            var mock = new List<AccountModel>();
            mock.Add(new AccountModel { 
              Id = DateTime.UtcNow.Ticks,
              UserName = "ThanhDat",
              Password = "123",
              Channels = new List<ChannelInfo>
              {
                  new ChannelInfo
                  {
                      Id = 0,
                      Name = "Chung"
                  },
                  new ChannelInfo
                  {
                      Id = 1,
                      Name = "Person"
                  },
                  new ChannelInfo
                  {
                      Id = 3,
                      Name = "Class 01"
                  },
                  new ChannelInfo
                  {
                      Id = 6,
                      Name = "Class 02"
                  }
              }
            });

            mock.Add(new AccountModel
            {
                Id = DateTime.UtcNow.Ticks,
                UserName = "XuanHuong",
                Password = "321",
                Channels = new List<ChannelInfo>
              {
                  new ChannelInfo
                  {
                      Id = 0,
                      Name = "Chung"
                  },
                  new ChannelInfo
                  {
                      Id = 4,
                      Name = "Person"
                  },
                  new ChannelInfo
                  {
                      Id = 3,
                      Name = "Class 01"
                  }
              }
            });

            mock.Add(new AccountModel
            {
                Id = DateTime.UtcNow.Ticks,
                UserName = "ThiHong",
                Password = "123",
                Channels = new List<ChannelInfo>
              {
                  new ChannelInfo
                  {
                      Id = 0,
                      Name = "Chung"
                  },
                  new ChannelInfo
                  {
                      Id = 5,
                      Name = "Person"
                  },
                  new ChannelInfo
                  {
                      Id = 3,
                      Name = "Class 01"
                  },
                  new ChannelInfo
                  {
                      Id = 6,
                      Name = "Class 02"
                  }
              }
            });

            System.IO.File.WriteAllText(LocalDataPath, JsonConvert.SerializeObject(mock));
            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
