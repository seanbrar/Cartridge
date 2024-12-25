# Cartridge

Cartridge is a modern, focused NFS client for Windows, optimized for retro gaming scenarios. It evolved from [NFSClient](https://github.com/original/repo), reimagined specifically for serving retro game libraries over the network with minimal latency and maximum reliability.

## Philosophy

Cartridge takes an opinionated approach: it does one thing - NFS client operations for gaming workloads - and aims to do it exceptionally well. We deliberately exclude enterprise features like complex authentication or federation to maintain simplicity and performance.

Think of it as the network equivalent of plugging a game cartridge into your console - it should "just work" without configuration overhead.

## Features

- Streamlined NFS v3 support optimized for gaming workloads
- Modern Windows focus (Windows 10/11 64-bit)
- Clean, minimal interface
- Performance-first design decisions

## Requirements

- Windows 10 or Windows 11 (64-bit only)
- .NET Framework [version TBD]
- A compatible NFS server (tested configurations documented in the wiki)

## Status

This project is under active development. Current focus is on core functionality and stability. While usable, expect frequent updates as we refine the experience.

Some aspects of the project (including .NET version requirements and build processes) are still being finalized and will be documented as they are determined.

## Building

[Development environment setup and build instructions will go here]

## Contributing

Contributions are welcome, but please note our focused scope. We prioritize:
- Performance improvements
- Stability enhancements
- Bug fixes
- Documentation improvements

We do not accept features that deviate from our core gaming-focused use case.

## License

This project is licensed under GPL-3.0, as it builds upon the original NFSClient project. See [LICENSE](LICENSE) for details.

## Acknowledgments

Cartridge builds upon the foundation laid by NFSClient (link to original project). We're grateful for their work in creating a solid NFS client implementation for Windows.

---
*Cartridge: Because network storage shouldn't feel like network storage.*