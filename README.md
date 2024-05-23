
# Net.SimpleBlog

Net.SimpleBlog é uma plataforma de blog simples desenvolvida como parte de um processo seletivo. A aplicação permite que usuários autenticados criem, editem e excluam suas próprias postagens. Inclui também funcionalidades para visualizar postagens e notificações em tempo real usando WebSockets.




## Funcionalidades

- Autenticação e autorização de usuários
- Operações CRUD para postagens de blog
- Notificações em tempo real para novas postagens
- Seguindo os princípios SOLID e utilizando Entity Framework para manipulação de dados


## Começando

### Pré-requisitos
- .NET 6 SDK
- Docker

### Executando o Projeto
1. Iniciar o Banco de Dados
O projeto utiliza um banco de dados que precisa ser iniciado usando Docker. Navegue até a raiz do projeto e execute o seguinte comando:

```bash
 docker-compose up
```
2. Aplicar Migrações
Após iniciar o banco de dados, aplique as migrações do Entity Framework para configurar o esquema do banco de dados. Execute o seguinte comando:

```bash
dotnet ef database update --project src/Net.SimpleBlog.Api
```

3. Executar a Aplicação

Agora você pode executar a aplicação. Use o seguinte comando:
```bash
dotnet run --project src/Net.SimpleBlog.Api

```


## Rodando os testes

O projeto possui teste de Unidade, Integração e E2E. Para rodar  os testes E2E, certifique-se de que os serviços necessários estão em execução. Navegue até o diretório Net.SimpleBlog/tests/Net.SimpleBlog.E2ETests e execute o seguinte comando:

```bash
docker-compose up
```

Depois, execute os testes 

```bash
dotnet test
```

## Uso

### Registrar um Usuário

Para começar a usar a aplicação, você precisa registrar um usuário. Use o endpoint /Users para criar um novo usuário.

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

### Autenticação

Autentique-se usando o endpoint /users/authenticate para obter um token JWT.

Exemplo de payload:

```javascript
{
  "email": "john.doe@example.com",
  "password": "SecurePassword123!"
}

```

A resposta conterá um token que você precisa incluir no cabeçalho Authorization para as requisições subsequentes.

### Operações CRUD para Postagens

Uma vez autenticado, você pode usar os seguintes endpoints para gerenciar postagens:

- Criar Postagem: POST /posts
Exemplo de payload:

```javascript
{
  "title": "Minha Primeira Postagem",
  "content": "Este é o conteúdo da minha primeira postagem."
}
```

- Obter Postagem por ID: GET /posts/{id}


- Obter Postagem por ID: GET /posts/{id}
Exemplo de payload:

```javascript
{
  "title": "Título Atualizado",
  "content": "Conteúdo atualizado."
}
```

- Excluir Postagem: DELETE /posts/{id}

- Visualização de Postagens
Usuários não autenticados podem visualizar postagens usando o endpoint GET /posts.

### Conectando-se ao WebSocket
Para receber notificações em tempo real sobre novas postagens, você pode se conectar ao WebSocket da aplicação. Siga os passos abaixo para se conectar usando o WebSocket King:

- Acesse o site WebSocket King.
- No campo URL, insira o endereço wss://localhost:7097/ws.
- Clique em Connect para se conectar ao WebSocket.
Agora, você receberá notificações em tempo real sempre que uma nova postagem for criada.

## Conclusão
Este projeto demonstra uma plataforma de blog simples, mas funcional, com autenticação de usuários, gerenciamento de postagens e notificações em tempo real. Siga as instruções acima para configurar, executar e testar a aplicação. Aproveite o uso do Net.SimpleBlog!