
# Net.SimpleBlog

Net.SimpleBlog � uma plataforma de blog simples desenvolvida como parte de um processo seletivo. A aplica��o permite que usu�rios autenticados criem, editem e excluam suas pr�prias postagens. Inclui tamb�m funcionalidades para visualizar postagens e notifica��es em tempo real usando WebSockets.




## Funcionalidades

- Autentica��o e autoriza��o de usu�rios
- Opera��es CRUD para postagens de blog
- Notifica��es em tempo real para novas postagens
- Seguindo os princ�pios SOLID e utilizando Entity Framework para manipula��o de dados


## Come�ando

### Pr�-requisitos
- .NET 6 SDK
- Docker

### Executando o Projeto
1. Iniciar o Banco de Dados
O projeto utiliza um banco de dados que precisa ser iniciado usando Docker. Navegue at� a raiz do projeto e execute o seguinte comando:

```bash
 docker-compose up
```
2. Aplicar Migra��es
Ap�s iniciar o banco de dados, aplique as migra��es do Entity Framework para configurar o esquema do banco de dados. Execute o seguinte comando:

```bash
dotnet ef database update --project src/Net.SimpleBlog.Api
```

3. Executar a Aplica��o

Agora voc� pode executar a aplica��o. Use o seguinte comando:
```bash
dotnet run --project src/Net.SimpleBlog.Api

```


## Rodando os testes

O projeto possui teste de Unidade, Integra��o e E2E. Para rodar  os testes E2E, certifique-se de que os servi�os necess�rios est�o em execu��o. Navegue at� o diret�rio Net.SimpleBlog/tests/Net.SimpleBlog.E2ETests e execute o seguinte comando:

```bash
docker-compose up
```

Depois, execute os testes 

```bash
dotnet test
```

## Uso

### Registrar um Usu�rio

Para come�ar a usar a aplica��o, voc� precisa registrar um usu�rio. Use o endpoint /Users para criar um novo usu�rio.

Exemplo de payload:

```javascript
{
  "name": "John Doe",
  "email": "john.doe@example.com",
  "password": "SecurePassword123!",
  "phone": "(12) 98765-7890",
  "cpf": "123.456.789-00",
  "rg": "MG1234567",
  "dateOfBirth": "1990-01-01",
  "is_active": true
}

```

### Autentica��o

Autentique-se usando o endpoint /users/authenticate para obter um token JWT.

Exemplo de payload:

```javascript
{
  "email": "john.doe@example.com",
  "password": "SecurePassword123!"
}

```

A resposta conter� um token que voc� precisa incluir no cabe�alho Authorization para as requisi��es subsequentes.

### Opera��es CRUD para Postagens

Uma vez autenticado, voc� pode usar os seguintes endpoints para gerenciar postagens:

- Criar Postagem: POST /posts
Exemplo de payload:

```javascript
{
  "title": "Minha Primeira Postagem",
  "content": "Este � o conte�do da minha primeira postagem."
}
```

- Obter Postagem por ID: GET /posts/{id}


- Obter Postagem por ID: GET /posts/{id}
Exemplo de payload:

```javascript
{
  "title": "T�tulo Atualizado",
  "content": "Conte�do atualizado."
}
```

- Excluir Postagem: DELETE /posts/{id}

- Visualiza��o de Postagens
Usu�rios n�o autenticados podem visualizar postagens usando o endpoint GET /posts.

### Conectando-se ao WebSocket
Para receber notifica��es em tempo real sobre novas postagens, voc� pode se conectar ao WebSocket da aplica��o. Siga os passos abaixo para se conectar usando o WebSocket King:

- Acesse o site WebSocket King.
- No campo URL, insira o endere�o wss://localhost:7097/ws.
- Clique em Connect para se conectar ao WebSocket.
Agora, voc� receber� notifica��es em tempo real sempre que uma nova postagem for criada.

## Conclus�o
Este projeto demonstra uma plataforma de blog simples, mas funcional, com autentica��o de usu�rios, gerenciamento de postagens e notifica��es em tempo real. Siga as instru��es acima para configurar, executar e testar a aplica��o. Aproveite o uso do Net.SimpleBlog!