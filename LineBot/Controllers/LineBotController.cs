using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Line.Messaging;
using LineBot.Hubs;
using LineBot.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

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
                //var content = "";

                //using (var reader = new StreamReader(_httpContext.Request.Body))
                //{
                //    content = await reader.ReadToEndAsync();
                //}

                //await _hubContext.Clients.All.SendAsync("ShowLog", $"Request: Content: {content}");

                var events = await _httpContext.Request.GetWebhookEventsAsync(_lineBotConfig.ChannelSecret);
                var lineMessagingClient = new LineMessagingClient(_lineBotConfig.AccessToken);
                var lineBotApp = new LineBotApp(lineMessagingClient, _hubContext);
                await lineBotApp.RunAsync(events);
            }
            catch (Exception ex)
            {
                //需要 Log 可自行加入
                //_logger.LogError(JsonConvert.SerializeObject(ex));
                await _hubContext.Clients.All.SendAsync("ShowLog", $"Error in LineBotController.Run: {ex.ToString()}");
            }
            return Ok();
        }

        [HttpGet("Check")]
        [HttpPost("Check")]
        public async Task<IActionResult> Check()
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("ShowLog", $"LineBotController.Check");

                throw new Exception("出錯囉");
            }
            catch (Exception e)
            {
                await _hubContext.Clients.All.SendAsync("ShowLog", $"Error in LineBotController.Check: {e.ToString()}");
            }

            return Ok();
        }
    }
}