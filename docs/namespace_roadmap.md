Cartridge                    # Root namespace
├── Protocol                # All protocol-related code
│   ├── Types             # Core types (formerly Commons)
│   │   ├── NFSHandle
│   │   ├── NFSAttributes
│   │   ├── NFSPermission
│   │   └── etc.
│   │
│   ├── Rpc              # RPC implementation
│   │   ├── Client      # RPC client implementation
│   │   └── Xdr        # XDR serialization
│   │
│   └── Operations       # NFS operations (flattened from V3)
│       ├── Mount       # Mount protocol
│       └── Core        # Core NFS operations
│
└── FileSystem           # Dokan and caching
    └── Dokan          # Dokan implementation