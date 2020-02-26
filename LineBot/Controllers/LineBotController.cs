using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Line.Messaging;
using LineBot.Hubs;
using LineBot.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace LineBot.Controllers
{
    [Route("api/linebot")]
    public class LineBotController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpContext _httpContext;
        private readonly LineBotConfig _lineBotConfig;
        private readonly IHubContext<ChatHub> _hubContext;

        public LineBotController(
            IServiceProvider serviceProvider,
            LineBotConfig lineBotConfig,
            IHubContext<ChatHub> hubContext)
        {
            _httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            _httpContext = _httpContextAccessor.HttpContext;
            _lineBotConfig = lineBotConfig;
            _hubContext = hubContext;
        }

        //完整的路由網址就是 https://xxx/api/linebot/run
        [HttpPost("run")]
        public async Task<IActionResult> Run()
        {
            try
            {
                var events = await _httpContext.Request.GetWebhookEventsAsync(_lineBotConfig.ChannelSecret);
                var lineMessagingClient = new LineMessagingClient(_lineBotConfig.AccessToken);
                var lineBotApp = new LineBotApp(lineMessagingClient, _hubContext);
                await lineBotApp.RunAsync(events);
            }
            catch (Exception ex)
            {
                //需要 Log 可自行加入
                //_logger.LogError(JsonConvert.SerializeObject(ex));
            }
            return Ok();
        }

        [HttpGet("Check")]
        public async Task<IActionResult> Check()
        {
            return Ok();
        }
    }
}