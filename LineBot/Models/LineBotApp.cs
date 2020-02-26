using Line.Messaging;
using Line.Messaging.Webhooks;
using LineBot.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LineBot.Models
{
    public class LineBotApp : WebhookApplication
    {
        private readonly LineMessagingClient _messagingClient;

        private readonly IHubContext<ChatHub> _hubContext;

        public LineBotApp(LineMessagingClient lineMessagingClient, IHubContext<ChatHub> hubContext)
        {
            _messagingClient = lineMessagingClient;
            _hubContext = hubContext;
        }

        protected override async Task OnMessageAsync(MessageEvent ev)
        {
            var result = new List<ISendMessage>();            

            try
            {
                switch (ev.Message)
                {
                    //文字訊息
                    case TextEventMessage textMessage:
                        {
                            //頻道Id
                            var channelId = ev.Source.Id;

                            //使用者Id
                            var userId = ev.Source.UserId;

                            if (textMessage.Text == "2020")
                            {
                                result.Add(new TextMessage($"純白世界！！"));
                            }
                            else if (textMessage.Text == "下一首" || textMessage.Text == "切歌")
                            {
                                await _hubContext.Clients.All.SendAsync("PlayNextVideo");

                                result.Add(new TextMessage($"OK, 播放下一首"));
                            }
                            else if (textMessage.Text.Contains("https://youtu.be/") || textMessage.Text.Contains("https://www.youtube.com/"))
                            {
                                if (textMessage.Text.StartsWith("點播"))
                                {
                                    var youtubeLink = textMessage.Text.Replace("點播 ", "");

                                    await _hubContext.Clients.All.SendAsync("AddVideo", youtubeLink);

                                    result.Add(new TextMessage($"OK, 已進行點播"));
                                }
                                else if (textMessage.Text.StartsWith("插播"))
                                {
                                    var youtubeLink = textMessage.Text.Replace("插播 ", "");

                                    await _hubContext.Clients.All.SendAsync("InsertVideo", youtubeLink);

                                    result.Add(new TextMessage($"OK, 已進行插播"));
                                }
                            }
                            else
                            {
                                var query = "";

                                if (textMessage.Text.StartsWith("點播"))
                                {
                                    query = textMessage.Text.Replace("點播 ", "");
                                }
                                else if (textMessage.Text.StartsWith("插播"))
                                {
                                    query = textMessage.Text.Replace("插播 ", "");
                                }
                                else if (textMessage.Text.StartsWith("找歌"))
                                {
                                    query = textMessage.Text.Replace("找歌 ", "");
                                }

                                if (!string.IsNullOrEmpty(query))
                                {
                                    var videos = YoutubeWebApiHelper.SearchVideos(query);

                                    if (videos.items.Count > 0)
                                    {
                                        //result.Add(new TextMessage($"OK, 與{query}相關的歌曲如下"));

                                        var carouselContainer = new CarouselContainer
                                        {
                                            Contents = new List<BubbleContainer>()
                                        };

                                        result.Add(new FlexMessage($"與{query}相關的歌曲查詢結果")
                                        {
                                            Contents = carouselContainer
                                        });

                                        foreach (var video in videos.items)
                                        {
                                            if (carouselContainer.Contents.Count == 10)
                                            {
                                                break;
                                            }

                                            carouselContainer.Contents.Add(new BubbleContainer
                                            {
                                                Header = new BoxComponent
                                                {
                                                    Layout = BoxLayout.Baseline,
                                                    Contents = new IFlexComponent[]
                                                    {
                                                        new TextComponent
                                                        {
                                                            Text = $"{video.snippet.title}",
                                                            Weight = Weight.Bold,
                                                            Size = ComponentSize.Md,
                                                            Wrap = true,
                                                            MaxLines = 2
                                                        },
                                                    }
                                                },
                                                Hero = new ImageComponent
                                                {
                                                    Url = video.snippet.thumbnails.medium.url,
                                                    AspectRatio = new AspectRatio(video.snippet.thumbnails.medium.width, video.snippet.thumbnails.medium.height),
                                                    AspectMode = AspectMode.Cover,
                                                    Size = ComponentSize.Full,
                                                },
                                                Footer = new BoxComponent
                                                {
                                                    Layout = BoxLayout.Vertical,
                                                    Contents = new List<IFlexComponent> {
                                                        new ButtonComponent
                                                        {
                                                            Style = ButtonStyle.Link,
                                                            Height = ButtonHeight.Sm,
                                                            Action = new MessageTemplateAction("點播", $"點播 https://youtu.be/{video.id.videoId}")
                                                        },
                                                        new SeparatorComponent(),
                                                        new ButtonComponent
                                                        {
                                                            Style = ButtonStyle.Link,
                                                            Height = ButtonHeight.Sm,
                                                            Action = new MessageTemplateAction("插播", $"插播 https://youtu.be/{video.id.videoId}")
                                                        },
                                                        new SeparatorComponent(),
                                                        new ButtonComponent
                                                        {
                                                            Style = ButtonStyle.Link,
                                                            Height = ButtonHeight.Sm,
                                                            Action = new UriTemplateAction("Youtube", $"https://youtu.be/{video.id.videoId}")
                                                        },
                                                    }
                                                }
                                            });
                                        }
                                    }
                                    else
                                    {
                                        result.Add(new TextMessage($"Sorry, 查無與{query}相關的歌曲"));
                                    }
                                }
                            }
                        }

                        break;
                }
            }
            catch (Exception e)
            {
                result.Add(new TextMessage($"Error, {e.ToString()}"));
            }

            await _messagingClient.ReplyMessageAsync(ev.ReplyToken, result);
        }
    }
}
