document.getElementById('vigiaAlerts').checked = true;

function changeTheme() {
    var header = document.getElementsByClassName('main-header')[0];
    var connectButton = document.getElementsByClassName('connectButton')[0];
    var input = document.getElementById('inputName');
    var online = document.getElementsByClassName('onlineUsers')[0];
    var chat = document.getElementsByClassName('chat')[0];

    if (document.getElementById('darkThemeCheck').checked) {
        document.getElementsByClassName('main')[0].style.color = 'white';
        
        header.style.backgroundColor = 'rgba(40, 40, 40, 0.8)';
        header.style.border = 'solid 10px rgb(50, 50, 50)';
    
        connectButton.style.backgroundColor = 'rgba(40, 40, 40)';
        connectButton.style.color = 'white';
        connectButton.style.border = 'none';
    
        input.style.backgroundColor = 'rgba(40, 40, 40, 0.8)'
        input.style.borderColor = 'white';
        input.style.color = 'white';
    
        online.style.backgroundColor = 'rgba(40, 40, 40, 0.8)';
        online.style.border = 'solid 10px rgb(50, 50, 50)';
    
        chat.style.backgroundColor = 'rgba(40, 40, 40, 0.8)';
        chat.style.border = 'solid 10px rgb(50, 50, 50)';
        
        document.getElementById('inputMsg').style.backgroundColor = 'rgba(40, 40, 40, 0.8)';
        document.getElementById('btnSendMsg').style.backgroundColor = 'rgba(40, 40, 40, 0.8)';

        document.getElementById('inputMsg').style.border = 'solid rgb(50, 50, 50) 5px';
        document.getElementById('btnSendMsg').style.border = 'solid rgb(50, 50, 50) 5px';
    
        document.getElementById('btnSendMsg').style.color = 'white';
        document.getElementById('inputMsg').style.color = 'white';
    }
    else {
        document.getElementsByClassName('main')[0].style.color = 'black';

        header.style.backgroundColor = 'rgba(255, 255, 255, 0.9)';
        header.style.border = 'solid 10px black;';
    
        connectButton.style.backgroundColor = 'rgba(255, 255, 255, 0.9)';
        connectButton.style.color = 'black';
        connectButton.style.border = 'solid 3px black';
    
        input.style.backgroundColor = 'rgba(255, 255, 255, 0.9)'
        input.style.borderColor = 'black';
        input.style.color = 'black';
    
        online.style.backgroundColor = 'rgba(255, 255, 255, 0.9)';
        online.style.border = '';
    
        chat.style.backgroundColor = 'rgba(255, 255, 255, 0.9)';
        chat.style.border = 'solid black 5px';
        
        document.getElementById('inputMsg').style.backgroundColor = 'rgba(255, 255, 255, 0.9)';

        document.getElementById('inputMsg').style.border = 'solid black 5px';
        document.getElementById('btnSendMsg').style.backgroundColor = 'rgba(255, 255, 255, 0.9)';


        document.getElementById('inputMsg').style.color = 'black';
        document.getElementById('btnSendMsg').style.color = 'black';
    }
}