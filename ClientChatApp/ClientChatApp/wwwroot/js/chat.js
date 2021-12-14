"use strict";

var serverEndpoint = document.getElementById("js-hubServiceEndpoint").value;
var connection = new signalR.HubConnectionBuilder().withUrl(serverEndpoint).build();
var chatElements = document.getElementsByClassName("chat-container");
var currentUserId = document.getElementById("authen-user-id").value;
var currentUserName = document.getElementById("authen-user-name").value;
var currentUserGroupIds = (document.getElementById("authen-user-channels").value).split(',');


document.getElementsByClassName("sendButton").disabled = true;

connection.on("ReceiveGroupMessage", function (userid, username, groupid, groupname, message) {
    var li = document.createElement("li");
    li.textContent = `${username} says ${message}`;
    let container_id = `${currentUserId}-${groupid}`;
    let container = document.getElementById(container_id);
    container.getElementsByClassName("messagesList")[0].appendChild(li);
});

for (var current of chatElements) {
    let container_id = `${currentUserId}-${current.dataset.channelId}`;
    let container = document.getElementById(container_id);

    container.getElementsByClassName("sendButton")[0].addEventListener("click", function (event) {
        var message = container.getElementsByClassName("messageInput")[0].value;
        connection.invoke("SendMessageToGroup", currentUserId, currentUserName, container.dataset.channelId, container.dataset.channelName, message).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    }, false);
}

connection.start().then(function () {
    if (currentUserGroupIds) {
        connection.invoke("AddUserToGroup", currentUserGroupIds).catch(function (err) {
            return console.error(err.toString());
        });
    }
    document.getElementsByClassName("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});



document.getElementById("new-channel-btn").addEventListener("click", async function (event) {
    var id = document.getElementById("new-channel-id").value;
    var name = document.getElementById("new-channel-name").value;
    let data = {
        Id: Number(id),
        Name: name
    };
    await fetch("/home/NewConnect", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
            //'Content-Type': 'application/x-www-form-urlencoded',
        },
        body: JSON.stringify(data)
    });

    window.location.replace("/home/index");
    event.preventDefault();
}, false);






