using Microsoft.AspNetCore.Mvc;
using NotReksaChat.Services;

namespace NotReksaChat.Controllers
{
    [Route("/")]
    public class ChatController : Controller
    {
        private readonly IOnline _users;
        public ChatController(IOnline usersOnline)
        {
            _users = usersOnline;
        }

        [Route("")]
        public IActionResult Index()
        {
            ViewBag.Online = _users.GetAll();
            return View();
        }
    }
}