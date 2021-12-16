using Lap1.Web.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectPaper.Areas.Admin.Controllers
{
    public class DashBoardController : Controller
    {
        private IOptions<ApplicationSettings> _settings;
        public DashBoardController(IOptions<ApplicationSettings> settings)
        {
            _settings = settings;
        }
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
