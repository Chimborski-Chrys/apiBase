# Como Autenticar no Swagger (Resolver Erro 401)

## O Problema: Erro 401 Unauthorized

Quando você tenta usar **PUT /api/users/{id}** ou **DELETE /api/users/{id}** e recebe erro **401**, significa que você não está autenticado.

---

## Solução: Passo a Passo

### 1. Registrar ou Fazer Login

#### Opção A: Registrar Novo Usuário

No Swagger, expanda **POST /api/auth/register**:

1. Clique em **"Try it out"**
2. Cole este JSON:

```json
{
  "name": "Admin",
  "email": "admin@exemplo.com",
  "password": "senha123",
  "confirmPassword": "senha123",
  "role": "Admin"
}
```

3. Clique em **"Execute"**

#### Opção B: Fazer Login (se já tem usuário)

Expanda **POST /api/auth/login**:

```json
{
  "email": "admin@exemplo.com",
  "password": "senha123"
}
```

---

### 2. Copiar o Token JWT

Na resposta, você verá algo assim:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiI4ZjE2...",
  "email": "admin@exemplo.com",
  "name": "Admin",
  "role": "Admin",
  "expiresAt": "2026-01-13T12:00:00Z"
}
```

**Copie APENAS o valor do campo `token`** (o texto gigante que começa com eyJ...)

---

### 3. Autenticar no Swagger

1. No **topo da página do Swagger**, procure o botão **"Authorize"** 🔓
2. Clique nele
3. Cole o token no campo **Value** (sem aspas, sem "Bearer", só o token)
4. Clique em **"Authorize"**
5. Clique em **"Close"**

Agora o cadeado 🔒 deve aparecer como **fechado** nos endpoints protegidos!

---

### 4. Testar o Update

Agora expanda **PUT /api/users/{id}**:

1. Cole o **ID** de um usuário existente no campo `id`
2. Clique em **"Try it out"**
3. Cole este JSON:

```json
{
  "name": "Nome Atualizado",
  "email": "email@exemplo.com",
  "passwordHash": "novo_hash_opcional",
  "role": "Admin",
  "isActive": true
}
```

**Nota:** O campo `passwordHash` é opcional. Se não quiser alterar a senha, você pode:
- Omitir o campo completamente, OU
- Passar `null`, OU
- Passar uma string vazia `""`

4. Clique em **"Execute"**

**Resposta esperada:** `200 OK` com os dados atualizados!

---

## Formato Correto do JSON para Update

### ✅ Atualizar APENAS o Email:

```json
{
  "email": "novoemail@exemplo.com"
}
```

### ✅ Atualizar APENAS o Nome:

```json
{
  "name": "João Silva Atualizado"
}
```

### ✅ Atualizar Email e Role:

```json
{
  "email": "joao@exemplo.com",
  "role": "Admin"
}
```

### ✅ Atualizar Senha:

```json
{
  "passwordHash": "novo_hash_bcrypt"
}
```

### ✅ Atualizar Múltiplos Campos:

```json
{
  "name": "João Silva",
  "email": "joao@exemplo.com",
  "role": "Admin",
  "isActive": false
}
```

### ✅ Atualizar Todos os Campos:

```json
{
  "name": "João Silva Completo",
  "email": "joao@exemplo.com",
  "passwordHash": "novo_hash",
  "role": "User",
  "isActive": true
}
```

### Campos Disponíveis (TODOS OPCIONAIS):

- **name** (opcional): Nome do usuário
- **email** (opcional): Email do usuário
- **passwordHash** (opcional): Hash da senha
- **role** (opcional): "Admin" ou "User"
- **isActive** (opcional): true ou false

**Importante:** Você pode enviar apenas os campos que deseja atualizar. Os campos não enviados permanecem inalterados.

### ❌ ERRADO (Não incluir id, createdAt, updatedAt):

```json
{
  "id": "uuid-aqui",
  "createdAt": "...",
  "updatedAt": "..."
}
```

---

## Verificar se Está Autenticado

### Sinais de que você ESTÁ autenticado:

- ✅ Botão "Authorize" mostra cadeado **fechado** 🔒
- ✅ Endpoints protegidos mostram cadeado **fechado** 🔒
- ✅ PUT e DELETE retornam **200** ou **204**

### Sinais de que você NÃO está autenticado:

- ❌ Botão "Authorize" mostra cadeado **aberto** 🔓
- ❌ Endpoints protegidos retornam **401 Unauthorized**
- ❌ Você não clicou em "Authorize" após copiar o token

---

## Token Expirado

Os tokens JWT expiram em **24 horas** (1440 minutos).

Se você receber erro 401 mesmo após autenticar:

1. Faça login novamente (**POST /api/auth/login**)
2. Copie o **novo token**
3. Clique em **"Authorize"** novamente
4. Cole o novo token

---

## Resumo Visual

```
┌─────────────────────────────────────────────────┐
│ 1. POST /api/auth/login                         │
│    Email: admin@exemplo.com                     │
│    Password: senha123                           │
└─────────────────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────┐
│ 2. COPIAR o token da resposta                   │
│    token: "eyJhbGciOiJI..."                     │
└─────────────────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────┐
│ 3. Clicar em "Authorize" 🔓 no topo            │
│    Colar o token                                │
│    Clicar em "Authorize"                        │
└─────────────────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────┐
│ 4. Agora PUT /api/users/{id} funciona! ✅      │
│    {                                            │
│      "name": "João Silva",                      │
│      "email": "joao@exemplo.com",               │
│      "passwordHash": "novo_hash",               │
│      "role": "Admin",                           │
│      "isActive": true                           │
│    }                                            │
└─────────────────────────────────────────────────┘
```

---

## Testando via cURL (Alternativa)

Se preferir testar via linha de comando:

### 1. Login

```bash
curl -X POST https://localhost:7000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@exemplo.com","password":"senha123"}'
```

### 2. Update (com token)

```bash
curl -X PUT https://localhost:7000/api/users/UUID_AQUI \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -d '{
    "name": "João Silva",
    "email": "joao@exemplo.com",
    "passwordHash": "novo_hash",
    "role": "Admin",
    "isActive": true
  }'
```

**Importante:**
- Substitua `UUID_AQUI` pelo ID do usuário
- Substitua `SEU_TOKEN_AQUI` pelo token que você copiou
- O campo `passwordHash` é opcional (pode omitir ou deixar vazio)

---

**Agora você consegue atualizar usuários! 🎉**
