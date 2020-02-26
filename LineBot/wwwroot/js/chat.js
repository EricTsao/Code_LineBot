"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/linebot/chathub").build();

connection.on("PlayNextVideo", function (link) {
    console.log('PlayNextVideo', link);

    playNextVideo();
});

connection.on("AddVideo", function (link) {
    console.log('AddVideo', link);

    let youtubeId = getVideoId(link);

    addVideo(youtubeId);
});

connection.on("InsertVideo", function (link) {
    console.log('InsertVideo', link);

    let youtubeId = getVideoId(link);

    insertVideo(youtubeId);
});

connection.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});
