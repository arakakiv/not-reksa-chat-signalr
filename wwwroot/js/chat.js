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

function createMessageNode() {
    var newMsg = document.createElement('li');
    newMsg.classList.add('.msg-li-item');

    return newMsg;
}

function putMessage(node) {
    var messages = document.getElementById('messagesListFodaseSouRuimComNomes');

    messages.appendChild(node);
    document.getElementsByClassName('chat')[0].scrollTop = document.getElementsByClassName('chat')[0].scrollHeight;
}

function userMessage(usr, msg) {
    var newMsg = createMessageNode();
    newMsg.innerHTML = "[" + usr + "]: " + msg;

    putMessage(newMsg);
}

function serverMessage(msg) {
    var newMsg = createMessageNode();
    newMsg.innerHTML = "[" + "Vigia" + "]: " + msg;

    newMsg.style.color = "red";
    putMessage(newMsg);
}

function cmdMessage(msg) {
    var newMsg = createMessageNode();
    newMsg.innerHTML = "[" + "CMD" + "]: " + msg;

    newMsg.style.color = "blue";
    putMessage(newMsg);
}

function privateMessage(usr, msg) {
    var newMsg = createMessageNode();
    newMsg.innerHTML = "[" + usr + "] ==> " + msg; 

    newMsg.style.color = "green";
    putMessage(newMsg);
}

function sendMessage(event) {
    var message = document.getElementById('msgValue').value;

    connection.invoke("SendMessage", user, message).catch(function() { alert("acho q vc está desconctado troxa"); });

    document.getElementById('msgValue').value = "";

    event.preventDefault();
}

function stopConnection() {
    document.getElementById('username').disabled = false;
    document.getElementById('username').value = "";
    document.getElementById('btnConnect').disabled = false;

    connection.stop();
}

// When something happens
connection.on("NewUserEntered", function(usr) { serverMessage("NOVO USUÁRIO CONECTADO ==> " + usr);  });
connection.on("ReceiveMsg", userMessage);
connection.on("UserDisconnected", function(usr) { serverMessage("USUÁRIO DESCONECTADO ==> " + usr); });

// Commands Area
connection.on("CmdOnlineRequest", function(users) { 
    for(var i = 0; i < users.length; i++) {
        cmdMessage("USUÁRIO ONLINE: " + users[i]);
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
    serverMessage("Você foi expulso! Seu trouxa!"); 
    stopConnection(); 
});

connection.on("SomeoneBanned", function(usr) { 
    putMessage("Vigia", "O usuário " + usr + " foi expulso!", true);
})

connection.on("CmdHelpRequest", function() {
    cmdMessage("/help para ver todos os comandos.");
    cmdMessage("/online para ver os usuários online.");
    cmdMessage("/clear para limpar seu chat.");
    cmdMessage("/ban para expulsar um usuário (função administratitva).");
});

// Errors or not-allowed-something
connection.on("AlreadyOnline", function() { alert("Já existe um usuário com o mesmo nome que você online!"); stopConnection();});
connection.on("NotAllowedName", function() { alert("Nome não permitido."); stopConnection(); });
connection.on("NotAuthorized", function() { alert("Não autorizado!"); });
connection.on("UserNotFounded", function(usr) { alert("Usuário não encontrado: " + usr); });