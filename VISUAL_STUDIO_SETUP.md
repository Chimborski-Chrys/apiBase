# Configuração do Visual Studio

## Definir Projeto de Inicialização

Quando você abre o `BaseApi.sln` no Visual Studio, o projeto **BaseApi.Api** deve ser o projeto de inicialização (startup project).

### Método 1: Via Solution Explorer (Recomendado)

1. Abra a solução `BaseApi.sln` no Visual Studio
2. No **Solution Explorer** (lado direito), localize o projeto `BaseApi.Api`
3. **Clique com o botão direito** em `BaseApi.Api`
4. Selecione **"Set as Startup Project"** (Definir como Projeto de Inicialização)
5. O projeto `BaseApi.Api` ficará em **negrito** indicando que é o projeto de inicialização

### Método 2: Via Menu

1. Clique no projeto `BaseApi.Api` no Solution Explorer
2. Menu superior: **Project** → **Set as Startup Project**

### Verificação

Após configurar, você deverá ver:

- **BaseApi.Api** aparece em **negrito** no Solution Explorer
- No menu superior, ao lado do botão ▶️ (Play), deve aparecer **"BaseApi.Api"**
- Ao pressionar **F5** ou clicar em ▶️, a API será executada e o Swagger abrirá automaticamente

## Configurar Múltiplos Projetos de Inicialização (Opcional)

Se no futuro você tiver um projeto Frontend ou outros serviços:

1. Clique com botão direito na **Solution** (topo do Solution Explorer)
2. **Properties** → **Startup Project**
3. Selecione **"Multiple startup projects"**
4. Configure quais projetos devem iniciar e em que ordem

## Perfis de Execução (Launch Profiles)

O projeto já está configurado com 2 perfis:

### 1. HTTP (porta 5000)
```
http://localhost:5000
```

### 2. HTTPS (porta 7000) - Recomendado
```
https://localhost:7000
```

Para alternar entre os perfis:

1. Clique na **seta ao lado do botão ▶️** (Play) no menu superior
2. Selecione **"https"** ou **"http"**

## Atalhos Úteis

| Atalho | Ação |
|--------|------|
| **F5** | Executar com debug |
| **Ctrl + F5** | Executar sem debug |
| **Shift + F5** | Parar execução |
| **Ctrl + Shift + B** | Build da solução |
| **Ctrl + Alt + L** | Abrir Solution Explorer |

## Configuração de Debug

### appsettings.Development.json

O arquivo está configurado para mostrar logs detalhados durante o desenvolvimento:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### Breakpoints

- Clique na margem esquerda do código para adicionar breakpoints
- Execute com F5 para debug com breakpoints
- Execute com Ctrl+F5 para executar sem debug (mais rápido)

## Troubleshooting Visual Studio

### Problema: "Não consigo ver o projeto BaseApi.Api"

**Solução:**
1. Feche o Visual Studio
2. Exclua as pastas `.vs` (oculta) e `bin/obj` de todos os projetos
3. Reabra a solução

### Problema: "Erro ao compilar"

**Solução:**
1. Menu: **Build** → **Clean Solution**
2. Menu: **Build** → **Rebuild Solution**
3. Ou via linha de comando:
   ```bash
   dotnet clean
   dotnet build
   ```

### Problema: "User Secrets não configurado"

**Solução:**
1. Clique com botão direito em `BaseApi.Api`
2. Selecione **"Manage User Secrets"**
3. Adicione a connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=baseapi_dev;Username=postgres;Password=postgres"
     }
   }
   ```
4. Salve o arquivo

### Problema: "Porta já em uso"

**Solução 1:** Altere as portas em `Properties/launchSettings.json`:
```json
"applicationUrl": "https://localhost:7001;http://localhost:5001"
```

**Solução 2:** Encontre e mate o processo na porta:
```powershell
# Encontrar processo
netstat -ano | findstr :5000

# Matar processo (substitua PID)
taskkill /PID <numero_do_pid> /F
```

## Extensões Recomendadas

Para melhor experiência de desenvolvimento:

1. **C# Dev Kit** (Microsoft)
2. **NuGet Package Manager** (já incluído)
3. **Entity Framework Core Power Tools**
4. **REST Client** (para testar API)
5. **Markdown Editor** (para editar README)

## Estrutura no Solution Explorer

Você deverá ver:

```
Solution 'BaseApi' (4 projects)
├── src
    ├── BaseApi.Api          ← Este deve estar em NEGRITO
    ├── BaseApi.Application
    ├── BaseApi.Domain
    └── BaseApi.Infra
```

## Executando pela Primeira Vez

1. **Configurar User Secrets** (uma única vez):
   - Execute: `.\setup-dev.ps1`
   - OU configure manualmente via "Manage User Secrets"

2. **Iniciar PostgreSQL**:
   ```bash
   docker-compose up -d
   ```

3. **Aplicar Migrations** (uma única vez):
   - Abra **Package Manager Console** (menu Tools)
   - Execute:
     ```
     Add-Migration InitialCreate -Project BaseApi.Infra -StartupProject BaseApi.Api
     Update-Database -Project BaseApi.Infra -StartupProject BaseApi.Api
     ```

4. **Executar a API**:
   - Pressione **F5** ou clique em ▶️

5. **Acessar Swagger**:
   - O navegador abrirá automaticamente em `https://localhost:7000`

## Package Manager Console

Para usar Entity Framework via console no Visual Studio:

1. Menu: **Tools** → **NuGet Package Manager** → **Package Manager Console**
2. Selecione `BaseApi.Infra` como **Default project**
3. Execute comandos:
   ```
   Add-Migration NomeDaMigration
   Update-Database
   Remove-Migration
   ```

---

**Pronto! Agora você está configurado para desenvolver no Visual Studio 🚀**
