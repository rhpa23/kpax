# kpax
TI Projects Configuration Manager


Usuário/GC
Nome, senha, administrador

Cadastro do projeto
Nome, email, Acronimo, template de email, Usuários (nome, senha)

Planejador de baselines:
Nome, conteúdo, data, avisar por email


Configuração da baseline:
 1- Verificar integração continua no TeamCity/Jenkins (se Ok continua)
 2- git tag <nome_da_baseline>
 3- Atualizar aplicação Web remota <comando que builda e envia código atualizado para o servidor remoto>
4- gerar binários <comando de build>
5- Fazer upload dos binários para nuvem <acesso a nuvem>
 6- envio o email <Template de email com informações da disponibilização> 
7- salvar email no repositório de gestão. (evidência pode ser guardada no proprio sistema)
8- Atualizar a baseline como lançada no planejamento de baselines
9- Realizar auditoria de Release
10- Sincronizar os repositórios IA -> HP

Gerar baseline:
Sugerir proxima baseline planejada e lista das proximas
Ao clicar em 'Gerar' mostra se todas as tarefas configuradas foram realizadas


https://www.youtube.com/watch?v=F0Xs33UMLnA
www.asp.net/mvc/overview/security/create-an-aspnet-mvc-5-app-with-facebook-and-google-oauth2-and-openid-sign-on
https://confluence.jetbrains.com/display/TCD9/REST+API#RESTAPI-QueuedBuilds


