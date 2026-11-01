# Sistema de Reservas Escolar

![CI](https://github.com/IancFreitas/sistema-reservas-escolar/actions/workflows/ci.yml/badge.svg)
![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4)

Aplicação de console em C# para um colégio controlar a reserva de **salas** (vídeo, teatro, apresentação)
e **equipamentos** (notebooks, projetores e tablets) pelos funcionários.

> Projeto criado originalmente em 2020 como PIM (Projeto Integrado Multidisciplinar) da faculdade
> e reescrito em 2026 com arquitetura em camadas, testes automatizados e boas práticas de segurança.

## Funcionalidades

- Login com senha armazenada como hash **PBKDF2 + salt** (nenhuma senha em texto puro), com limite de tentativas
- Reserva e devolução de salas e equipamentos, com controle de quem reservou cada item
- Professores só devolvem as próprias reservas; administradores podem devolver qualquer uma
- Consulta das reservas ativas do usuário
- Cadastro, listagem e remoção de funcionários (somente administradores)
- Validação de todas as entradas: digitar algo inválido nunca derruba o programa

## Arquitetura

```
src/SistemaReservas
├── Domain/      Entidades (Resource, Room, Equipment, Employee) e OperationResult
├── Data/        SchoolDatabase: armazenamento em memória + dados de demonstração
├── Services/    Regras de negócio (AuthService, ReservationService, EmployeeService)
├── Security/    PasswordHasher (PBKDF2/SHA-256)
├── UI/          Menus de console (ConsoleApp, ConsoleInput)
└── Program.cs   Ponto de entrada
tests/SistemaReservas.Tests   Testes unitários com xUnit
```

A interface só lê e mostra dados; as regras ficam nos serviços, que retornam um `OperationResult`
com sucesso/falha e a mensagem para o usuário. Isso permite testar toda a lógica sem console
e trocar o armazenamento em memória por um banco de dados sem alterar as regras.

## Como executar

Pré-requisito: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

```bash
git clone https://github.com/IancFreitas/sistema-reservas-escolar.git
cd sistema-reservas-escolar
dotnet run --project src/SistemaReservas
```

Usuários de demonstração:

| Usuário     | Senha      | Perfil        |
|-------------|------------|---------------|
| `admin`     | `admin123` | Administrador |
| `professor` | `prof123`  | Professor     |

### Exemplo de uso

```
=== Sistema de Reservas | Colégio Vencer Sempre ===
Usuário: professor
Senha: *******

Bem-vindo(a), Professor de Exemplo!

--- Menu principal ---
  1 - Reservar sala
  2 - Reservar equipamento
  3 - Devolver item
  4 - Minhas reservas
  0 - Sair
Escolha uma opção: 2

--- Qual tipo de equipamento? ---
  1 - Notebooks
  2 - Projetores
  3 - Tablets
  0 - Voltar
Escolha uma opção: 2

--- Projetores disponíveis ---
  1 - Epson 410 (série 781245, 220V)
  2 - Sony Light 12 (série 903417, 220V)
  3 - HP FullCinema (série 2362198, 220V)
  0 - Voltar
Escolha uma opção: 1
Epson 410 reservado(a) com sucesso.
```

## Testes

```bash
dotnet test
```

Os testes cobrem reserva, devolução, permissões por perfil, cadastro e remoção de funcionários,
autenticação e hash de senhas. Eles rodam automaticamente a cada push pelo GitHub Actions.

## Próximos passos

- Persistência em banco de dados (SQLite com Entity Framework Core)
- Reservas com data e horário, evitando conflitos de agenda
- API REST em ASP.NET Core para uma interface web
