# NFS Implementation Roadmap (Refined)

## Completed Components

### Core NFS Client
- **✅ Basic NFSv3Client structure**  
  A foundational structure that organizes the client's main logic.

- **✅ Essential interface definitions (INFSv3Operations)**  
  Provides a contract for the core NFSv3 operations.

- **✅ Core operation implementations**  
  - **Directory Operations** (LOOKUP, READDIRPLUS)  
    - Implements basic directory operations needed for enumerating game files.
  - **File Operations** (READ, GETATTR)  
    - Supports essential read and attribute-fetching tasks for ROM access.
  - **File Management** (CREATE, REMOVE)  
    - Enables basic file lifecycle handling if needed (e.g., saving configuration or temporary data).
  - **Directory Management** (MKDIR, RMDIR)  
    - Allows creation/removal of directories, though might be minimal for a ROM-centered workflow.
  - **System Operations** (FSINFO, FSSTAT, ACCESS, PATHCONF)  
    - Retrieves high-level file system info and handles access checks.

- **✅ Base Args/Result classes**  
  Framework for constructing and parsing RPC/XDR calls/responses.

- **✅ Basic type definitions (NFSv3FileType, NFSv3Time)**  
  Core enumerations and time structures for NFS metadata.

### Data Structures
- **✅ Complete Args Classes**:
  - ✅ **LookupArgs**  
  - ✅ **ReadArgs**  
  - ✅ **WriteArgs**  
  - ✅ **CreateArgs**  
  - ✅ **RemoveArgs**  
  - ✅ **MkDirArgs**  
  - ✅ **RmDirArgs**  
  - ✅ **ReadDirPlusArgs**
  - ✅ **GetAttrArgs**
  - ✅ **System operation args**  
    - ACCESS, FSINFO, FSSTAT, PATHCONF

- **✅ Complete Result Classes**:
  - ✅ **LookupResult**  
  - ✅ **ReadResult**  
  - ✅ **WriteResult**  
  - ✅ **CreateResult**  
  - ✅ **RemoveResult**  
  - ✅ **MkDirResult**  
  - ✅ **RmDirResult**  
  - ✅ **ReadDirPlusResult**
  - ✅ **GetAttrResult**
  - ✅ **System operation results**  

### Caching Layer
- **✅ HandleCache implementation**  
  - Basic caching of file handles to reduce overhead from repeated LOOKUP calls.
  - Simple time-based expiration.

- **✅ DirectoryCache implementation**  
  - Caches directory entries and attributes with TTL.
  - Supports efficient directory scanning.
  - Includes memory management and entry limits.

- **✅ AttributeCache implementation**  
  - Caches file attributes (GETATTR results).
  - Time-based expiration with configurable TTL.

- **✅ Cache invalidation strategies**  
  - Time-based TTL implementation.
  - Manual invalidation support.
  - Cookie verifier checking for directory modifications.

## Next Steps

### File System Layer
- **[ ] WinFsp Integration**  
  - Hook NFS client into WinFsp for Windows filesystem exposure.
  - Implement necessary filesystem interfaces.
  - Handle path resolution and file operations.

### Performance Optimizations (Post-Alpha)
- **[ ] Read-ahead strategies**  
  - Implement read-ahead buffering for sequential ROM reads.
  - Configure block sizes for typical ROM sizes.

- **[ ] Directory prefetching**  
  - Optimize READDIRPLUS usage for large directories.
  - Implement aggressive directory prefetching.

### Error Handling (Post-Alpha)
- **[ ] Enhanced error handling**  
  - Specific NFS exceptions for common failures.
  - Retry strategies for transient failures.
  - Consistent error propagation.

### Documentation
- **[ ] API documentation**  
  - Document specialized fields/flags that handle caching and read-ahead.  
- **[ ] Usage examples**  
  - Include basic code snippets for mounting and reading a directory.  
- **[ ] Performance tuning guide**  
  - Explain how to adjust read-ahead, TTLs, and other caching parameters.  
- **[ ] Error handling guide**  
  - Document the expected error codes and recommended recovery steps.  

## Future Considerations

### Reliability
- **[ ] Retry strategies**  
  - Handle transient network failures gracefully.
- **[ ] Connection recovery**  
  - Transparent reconnection if the server drops the connection.
- **[ ] Stale handle handling**  
  - Fresh LOOKUP or simply fail if the handle is stale?
- **[ ] Error recovery procedures**  
  - Tie in with the logging/monitoring system for clear diagnostics.

### Monitoring/Debugging
- **[ ] Performance metrics**  
  - Per-operation latency, cache hit/miss ratio, etc.
- **[ ] Operation logging**  
  - Detailed logs of RPC calls for troubleshooting.
- **[ ] Cache statistics**  
  - Monitor usage patterns to tune TTLs or size.
- **[ ] Diagnostic tools**  
  - Integrate into UI? Real-time observation.

## Not Planned (Initial Release)
- **Complex file locking**  
  - Many retro emulators only read ROM files, so advanced locking is less critical.  
- **Symbolic links**  
- **Advanced handle recovery**  
  - Keep a basic approach for now.  
- **Complex cache coherency**  
  - Focus on simpler or time-based invalidation.  
- **Write-back caching**  
- **MKNOD operations**  
- **LINK operations**  

## Next Immediate Tasks
1. **Begin WinFsp integration**  
2. **Implement basic UI components**  
3. **Test basic filesystem operations**  
