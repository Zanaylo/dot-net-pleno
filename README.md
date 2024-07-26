# **dot-net-pleno**

Este repositório tem por finalidade, disponibilizar conteúdo para execução da avaliação para desenvolvedor pleno utilizando .Net 8 e SQL Server. 
A descrição da avaliação está descrita em nosso [Wiki](https://github.com/StallosTecnologia/dot-net-pleno/wiki "Wiki").

# **Lembretes**

1. Realize o Update-database

2. Configure a string de conexão

3. Defina no `secret.json` ou no `appsettings.json` o seguinte padrão:

    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "exemplo conexão"
      },
      "Jwt": {
        "SecretKey": "exemplo secret key"
      },
      "UserCredentials": {
        "ClientId": "exemplo UserName",
        "ClientSecret": "exemplo Password",
        "RosterId": "autenticação roster client id",
        "RosterSecret": "autenticação roster client secret",
        "RosterXApi": "Xapi-key roster"
      }
    }
    ```

> **Nota:** `UserCredentials` é opcional. Basta criar no banco de dados um usuário para autenticação básica e autenticação na API do Roster.
