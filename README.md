# kpax
TI Projects Configuration Manager


Usu�rio/GC
Nome, senha, administrador

Cadastro do projeto
Nome, email, Acronimo, template de email, Usu�rios (nome, senha)

Planejador de baselines:
Nome, conte�do, data, avisar por email


Configura��o da baseline:
 1- Verificar integra��o continua no TeamCity/Jenkins (se Ok continua)
 2- git tag <nome_da_baseline>
 3- Atualizar aplica��o Web remota <comando que builda e envia c�digo atualizado para o servidor remoto>
4- gerar bin�rios <comando de build>
5- Fazer upload dos bin�rios para nuvem <acesso a nuvem>
 6- envio o email <Template de email com informa��es da disponibiliza��o> 
7- salvar email no reposit�rio de gest�o. (evid�ncia pode ser guardada no proprio sistema)
8- Atualizar a baseline como lan�ada no planejamento de baselines
9- Realizar auditoria de Release
10- Sincronizar os reposit�rios IA -> HP

Gerar baseline:
Sugerir proxima baseline planejada e lista das proximas
Ao clicar em 'Gerar' mostra se todas as tarefas configuradas foram realizadas


https://www.youtube.com/watch?v=F0Xs33UMLnA
www.asp.net/mvc/overview/security/create-an-aspnet-mvc-5-app-with-facebook-and-google-oauth2-and-openid-sign-on
https://confluence.jetbrains.com/display/TCD9/REST+API#RESTAPI-QueuedBuilds


