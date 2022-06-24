using AnimeBot.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace AnimeBot.Controllers
{
    public class TelegramWebhookController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromServices] UpdateHandler handleUpdateService,
                                          [FromBody] Update update)
        {

            await handleUpdateService.EchoAsync(update);
            return Ok();
        }
    }
}
