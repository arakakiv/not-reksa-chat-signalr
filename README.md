# Chat feito com SignalR e Razor Pages (Server) e JavaScript (Client).
Acesse o webapp em: https://not-reksa-chat.herokuapp.com/

## Comandos:
- **/help**: lista todos os comandos.
- **/online**: lista todos os usuários ativos para quem solicita o comando.
- **/clear**: limpa o chat para quem solicita o comando.
- **/ban 'user'**: expulsa um usuário. É necessário que seja usada uma senha para tal (posteriormente, funções de moderadores serão criadas). 
- **/msg user 'msg'**: manda uma mensagem privada para um único usuário.
- **/mute 'user'**: silencia um usuário para quem solicita o comando.
- **/unmute 'user'**: desmuta um usuário que já estava mutado anteriormente para quem solicita o comando.

## Importante
O aplicativo também valida os dados de entrada, principalmente server side. Dentre algumas validações, existem a verificação de **XSS** *(Cross Site Scripting)*, buscando barrar a presença de tags html ou a execução de códigos javascript e também a possibilidade de, pelo console **DevTools** de cada browser, executar novas conexões e invocar comandos do server side.

Agora, um intervalo de um segundo deve ser seguido para mandar uma mensagem, prevenindo muito flood.
