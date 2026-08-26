# 🏦 Simulador Bancário API & Web Client (Monorepo)

Projeto full-stack desenvolvido para estudar aprofundadamente arquitetura de APIs RESTful em C# (.NET), persistência de dados com ORM, regras de domínio financeiro, segurança de acesso e integração com Front-End moderno.

---

## 🚀 Tecnologias e Conceitos Aplicados

* **Back-End:** C#, .NET (ASP.NET Core Web API), Programação Orientada a Objetos (POO), Encapsulamento (`private set`).
* **Banco de Dados & ORM:** **Entity Framework Core**, **SQLite** (banco relacional em arquivo local com persistência imutável).
* **Front-End:** HTML5, JavaScript Moderno (`async/await`, `fetch API`), CSS3, Framework Bootstrap 5 para UI Responsiva.
* **Arquitetura & Segurança:** 
  * Estrutura **Monorepo** integrando API e arquivos estáticos servidos diretamente pelo Kestrel (.NET).
  * Validação de unicidade de dados no banco (CPF único por conta).
  * Persistência de sessão no lado do cliente (`sessionStorage`) para manter o usuário autenticado após recarregamentos de página (`F5`).
  * Controle de acesso restrito no painel administrativo via validação de credenciais de Administrador na API.
  * Uso de DTOs (*Data Transfer Objects*) para segurança de entrada de dados.

---

## ⚙️ Regras de Negócio do Sistema

1. **Criação de Conta:** 
   * Exige obrigatoriamente Nome (Titular) e CPF, gerando um identificador único (`Guid`), uma **Chave Única de transferência automática** (estilo chave PIX/cripto) e iniciando com saldo zerado.
   * **Unicidade:** O sistema valida rigidamente se o CPF já existe no banco de dados antes de autorizar a abertura de uma nova conta.
2. **Transações e Transferências Seguras:** 
   * **Depósitos:** Validados para aceitar apenas valores estritamente maiores que zero.
   * **Saques:** Protegidos contra saldo insuficiente e valores negativos (retornando erro HTTP 400 controlado).
   * **Transferência por Chave Única:** Permite enviar valores diretamente para outra conta informando apenas a chave de destino, realizando a transação de forma atômica no banco de dados.
3. **Auditoria e Extrato:** Todas as movimentações (Depósitos, Saques e Transferências) criam registros relacionais na tabela `Transacoes`, gravando data e hora exatas (`DateTime.Now`) vinculadas à respectiva conta.
4. **Painel Administrativo Restrito:** 
   * Requer login com credenciais de administrador validadas no banco de dados.
   * Permite a listagem em tempo real de todas as contas do sistema e a auditoria detalhada de saldos e históricos por busca de CPF ou Nome.

---

## 🛠️ Como Executar o Projeto Localmente

### Pré-requisitos
* Ter o [.NET SDK](https://dotnet.microsoft.com/) instalado na sua máquina.

### Passo a Passo para Rodar:

1. **Clone o repositório e navegue até a pasta raiz:**
   ```bash
   cd NomeDoRepositorio
   dotnet restore
   dotnet run

2. **Com o servidor rodando abra o seu navegador e acesse:**
    portal do cliente: http://localhost:5279/index.html
    painel administrativo: http://localhost:5279/admin-login.html
    credenciais: admin@bancofalso.com / 000.000.000-00 / admin123

### Créditos e agradecimentos:
Meu tio Rafael Araujo Soares - pela consultoria na configuração avançada do provedor de arquivos estáticos do ASP.NET Core, unificando o front-end estático ao ciclo de vida da api na mesma porta

Leonardo Resende - pelos direcionamentos valiosos sobre padrões de mercado

Sinueh - pelos apontamentos sobre separação de responsabilidades (service pattern) e arquitetura de rotas