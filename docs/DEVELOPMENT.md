# Cartridge Development Guide

## Project Status

### Current Focus
- Modernizing codebase to SDK-style project
- Investigating and fixing stability issues
- Improving mount/unmount reliability

### Known Issues
1. Stability
   - Connection drops after extended periods
   - Unmounting not graceful in all scenarios
   - Administrative access requirements causing crashes

2. Performance
   - Latency baseline needs investigation
   - Connection monitoring needed

3. User Experience
   - No background service support
   - Manual elevation required
   - No auto-mount capability

## Development Priorities

### Phase 1: Stabilization
1. [ ] Implement comprehensive logging
2. [ ] Fix administrative access crashes
3. [ ] Investigate connection drops
4. [ ] Improve unmount reliability

### Phase 2: Modernization
1. [ ] Complete SDK-style migration
2. [ ] Remove V2/V4 protocol support
3. [ ] Update Dokan integration
4. [ ] Modernize RPC layer

### Phase 3: Enhancement
1. [ ] Implement background service
2. [ ] Add auto-mount support
3. [ ] Add system tray integration
4. [ ] Implement automatic reconnection

## Testing Requirements
- Windows 11 compatibility verification
- Extended running tests (24h+)
- Network condition variation tests
- Permission/security testing

## Performance Goals
- Document baseline NFSv3 latency expectations
- Establish performance benchmarks
- Monitor resource usage

## Contributing
Before working on any changes, please read our [contribution guidelines](CONTRIBUTING.md) carefully. Cartridge maintains a focused scope and specific requirements for contributions.