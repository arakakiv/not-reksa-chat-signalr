"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();
var user = "";

function enterTextArea(event) {
    if (event.keyCode == 13) {
        sendMessage(event);
    }
}

function connect() {
    if (document.getElementById('username') == "") {
        alert('nome invalido pfv nao tente xss!! vai funcionar e vai bagunçar td!!');
        return;
    }

    user = document.getElementById('username').value;

    document.getElementById('username').disabled = true;
    document.getElementById('btnConnect').disabled = true;

    connection.start().then(function() {
        connection.invoke("NewUserEntered", user);
    });
}

function putMessage(usr, msg) {
    var messages = document.getElementById('messagesListFodaseSouRuimComNomes');

    var newMsg = document.createElement('li');
    newMsg.classList.add('.msg-li-item');
    newMsg.innerHTML = "[" + usr + "]: " + msg;

    messages.appendChild(newMsg);

    document.getElementsByClassName('chat')[0].scrollTop = document.getElementsByClassName('chat')[0].scrollHeight;
}

function sendMessage(event) {
    var message = document.getElementById('msgValue').value;

    connection.invoke("SendMessage", user, message).catch(function() { alert("acho q vc está desconctado troxa"); });

    document.getElementById('msgValue').value = "";

    event.preventDefault();
}

function stopConnection() {
    document.getElementById('username').disabled = false;
    document.getElementById('btnConnect').disabled = false;

    connection.stop();
}

// When something happens
connection.on("NewUserEntered", function(usr) { putMessage("Vigia", "Novo usuário entrou: " + usr); });
connection.on("ReceiveMsg", putMessage);
connection.on("UserDisconnected", function(usr) { putMessage("Vigia", "USUÁRIO DESCONECTADO ou EXPULSO: " + usr); });

// Commands Area
connection.on("CmdOnlineRequest", function(users) { 
    for(var i = 0; i < users.length; i++) {
        putMessage("Vigia", "USUÁRIO ONLINE: " + users[i]);
    }
});

connection.on("CmdClearRequest", function() {
    var messages = document.getElementById('messagesListFodaseSouRuimComNomes');
    messages.innerHTML = '';
});

connection.on("CmdBanRequest", function(usr) {
    var psswd = prompt("qual é a senha seu merda??? ");
    connection.invoke("BanResponse", usr, psswd);
});

connection.on("BanResponse", function() { alert("Você foi expulso! Seu trouxa!"); stopConnection(); });

// Errors or not-allowed-somethin
connection.on("AlreadyOnline", function() { alert("Já existe um usuário com o mesmo nome que você online!"); stopConnection();});
connection.on("NotAuthorized", function() { alert("nao autorizando otaro!"); });