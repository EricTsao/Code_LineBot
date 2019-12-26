using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LineBot.Controllers
{
    public class LineBotController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post()
        {
            string ChannelAccessToken = "Ip7io7NOK9j+5WKRbCkxzy26ufwCNDvy3eKVARM01gn5FjiIoyK+Z7PHyObefWGadbCRtzYfi73SnuY+Ox+UfBzpRAcMwG220Oy8kfLyW/VcGN6cnD251yG9/mtdcwWlGrPFePksk5oWBHGYkwq9fQdB04t89/1O/w1cDnyilFU=";

            try
            {
                //取得 http Post RawData(should be JSON)
                var postData = Request.Content.ReadAsStringAsync().Result;
                //剖析JSON
                var peceivedMessage = isRock.LineBot.Utility.Parsing(postData);
                //回覆訊息
                var message = "你說了:" + peceivedMessage.events[0].message.text;
                //回覆用戶
                isRock.LineBot.Utility.ReplyMessage(peceivedMessage.events[0].replyToken, message, ChannelAccessToken);
                //回覆API OK
                return Ok();
            }
            catch (Exception ex)
            {
                return Ok();
            }
        }
    }
}
