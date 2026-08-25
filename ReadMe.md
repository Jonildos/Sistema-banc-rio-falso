# 🏦 Simulador Bancário API & Web Client (Monorepo)

Projeto full-stack desenvolvido para estudar aprofundadamente arquitetura de APIs RESTful em C# (.NET), persistência de dados com ORM, regras de domínio financeiro, segurança de acesso e integração com Front-End moderno.

---

## 🚀 Tecnologias e Conceitos Aplicados

* **Back-End:** C#, .NET (ASP.NET Core Web API), Programação Orientada a Objetos (POO), Encapsulamento (`private set`).
* **Banco de Dados & ORM:** **Entity Framework Core**, **SQLite** (banco relacional em arquivo local com persistência imutável).
* **Front-End:** HTML5, JavaScript Moderno (`async/await`, `fetch API`), CSS3, Framework Bootstrap 5 para UI Responsiva.
* **Arquitetura & Segurança:** 
  * Estrutura **Monorepo** integrando API e telas estáticas separadas (Portal do Cliente e Portal Admin).
  * Validação de unicidade de dados no banco (CPF único por conta).
  * Controle de acesso restrito no painel administrativo via validação de credenciais de Administrador na API e travamento de sessão (`sessionStorage`).
  * Uso de DTOs (*Data Transfer Objects*) para segurança de entrada de dados.

---

## ⚙️ Regras de Negócio do Sistema

1. **Criação de Conta:** 
   * Exige obrigatoriamente Nome (Titular) e CPF, gerando um identificador único (`Guid`) e iniciando com saldo zerado.
   * **Unicidade:** O sistema valida rigidamente se o CPF já existe no banco de dados antes de autorizar a abertura de uma nova conta.
2. **Transações Seguras:** 
   * **Depósitos:** Validados para aceitar apenas valores estritamente maiores que zero.
   * **Saques:** Protegidos contra saldo insuficiente e valores negativos (retornando erro HTTP 400 controlado).
3. **Auditoria e Extrato:** Todas as movimentações (Depósitos e Saques) criam registros relacionais na tabela `Transacoes`, gravando data e hora exatas (`DateTime.Now`) vinculadas à respectiva conta.
4. **Painel Administrativo Restrito:** 
   * Requer login com credenciais de administrador validadas no banco de dados.
   * Permite a listagem em tempo real de todas as contas do sistema e a auditoria detalhada de saldos e históricos por busca de CPF ou Nome.

---

## 🛠️ Como Executar o Projeto Localmente

### Pré-requisitos
* Ter o [.NET SDK](https://dotnet.microsoft.com/) instalado na sua máquina.

### 1. Clonar e Rodar o Back-End (API)
Abra o terminal na pasta raiz do projeto e execute os comandos:
```bash
# Restaura as dependências (incluindo o Entity Framework e SQLite)
dotnet restore

# Executa o servidor C# (O banco SQLite 'banco_bancofalso.db' será gerado automaticamente)
dotnet run