# NFS Layer Architecture (Specialized for Retro ROM Access)

## Overview
Our specialized NFS client targets one main workflow: **low-latency, high-throughput access to game ROMs**. We omit or defer non-essential features—like file locking or symbolic link handling—to streamline performance and reduce complexity.

## Core Components

1. **Basic NFSv3 Operations**  
   - **LOOKUP**  
     - Retrieves file handles quickly.  
   - **READ**  
     - Optimized for sequential and random reads of game files.  
   - **GETATTR**  
     - Fetches metadata (size, timestamps) to support caching and read operations.  
   - **READDIRPLUS**  
     - Prefers a single call to gather directory listings and file attributes, improving directory scan speed.  
   - **Minimal Write Operations (CREATE, REMOVE, MKDIR, RMDIR)**  
     - Only if needed for occasional save/config file manipulation.  
   - **System Ops (FSSTAT, FSINFO, ACCESS, PATHCONF)**  
     - Implemented in a minimal form to provide basic filesystem data. Full coverage is not mandatory if rarely used.

2. **Caching & Buffering**
   - **Directory Cache**  
     - Stores entire directory listings (including attributes) for a configurable TTL.  
     - Designed for near-instant repeated scans of large ROM folders.  
   - **Attribute Cache**  
     - Caches file attributes (size, timestamps) to reduce repeated GETATTR calls.  
     - Time-based expiration (e.g., 30 seconds to a few minutes).  
   - **Read-ahead Buffering**  
     - Fetches data in chunks (e.g., 64 KB–1 MB) to reduce network round trips.  
     - Tunable parameter for varying network speeds and ROM sizes.

3. **Handle Management**
   - **Handle Cache**  
     - Retains file handles for repeated access to the same files/folders.  
     - Simple eviction strategy (e.g., LRU or time-based) to avoid memory bloat.  

4. **Error Handling**
   - **Transient Network Issues**  
     - Single retry for failing reads/LOOKUP calls.  
     - Surfaces error if retry also fails.  
   - **Stale Handles**  
     - Re-LOOKUP once; if still stale, return error.  
   - **Access/Permissions**  
     - Bubble up immediately for user or system-level fixes.

5. **Exclusions / Deferred Features**
   - **File Locking** (LOCK/LOCKU/LOCKT)  
     - Omitted due to read-mostly nature of ROM usage.  
   - **Symbolic Links** (SYMLINK, READLINK)  
     - Not essential unless required by specific environments.  
   - **Advanced Coherency / Write-back**  
     - Overkill for primarily read-only usage.  
   - **MKNOD, LINK**  
     - Rarely relevant for simple game file handling.

## Implementation Notes

1. **RPC/XDR Framework**  
   - A minimal, robust RPC engine to encode/decode request/response data.  
   - Lean approach with built-in support for the necessary NFSv3 structures.

2. **WinFsp Integration**  
   - The file system layer is exposed via WinFsp, allowing Windows to treat the remote share as a local drive.  
   - Ensures minimal overhead between the NFS layer and the Windows filesystem interface.

3. **Configuration Hooks**  
   - **Cache TTLs** (directory/attribute)  
   - **Read-ahead Block Size**  
   - **Retry Limit**  
   - **Log/Debug Level**

4. **Testing & Validation**  
   - **Unit Tests** for each NFS operation (serialization, deserialization, success/failure codes).  
   - **Integration Tests** against actual NFS servers (e.g., a typical NAS) focusing on large directory scans and concurrent file reads.

---

### Summary
By focusing on a small set of NFSv3 operations, adding strategic caching (especially for directories and file attributes), and employing read-ahead buffering for game files, we achieve a **fast, simplified NFS client** tailored to retro gaming needs. Additional or more complex operations (locks, symlinks, advanced caching) are postponed until they’re truly needed.