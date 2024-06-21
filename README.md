# Secure Chat Application

## Overview
This project is a Secure Chat Application developed as part of the coursework for the Information Systems Programming course in the 2023/2024 academic year. The main objective of this project is to create a chat system with secure message exchange using C# (Windows Forms, Console Application, and Web App). The project consists of two modules: client and server.

## Authors
- [Maxim D.](https://github.com/maximdudai)
- [Ana G.](https://github.com/ana-fg)
- [Edo S.](https://github.com/18pingu18)

## Objectives
- **Client Module (UI)**:
  - Send public key
  - Authenticate to the server with credentials
  - Send and receive messages
  - Ensure secure communications
  - Validate messages using digital signatures

- **Server Module (No UI)**:
  - Accept client connections
  - Store client public keys
  - Authenticate registered users
  - Validate client signatures
  - Securely send and receive messages
  - Process message data securely

## Key Features
- **Client Module**:
  - User Interface to perform secure message exchange.
  - Public key exchange for secure communication.
  - Authentication using credentials.
  - Secure message exchange with digital signatures.
  
- **Server Module**:
  - Multi-client support.
  - Public key management and user authentication.
  - Message validation and secure data processing.

## Technologies
- **Sockets TCP/IP in .NET**
- **Cryptographic Algorithms in .NET**
- **ProtocolSI**
