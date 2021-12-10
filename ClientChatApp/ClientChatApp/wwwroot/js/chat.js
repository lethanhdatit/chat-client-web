"use strict";

const { each } = require("jquery");

var serverEndpoint = document.getElementById("js-hubServiceEndpoint").value;
var connection = new signalR.HubConnectionBuilder().withUrl(serverEndpoint).build();
var chatElements = document.getElementsByClassName("chat-container");
for (var i = 0; i < chatElements.length; i++) {
    var current = chatElements[i];
    
    //Disable send button until connection is established
    current.getElementById("sendButton").disabled = true;

    connection.on("ReceiveMessage", function (user, message) {
        var li = document.createElement("li");
        current.getElementById("messagesList").appendChild(li);
        // We can assign user-supplied strings to an element's textContent because it
        // is not interpreted as markup. If you're assigning in any other way, you 
        // should be aware of possible script injection concerns.
        li.textContent = `${user} says ${message}`;
    });

    connection.start().then(function () {
        current.getElementById("sendButton").disabled = false;
    }).catch(function (err) {
        return console.error(err.toString());
    });

    current.getElementById("sendButton").addEventListener("click", function (event) {
        var user = current.getElementById("userName").innerText;
        var message = current.getElementById("messageInput").value;
        connection.invoke("SendMessage", user, message).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    });
}
