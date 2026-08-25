# 🏦 Simulador Bancário API & Web Client (Monorepo)

Projeto de desenvolvimento full-stack desenvolvido para estudar aprofundadamente arquitetura de APIs RESTful em C# (.NET), regras de domínio financeiro, encapsulamento de dados, controle de CORS e integração com Front-End moderno.

---

## 🚀 Tecnologias e Conceitos Aplicados

* **Back-End:** C#, .NET (ASP.NET Core Web API), Programação Orientada a Objetos (POO), Princípios de Encapsulamento (`private set`), Tratamento de Exceções (`try/catch` com Erros HTTP 400/404).
* **Front-End:** HTML5, JavaScript Moderno (`async/await`, `fetch API`), CSS3, Framework Bootstrap 5 para UI Responsiva.
* **Arquitetura & Segurança:** 
  * Estrutura **Monorepo** integrando API e telas estáticas.
  * Validação de regras de negócio estritamente no Back-End (o Front-End atua apenas como camada de apresentação).
  * Gestão de CORS restrito e DTOs (Data Transfer Objects) para segurança de entrada de dados.

---

## ⚙️ Regras de Negócio do Sistema

1. **Criação de Conta:** Exige obrigatoriamente Nome (Titular) e CPF, gerando um identificador único de chave criptográfica (`Guid`) e iniciando com saldo zerado.
2. **Transações Seguras:** 
   * **Depósitos:** Validados para aceitar apenas valores estritamente maiores que zero.
   * **Saques:** Protegidos contra saldo insuficiente e valores negativos (retornando erro HTTP 400 controlado).
3. **Auditoria e Extrato:** Todas as movimentações (Depósitos e Saques) geram objetos do tipo `Transacao`, gravando o carimbo de data/hora exato (`DateTime.Now`) para formar o extrato imutável da conta.
4. **Painel Administrativo:** Permite a listagem em tempo real de todas as contas do sistema e a auditoria detalhada de saldos e históricos por busca de CPF/Nome.

---

## 🛠️ Como Executar o Projeto Localmente

### Pré-requisitos
* Ter o [.NET SDK](https://dotnet.microsoft.com/) instalado na sua máquina.

### 1. Clonar e Rodar o Back-End (API)
Abra o terminal na pasta raiz do projeto e execute os comandos:
```bash
# Restaura as dependências e compila o projeto
dotnet restore

# Executa o servidor C#
dotnet run