using Microsoft.AspNetCore.Mvc;
using NotReksaChat.Services;

namespace NotReksaChat.Controllers
{
    [Route("/")]
    public class ChatController : Controller
    {
        public IOnline Users { get; }

        public ChatController(IOnline usersOnline)
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