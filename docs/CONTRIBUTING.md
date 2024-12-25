# Contributing to Cartridge

Cartridge is a focused project with a clear philosophy: provide a modern, minimal, high-performance NFS client specifically optimized for gaming workloads on Windows. This guide will help you understand what kinds of contributions align with this vision.

## Project Philosophy

Cartridge embraces:
- Performance-first design
- Modern Windows focus (Windows 10/11 64-bit only)
- Minimal, clean interfaces
- Stability and reliability
- Focused feature set

We explicitly avoid:
- Enterprise features
- Complex authentication systems
- Cross-platform support
- Feature bloat
- Legacy Windows support

## What We Welcome

We actively welcome contributions that:
- Improve performance
- Enhance stability
- Fix bugs
- Improve code quality
- Add tests
- Improve documentation
- Reduce complexity

## What We Don't Accept

To maintain our focused scope, we do not accept contributions that:
- Add new features beyond our core gaming use case
- Implement enterprise-focused capabilities
- Add support for older Windows versions
- Increase complexity without substantial benefits
- Add dependencies that aren't strictly necessary

## Development Guidelines

1. **Code Style**
   - Follow modern C# conventions
   - Embrace new language features
   - Keep code simple and focused
   - Add comments only when necessary to explain complex logic

2. **Performance**
   - Consider performance implications of all changes
   - Include benchmarks for performance-related changes
   - Be mindful of memory allocations

3. **Testing**
   - Include appropriate tests for new code
   - Verify changes work on Windows 10 and 11
   - Test with real gaming workloads

## Pull Request Process

1. Start with an issue discussion
2. Keep changes focused and minimal
3. Include clear commit messages
4. Update documentation if needed
5. Add tests for new code
6. Ensure all tests pass
7. Be open to feedback and iteration

## Questions?

If you're unsure whether your planned contribution aligns with our vision, please open an issue for discussion first. We appreciate your interest but maintain a high bar for changes to keep Cartridge focused and efficient.