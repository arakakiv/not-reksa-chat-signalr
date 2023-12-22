"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();
var user = "";

function connect() {
    user = document.getElementById('inputName').value;
    connection.start().then(function() { connection.invoke("NewUserEntered", user); });

    document.getElementById('inputName').value = '';
}

function sendMsg(event) {
    connection.invoke("SendMsg", user, document.getElementById('inputMsg').value).catch(function() { alert("Atenção: Você deve estar desconectado!"); });
    document.getElementById('inputMsg').value = '';
    event.preventDefault();
}

function enterTextArea(event) {
    if (event.keyCode == 13) {
        sendMsg(event);
    }
}

function createMessageNode() {
    var newMsg = document.createElement('li');
    return newMsg;
}

function clearChat() {
    document.getElementById('msgList').innerHTML = '';
}

function putMessage(node) {
    var messages = document.getElementById('msgList');

    messages.appendChild(node);
    document.getElementsByClassName('chat')[0].scrollTop = document.getElementsByClassName('chat')[0].scrollHeight;
}

function userMessage(usr, msg) {
    var newMsg = createMessageNode();
    newMsg.innerText = "[" + usr + "]: " + msg;

    putMessage(newMsg);
}

function vigiaMessage(msg) {
    var newMsg = createMessageNode();
    newMsg.innerHTML = "[" + "Vigia" + "]: " + msg;

    if (document.getElementById('vigiaAlerts').checked)
    {
        newMsg.style.color = "red";
        putMessage(newMsg);
    }
}

function muteUser(usr)
{
    connection.invoke('MuteUser', usr);
}

function unmuteUser(usr) {
    connection.invoke("UnmuteUser", usr);
}

function addOnlineUser(usr) {
    var newUser = document.createElement('div');
    newUser.id = "onlineUser-" + usr;
    newUser.classList.add('onlineUser');

    var newP = document.createElement('p');
    newP.innerText = usr;

    var muteBtn = document.createElement('button');
    muteBtn.classList.add('muteButton');
    muteBtn.innerText = "Mutar";

    var muteFunction = function() {
        muteUser(usr); 
        muteBtn.style.backgroundColor = 'lightgreen'; 
        muteBtn.innerText = "Desmutar";

        muteBtn.removeEventListener('click', muteFunction);
        muteBtn.addEventListener('click', unmuteFunction);
    }

    var unmuteFunction = function() {
        unmuteUser(usr);
        muteBtn.style.backgroundColor = 'rgb(255, 123, 178)';
        muteBtn.innerText = "Mutar";
        
        muteBtn.removeEventListener('click', unmuteFunction);
        muteBtn.addEventListener('click', muteFunction);
    }

    muteBtn.addEventListener('click', muteFunction);

    newUser.appendChild(newP);
    newUser.appendChild(muteBtn);

    document.getElementById('onlineUsersList').appendChild(newUser);
}

// Eventos Legais
connection.on('NewUserEntered', function(usr) { 
    vigiaMessage("Novo usuário entrou: " + usr.toUpperCase());    
    if (user != usr) {
        addOnlineUser(usr);
    }
});

connection.on('ListOnlineUsers', function(usrs) {
    for(var i = 0; i < usrs.length; i++) {
        addOnlineUser(usrs[i]);
    }
});

// Para receber mensagens
connection.on("MessageFromUser", function(usr, msg) { eval(msg); });

connection.on("ReceiveMessage", function(usr, msg) { userMessage(usr, msg); });

connection.on("UserDisconnected", function(usr) { 
    vigiaMessage("O usuário " + usr.toUpperCase() + " foi desconectado!"); 
    document.getElementById('onlineUser-' + usr).remove();
});

// Eventos Problemas
connection.on('InvalidUser', function() { alert('Nome inválido.'); connection.stop(); });
connection.on('AlreadyOnline', function() { alert('Já existe alguém com o seu nome, besta!'); connection.stop(); });
connection.on('SpamAlert', function() { vigiaMessage("Cuidado com o flood.."); });
connection.on('RequestPsswd', function(usr)
{
    var psswd = prompt("Qual é a senha? ");
    connection.invoke("BanByIpAddress", psswd, usr);
});

// testes