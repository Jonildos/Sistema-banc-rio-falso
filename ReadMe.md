# 🏦 Simulador Bancário API & Web Client (Monorepo)

Projeto full-stack desenvolvido para estudar aprofundadamente arquitetura de APIs RESTful em C# (.NET), persistência de dados com ORM, regras de domínio financeiro, segurança de acesso, tratamento de estornos cruzados/bidirecionais e integração com Front-End moderno estilo *fintech*.

---

## 🚀 Tecnologias e Conceitos Aplicados

* **Back-End:** C#, .NET (ASP.NET Core Web API), Programação Orientada a Objetos (POO), Encapsulamento de domínio (`private set`).
* **Banco de Dados & ORM:** **Entity Framework Core**, **SQLite** (banco relacional em arquivo local com persistência imutável e gerenciamento de chaves estrangeiras).
* **Segurança de Acesso:** Hashing seguro de senhas utilizando a biblioteca **BCrypt** (`BCrypt.Net-Next`) para prevenção de vazamento de credenciais.
* **Front-End:** HTML5, JavaScript Moderno (`async/await`, `fetch API`), CSS3, Framework Bootstrap 5 para UI Responsiva e FontAwesome para ícones corporativos.
* **Arquitetura & Segurança:** 
  * Estrutura **Monorepo** unificando a API e os arquivos estáticos do front-end servidos diretamente pelo Kestrel (.NET).
  * Validação matemática rigorosa de CPF e unicidade de dados no banco (impedindo CPFs duplicados).
  * Persistência de sessão no lado do cliente (`sessionStorage`) para manter o usuário e o administrador autenticados após recarregamentos de página (`F5`).
  * Unificação da autenticação administrativa em arquivo único (`admin.html`), eliminando redundâncias de páginas de login isoladas.
  * Uso de DTOs (*Data Transfer Objects*) para desacoplamento e segurança na entrada de dados via corpo de requisição (`[FromBody]`).

---

## ⚙️ Regras de Negócio do Sistema

1. **Criação de Conta:** 
   * Exige obrigatoriamente Nome (Titular), CPF e Senha de acesso. Gera um identificador universal (`Guid`), um hash seguro de senha via BCrypt e uma **Chave Única de transferência automática** (`ChavePix` estilo PIX).
   * **Validação Rígida:** O sistema valida tanto a integridade matemática dos dígitos do CPF quanto a unicidade do cadastro no banco de dados.
2. **Transações e Transferências Seguras:** 
   * **Depósitos e Saques:** Protegidos contra saldos negativos, valores zerados e restrições de saldo em caso de retiradas indevidas.
   * **Transferência por Chave Única:** Realiza o envio de valores de forma atômica e simultânea entre as contas envolvidas, gerando rastreabilidade cruzada (`ContaDestinoId` e `TransferenciaId`).
3. **Auditoria, Extrato e Estorno Automatizado:** 
   * Todas as movimentações geram registros relacionais na tabela `Transacoes`, gravando data, hora e tipo.
   * **Estorno Unificado pelo Admin:** Ao solicitar o estorno de uma transferência em qualquer uma das pontas do extrato, o sistema valida ativamente se o destinatário ainda possui saldo disponível, retira o valor de lá, **devolve integralmente para a conta de quem enviou** e remove ambas as pontas do histórico do banco de forma automática.
4. **Painel Administrativo Restrito (`admin.html`):** 
   * Conta com fluxo integrado de autenticação master.
   * Permite listagem geral de contas, busca dinâmica por nome/cpf, auditoria de extrato detalhado por conta, ajuste administrativo de saldo (juros, dívidas ou empréstimos) e acionamento de estornos com um único clique.

---

## 🛠️ Como Executar o Projeto Localmente

### Pré-requisitos
* Ter o [.NET SDK](https://dotnet.microsoft.com/) instalado na sua máquina.
* Extensão **Live Server** (no VS Code) recomendada para servir o Front-End de forma fluida.

### Passo a Passo para Rodar:

1. **Navegue até a pasta raiz do projeto no terminal:**
   ```bash
   cd Sistema-banc-rio-falso
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