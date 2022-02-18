"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();
var user = "";

function enterTextArea(event) {
    if (event.keyCode == 13) {
        sendMessage(event);
    }
}

function connect() {
    if (document.getElementById('username').value == "") {
        alert('Nome inválido.');
        return;
    }

    user = document.getElementById('username').value;

    document.getElementById('username').disabled = true;
    document.getElementById('btnConnect').disabled = true;

    connection.start().then(function() {
        connection.invoke("NewUserEntered", user);
    });
}

function putMessage(usr, msg, isFromServer) {
    var messages = document.getElementById('messagesListFodaseSouRuimComNomes');

    var newMsg = document.createElement('li');
    newMsg.classList.add('.msg-li-item');
    newMsg.innerHTML = "[" + usr + "]: " + msg;

    if (isFromServer) {
        newMsg.style.color = "red";
    }

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
connection.on("NewUserEntered", function(usr) { putMessage("Vigia", "Novo usuário entrou -->" + usr, true);  });
connection.on("ReceiveMsg", putMessage);
connection.on("UserDisconnected", function(usr) { putMessage("Vigia", "USUÁRIO DESCONECTADO --> " + usr, true); });

// Commands Area
connection.on("CmdOnlineRequest", function(users) { 
    for(var i = 0; i < users.length; i++) {
        putMessage("Vigia", "USUÁRIO ONLINE: " + users[i], true);
    }
});

connection.on("CmdClearRequest", function() {
    var messages = document.getElementById('messagesListFodaseSouRuimComNomes');
    messages.innerHTML = '';
});

connection.on("CmdBanRequest", function(usr) {
    var psswd = prompt("pswd: ");
    connection.invoke("BanResponse", usr, psswd);
});

connection.on("BanResponse", function() { 
    putMessage("Vigia", "Você foi expulso! Seu trouxa!", true); 
    stopConnection(); 
});

connection.on("SomeoneBanned", function(usr) { 
    putMessage("Vigia", "O usuário " + usr + " foi expulso!", true);
})

connection.on("CmdHelpRequest", function() {
    putMessage("Vigia", "/help para ver todos os comandos.", true);
    putMessage("Vigia", "/online para ver os usuários online.", true);
    putMessage("Vigia", "/clear para limpar seu chat.", true);
    putMessage("Vigia", "/ban para expulsar um usuário (função administratitva).", true);
});

// Errors or not-allowed-something
connection.on("AlreadyOnline", function() { alert("Já existe um usuário com o mesmo nome que você online!"); stopConnection();});
connection.on("NotAllowedName", function() { alert("Nome não permitido."); stopConnection(); });
connection.on("NotAuthorized", function() { alert("Não autorizado!"); });
connection.on("UserNotFounded", function(usr) { alert("Usuário não encontrado: " + usr); });