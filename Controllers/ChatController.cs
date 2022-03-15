using Microsoft.AspNetCore.Mvc;
using NotReksaChat.Services;

namespace NotReksaChat.Controllers
{
    [Route("/")]
    public class ChatController : Controller
    {
        public Online Users { get; }

        public ChatController(Online usersOnline)
        {
            Users = usersOnline;
        }

        [Route("")]
        public IActionResult Index()
        {
            ViewBag.Online = Users.GetAll();
            return View();
        }
    }
}