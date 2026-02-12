using System.Security.AccessControl;
using System.Security.Principal;

namespace Cartridge.Core.FileSystem.Security
{
    public class NFSSecurityProvider
    {
        private readonly SecurityIdentifier _everyoneSid;
        private readonly SecurityIdentifier _adminSid;
        private readonly Dictionary<uint, SecurityIdentifier> _uidToSidCache;
        private readonly Dictionary<uint, SecurityIdentifier> _gidToSidCache;

        public NFSSecurityProvider()
        {
            _everyoneSid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            _adminSid = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
            _uidToSidCache = new Dictionary<uint, SecurityIdentifier>();
            _gidToSidCache = new Dictionary<uint, SecurityIdentifier>();
        }

        public string CreateSecurityDescriptor(uint uid, uint gid, uint mode)
        {
            var securityDescriptor = new RawSecurityDescriptor(
                DiscretionaryAcl: new RawAcl(
                    revision: 2,
                    capacity: 4),
                SystemAcl: null,
                Owner: GetOrCreateUserSid(uid),
                Group: GetOrCreateGroupSid(gid));

            var dacl = securityDescriptor.DiscretionaryAcl;
            
            // Add basic access rules based on Unix mode
            if ((mode & 0x100) != 0)  // Owner read
                AddAccessRule(dacl, GetOrCreateUserSid(uid), FileSystemRights.Read);
            if ((mode & 0x080) != 0)  // Owner write
                AddAccessRule(dacl, GetOrCreateUserSid(uid), FileSystemRights.Write);
            if ((mode & 0x040) != 0)  // Owner execute
                AddAccessRule(dacl, GetOrCreateUserSid(uid), FileSystemRights.ExecuteFile);

            // Group permissions
            if ((mode & 0x020) != 0)  // Group read
                AddAccessRule(dacl, GetOrCreateGroupSid(gid), FileSystemRights.Read);
            if ((mode & 0x010) != 0)  // Group write
                AddAccessRule(dacl, GetOrCreateGroupSid(gid), FileSystemRights.Write);
            if ((mode & 0x008) != 0)  // Group execute
                AddAccessRule(dacl, GetOrCreateGroupSid(gid), FileSystemRights.ExecuteFile);

            // Others permissions
            if ((mode & 0x004) != 0)  // Others read
                AddAccessRule(dacl, _everyoneSid, FileSystemRights.Read);
            if ((mode & 0x002) != 0)  // Others write
                AddAccessRule(dacl, _everyoneSid, FileSystemRights.Write);
            if ((mode & 0x001) != 0)  // Others execute
                AddAccessRule(dacl, _everyoneSid, FileSystemRights.ExecuteFile);

            return securityDescriptor.GetSddlForm(AccessControlSections.All);
        }

        public string GetDefaultSecurity()
        {
            // Create a default security descriptor that allows read access to everyone
            var securityDescriptor = new RawSecurityDescriptor(
                DiscretionaryAcl: new RawAcl(
                    revision: 2,
                    capacity: 1),
                SystemAcl: null,
                Owner: _adminSid,
                Group: _everyoneSid);

            var dacl = securityDescriptor.DiscretionaryAcl;
            AddAccessRule(dacl, _everyoneSid, FileSystemRights.ReadAndExecute);
            AddAccessRule(dacl, _adminSid, FileSystemRights.FullControl);

            return securityDescriptor.GetSddlForm(AccessControlSections.All);
        }

        private SecurityIdentifier GetOrCreateUserSid(uint uid)
        {
            if (_uidToSidCache.TryGetValue(uid, out var sid))
                return sid;

            // Create a unique SID for this UID
            // Note: In a production system, you might want to map to actual Windows accounts
            sid = new SecurityIdentifier($"S-1-5-21-3623811015-3361044348-30300820-{1000 + uid}");
            _uidToSidCache[uid] = sid;
            return sid;
        }

        private SecurityIdentifier GetOrCreateGroupSid(uint gid)
        {
            if (_gidToSidCache.TryGetValue(gid, out var sid))
                return sid;

            // Create a unique SID for this GID
            // Note: In a production system, you might want to map to actual Windows groups
            sid = new SecurityIdentifier($"S-1-5-21-3623811015-3361044348-30300820-{2000 + gid}");
            _gidToSidCache[gid] = sid;
            return sid;
        }

        private void AddAccessRule(RawAcl dacl, SecurityIdentifier sid, FileSystemRights rights)
        {
            dacl.InsertAce(
                index: dacl.Count,
                ace: new CommonAce(
                    AceFlags.None,
                    AceQualifier.AccessAllowed,
                    (int)rights,
                    sid,
                    false,
                    null));
        }
    }
}
